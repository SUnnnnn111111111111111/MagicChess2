using UnityEngine;
using DG.Tweening;
using System.Collections;

public class FigureMover : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float rotateDuration = 0.2f;
    [SerializeField] private Ease moveEase = Ease.OutQuad; 
    [SerializeField] private float jumpPower = 1.0f; 
    [SerializeField] private int jumpCount = 1; 

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

    /// <summary>
    /// Перемещает фигуру на выбранную клетку с анимацией.
    /// </summary>
    public void MoveToTile(Tile targetTile)
{
    if (GameStateManager.Instance.HasMovedThisTurn)
        return;

    if (figure == null || targetTile == null)
    {
        return;
    }
    
    if (targetTile.OccupyingFigure != null &&
        targetTile.OccupyingFigure.whiteTeamAffiliation == figure.whiteTeamAffiliation)
    {
        return;
    }
        
    if (!targetTile.IsHighlighted)
    {
        return;
    }

    if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation)
    {
        CaptureEnemyAtTile(targetTile);
    }
    
    GameStateManager.Instance.HasMovedThisTurn = true;
    figure.isFirstMove = true;
    
    if (figure.CurrentTile != null)
        figure.CurrentTile.SetOccupyingFigure(null);
    
    originalRotation = figure.transform.rotation;
    Vector3 direction = (targetTile.transform.position - figure.transform.position).normalized;
    Vector3 lookAtPosition = figure.transform.position + direction;
    HighlightTilesManager.Instance.ClearHighlights();
    
    // Анимация перемещения
    figure.transform.DOLookAt(lookAtPosition, rotateDuration, AxisConstraint.Y)
        .OnComplete(() =>
        {
            Vector3 targetPos = targetTile.transform.position;
            targetPos.y = figure.transform.position.y;
            
            figure.transform.DOJump(targetPos, jumpPower, jumpCount, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() =>
                {
                    if (figure.CurrentTile.isEventTriggering) figure.countOfMovesIsOnEventTriggeringTile++;
                    SelectedFigureManager.Instance.SelectedFigure = null;
                    figure.CurrentTile = targetTile;
                    targetTile.SetOccupyingFigure(figure);
                    
                    FogOfWarManager.Instance.UpdateFogOfWar();
                    
                    figure.transform.DORotateQuaternion(originalRotation, rotateDuration);
                    
                    DOVirtual.DelayedCall(figure.delayBeforePassingTheMove, () =>
                    {
                        GameStateManager.Instance.SwitchTurn();
                    });
                });
        });
}

    /// <summary>
    /// Обрабатывает захват врага, инициируя анимацию уничтожения.
    /// </summary>
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

    /// <summary>
    /// Корутина для проигрывания анимации смерти врага и уничтожения объекта.
    /// </summary>
    private IEnumerator DestroyEnemyFigure(Figure enemyFigure)
    {
        if (enemyFigure == null)
            yield break;

        if (enemyFigure.deathAnimationObject != null)
            enemyFigure.deathAnimationObject.SetActive(true);

        yield return new WaitForSeconds(enemyFigure.deathDelay);

        if (enemyFigure != null)
            Destroy(enemyFigure.gameObject);
    }
}
