using UnityEngine;
using DG.Tweening;
using System.Collections;

public class FigureMover : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.5f; 
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); 
    [SerializeField] private float rotateDuration = 0.2f; 

    private Figure figure;
    private Quaternion originalRotation; 

    private void Start()
    {
        figure = GetComponent<Figure>();
        originalRotation = figure.transform.rotation; 
    }

    public void MoveToTile(Tile targetTile)
    {
        if (figure == null || targetTile == null) return;

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation == figure.whiteTeamAffiliation)
        {
            return;
        }

        if (!targetTile.IsHighlighted)
        {
            return;
        }
        

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation)
        {
            FigureMover enemyMover = targetTile.OccupyingFigure.GetComponent<FigureMover>();

            if (enemyMover != null)
            {
                enemyMover.StartCoroutine(enemyMover.DestroyEnemyFigure(targetTile.OccupyingFigure));
            }
            else
            {
                Destroy(targetTile.OccupyingFigure.gameObject);
            }
        }

        Tile currentTile = figure.GetCurrentTile();
        if (currentTile != null)
        {
            currentTile.SetOccupyingFigure(null);
        }
        
        originalRotation = figure.transform.rotation;
        
        Vector3 direction = (targetTile.transform.position - figure.transform.position).normalized;
        Vector3 lookAtPosition = figure.transform.position + direction;
        
        figure.transform.DOLookAt(lookAtPosition, rotateDuration, AxisConstraint.Y)
            .OnComplete(() =>
            {
                figure.transform.DOMove(targetTile.transform.position, moveDuration)
                    .SetEase(moveCurve) 
                    .OnComplete(() =>
                    {
                        figure.SetCurrentTile(targetTile);
                        targetTile.SetOccupyingFigure(figure);
                        
                        figure.transform.DORotateQuaternion(originalRotation, rotateDuration);

                        HighlightTilesController.Instance.ClearHighlights();
                        GameManager.Instance.SelectedFigure = null;
                    });
            });
    }

    private IEnumerator DestroyEnemyFigure(Figure enemyFigure)
    {
        if (enemyFigure == null) yield break;

        FigureMover enemyMover = enemyFigure.GetComponent<FigureMover>();
        if (enemyMover == null) yield break;

        if (enemyFigure.deathAnimationObject != null)
        {
            enemyFigure.deathAnimationObject.SetActive(true);
        }

        yield return new WaitForSeconds(enemyFigure.deathDelay);

        if (enemyFigure != null)
        {
            Destroy(enemyFigure.gameObject);
        }
    }
}
