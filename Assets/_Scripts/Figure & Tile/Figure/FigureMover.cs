using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class FigureMover
{
    private Figure figure;
    private Quaternion originalRotation;

    public FigureMover(Figure owner)
    {
        figure = owner;
        originalRotation = figure.transform.rotation;
    }

    public void TryMoveToTile(Tile targetTile)
    {
        if (GameStateManager.Instance.madeAFigureMoveAtThisTurn) 
            return;

        if (figure == null || targetTile == null)
            return;

        if (targetTile.IsHighlighted == false)
            return;

        if (targetTile.OccupyingFigure != null)
        {
            if (targetTile.OccupyingFigure.WhiteTeamAffiliation == figure.WhiteTeamAffiliation)
                return;

            CaptureEnemy(targetTile.OccupyingFigure);
        }

        ExecuteMoveSequence(targetTile);
    }

    private void ExecuteMoveSequence(Tile targetTile)
    {
        // Обработка рокировки для короля
        if (figure.IsKing)
        {
            if (CastlingService.IsCastlingPossibleInDirection(figure, new Vector2Int(6, 0), out var rook1, out var kingTarget1)
                && targetTile.Position == kingTarget1.Position)
            {
                CastlingService.ExecuteCastlingWithAnimation(figure, kingTarget1, rook1);
                return;
            }
            if (CastlingService.IsCastlingPossibleInDirection(figure, new Vector2Int(-8, 0), out var rook2, out var kingTarget2)
                && targetTile.Position == kingTarget2.Position)
            {
                CastlingService.ExecuteCastlingWithAnimation(figure, kingTarget2, rook2);
                return;
            }
        }

        var anim = AnimationSettingsFactory.GetAnimationSettings(figure.TilesSelectionSettings);

        GameStateManager.Instance.madeAFigureMoveAtThisTurn = true;
        figure.HasMovedThisTurn = true;
        figure.IsFirstMove = false;

        if (figure.CurrentTile != null)
        {
            figure.CurrentTile.SetOccupyingFigure(null);
        }

        Vector3 direction = (targetTile.transform.position - figure.transform.position).normalized;
        Vector3 lookAtTarget = figure.transform.position + direction;

        HighlightTilesManager.Instance.ClearHighlights();

        figure.transform.DOLookAt(lookAtTarget, anim.rotateDuration, AxisConstraint.Y).OnComplete(() =>
        {
            Vector3 finalPos = new Vector3(
                Mathf.Round(targetTile.transform.position.x),
                figure.transform.position.y,
                Mathf.Round(targetTile.transform.position.z)
            );

            figure.transform.DOJump(finalPos, anim.jumpPower, anim.jumpCount, anim.moveDuration)
                .SetEase(anim.moveEase)
                .OnComplete(() =>
                {
                    FinalizeMovement(targetTile);
                });
        });
    }

    public void MoveWithAnimationTo(Tile targetTile, Action onComplete = null)
    {
        var anim = AnimationSettingsFactory.GetAnimationSettings(figure.TilesSelectionSettings);

        figure.HasMovedThisTurn = true;
        figure.IsFirstMove = false;

        if (figure.CurrentTile != null)
            figure.CurrentTile.SetOccupyingFigure(null);

        Vector3 direction = (targetTile.transform.position - figure.transform.position).normalized;
        Vector3 lookAtTarget = figure.transform.position + direction;

        figure.transform.DOLookAt(lookAtTarget, anim.rotateDuration, AxisConstraint.Y).OnComplete(() =>
        {
            Vector3 finalPos = new Vector3(
                Mathf.Round(targetTile.transform.position.x),
                figure.transform.position.y,
                Mathf.Round(targetTile.transform.position.z)
            );

            figure.transform.DOJump(finalPos, anim.jumpPower, anim.jumpCount, anim.moveDuration)
                .SetEase(anim.moveEase)
                .OnComplete(() =>
                {
                    figure.CurrentTile = targetTile;
                    targetTile.SetOccupyingFigure(figure);
                    figure.CurrentPosition = targetTile.Position;

                    figure.transform.DORotateQuaternion(originalRotation, 0.2f);
                    onComplete?.Invoke();
                });
        });
    }

    private void FinalizeMovement(Tile newTile)
    {
        figure.CurrentTile = newTile;
        newTile.SetOccupyingFigure(figure);

        figure.CurrentPosition = new Vector2Int(
            Mathf.RoundToInt(newTile.transform.position.x),
            Mathf.RoundToInt(newTile.transform.position.z)
        );

        SelectedFigureManager.Instance.SelectedFigure = null;

        PawnMovementPromotionManager.Instance.HandlePawnMovementPromotion(figure, newTile);

        FogOfWarManager.Instance.UpdateFogOfWar();

        figure.transform.DORotateQuaternion(originalRotation, 0.2f);
        
        GameStateManager.Instance.EndTurn();
        
    }

    private void CaptureEnemy(Figure enemy)
    {
        DeathEffectFactory.Instance.CreateDeathEffect(enemy.transform);

        Tile enemyTile = enemy.CurrentTile;
        enemyTile.SetOccupyingFigure(null);

        if (FiguresRepository.Instance != null)
            FiguresRepository.Instance.UnregisterFigure(enemy);

        if (FogOfWarManager.Instance != null)
            FogOfWarManager.Instance.UpdateFogOfWar();

        if (SelectedFigureManager.Instance.SelectedFigure == enemy)
        {
            SelectedFigureManager.Instance.SelectedFigure = null;
        }

        Object.Destroy(enemy.gameObject);
    }
}
