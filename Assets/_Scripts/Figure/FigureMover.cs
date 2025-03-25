using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(MeshSplitter))]
public class FigureMover : MonoBehaviour
{
    private Figure figure;
    private Quaternion originalRotation;

    private void Start()
    {
        figure = GetComponent<Figure>();
        if (figure != null)
        {
            originalRotation = figure.transform.rotation;
        }
        else
        {
            Debug.LogWarning("[Start] FigureMover не обнаружил компонент Figure на объекте.");
        }
    }

    public void MoveToTile(Tile targetTile)
    {
        if (GameStateManager.Instance.madeAFigureMoveAtThisTurn)
            return;

        if (figure == null || targetTile == null)
            return;

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation == figure.whiteTeamAffiliation)
            return;

        if (!targetTile.IsHighlighted)
            return;

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation)
        {
            CaptureEnemyAtTile(targetTile);
        }

        // Получаем настройки анимации через фабрику
        var animationSettings = AnimationSettingsFactory.GetAnimationSettings(figure.neighborTilesSelectionSettings);

        GameStateManager.Instance.madeAFigureMoveAtThisTurn = true;
        figure.hasMovedThisTurn = true;
        figure.isFirstMove = true;

        if (figure.CurrentTile != null)
            figure.CurrentTile.SetOccupyingFigure(null);

        originalRotation = figure.transform.rotation;
        Vector3 direction = (targetTile.transform.position - figure.transform.position).normalized;
        Vector3 lookAtPosition = figure.transform.position + direction;

        HighlightTilesManager.Instance.ClearHighlights();

        // Анимация перемещения
        figure.transform.DOLookAt(lookAtPosition, animationSettings.rotateDuration, AxisConstraint.Y)
            .OnComplete(() =>
            {
                Vector3 targetPos = targetTile.transform.position;
                targetPos.x = Mathf.Round(targetPos.x);
                targetPos.z = Mathf.Round(targetPos.z);
                targetPos.y = figure.transform.position.y;

                figure.transform.DOJump(targetPos, animationSettings.jumpPower, animationSettings.jumpCount, animationSettings.moveDuration)
                    .SetEase(animationSettings.moveEase)
                    .OnComplete(() =>
                    {
                        Tile previousTile = figure.CurrentTile;
                        SelectedFigureManager.Instance.SelectedFigure = null;
                        figure.CurrentTile = targetTile;
                        targetTile.SetOccupyingFigure(figure);

                        PawnMovementPromotionManager.Instance.HandlePawnMovementPromotion(figure, targetTile);

                        FogOfWarManager.Instance.UpdateFogOfWar();

                        figure.transform.DORotateQuaternion(originalRotation, animationSettings.rotateDuration);

                        DOVirtual.DelayedCall(figure.delayBeforePassingTheMove, () =>
                        {
                            GameStateManager.Instance.EndTurn();
                        });
                    });
            });
    }

    private void CaptureEnemyAtTile(Tile targetTile)
    {
        Figure enemyFigure = targetTile.OccupyingFigure;
        if (enemyFigure == null)
            return;

        FigureMover enemyMover = enemyFigure.GetComponent<FigureMover>();
        if (enemyMover != null)
        {
            enemyMover.StartCoroutine(enemyMover.DestroyEnemyFigure(enemyFigure));
        }
        else
        {
            Destroy(enemyFigure.gameObject);
        }
    }

    private IEnumerator DestroyEnemyFigure(Figure enemyFigure)
    {
        if (enemyFigure == null)
            yield break;

        // Запуск анимации разрушения через MeshSplitter
        MeshSplitter meshSplitter = enemyFigure.GetComponent<MeshSplitter>();
        if (meshSplitter != null)
        {
            meshSplitter.SplitMeshAndExplode(enemyFigure);
        }
        else
        {
            Debug.LogWarning("MeshSplitter не найден на фигуре.");
        }

        yield return new WaitForSeconds(enemyFigure.deathDelay);

        if (enemyFigure != null)
            Destroy(enemyFigure.gameObject);
    }
}

