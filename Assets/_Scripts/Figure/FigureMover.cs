using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Figure))]
[RequireComponent(typeof(MeshSplitter))]
public class FigureMover : MonoBehaviour
{
    private Figure figure;
    private Quaternion originalRotation;

    private void Awake()
    {
        figure = GetComponent<Figure>();
        originalRotation = figure.transform.rotation;
    }

    public void TryMoveToTile(Tile targetTile)
    {
        if (GameStateManager.Instance.madeAFigureMoveAtThisTurn)
            return;

        if (figure == null || targetTile == null)
            return;

        if (!targetTile.IsHighlighted)
            return;

        if (targetTile.OccupyingFigure != null)
        {
            if (targetTile.OccupyingFigure.whiteTeamAffiliation == figure.whiteTeamAffiliation)
                return;

            CaptureEnemy(targetTile.OccupyingFigure);
        }

        ExecuteMoveSequence(targetTile);
    }

    private void ExecuteMoveSequence(Tile targetTile)
    {
        var anim = AnimationSettingsFactory.GetAnimationSettings(figure.neighborTilesSelectionSettings);

        GameStateManager.Instance.madeAFigureMoveAtThisTurn = true;
        figure.hasMovedThisTurn = true;
        figure.isFirstMove = true;

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

    private void FinalizeMovement(Tile newTile)
    {
        Tile previousTile = figure.CurrentTile;
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

        DOVirtual.DelayedCall(figure.delayBeforePassingTheMove, () =>
        {
            GameStateManager.Instance.EndTurn();
        });
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
        
        Destroy(enemy.gameObject);
    }
}
