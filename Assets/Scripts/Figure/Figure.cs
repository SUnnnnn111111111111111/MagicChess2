using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation; 
    public NeighborSelectionSettings neighborSelectionSettings; 
    
    [Header("Death Animation")]
    [SerializeField] private GameObject deathAnimationObject; // Дочерний объект для анимации смерти
    [SerializeField] private float deathDelay = 1.0f; // Задержка перед уничтожением фигуры
    
    private Tile currentTile; 

    private void Start()
    {
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        currentTile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (currentTile != null)
        {
            currentTile.SetOccupyingFigure(this);
            // Debug.Log($"✅ Фигура {gameObject.name} зарегистрирована на клетке {currentTile.Position}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
    }
    
    public void HighlightAvailableToMoveTiles()
    {
        if (currentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }

        MoveCalculator moveCalculator = GetMoveCalculator();
        List<Tile> moves = moveCalculator.CalculateMoves(currentTile, neighborSelectionSettings, whiteTeamAffiliation);

        List<Tile> emptyTiles = moves.Where(tile => tile.OccupyingFigure == null).ToList();
        List<Tile> enemyTiles = moves.Where(tile => tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation).ToList();

        HighlightTilesController.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesController.Instance.HighlightEnemyTiles(enemyTiles);
    }
    
    
    private MoveCalculator GetMoveCalculator()
    {
        if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.WhitePawn || rule.neighborType == NeighborType.BlackPawn))
        {
            return new PawnMoveCalculator();
        }
        else if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.KnightMove))
        {
            return new KnightMoveCalculator();
        }
        else
        {
            return new DefaultMoveCalculator(); 
        }
    }
    
    public void MoveToTile(Tile targetTile)
    {
        if (targetTile == null)
        {
            return;
        }

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation == whiteTeamAffiliation)
        {
            return;
        }

        if (!targetTile.IsHighlighted)
        {
            return;
        }

        Debug.Log($"🔄 Фигура {gameObject.name} перемещается на клетку {targetTile.Position}.");
        
        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
        {
            Debug.Log($"✅ Фигура {targetTile.OccupyingFigure.name} атакована.");
            StartCoroutine(DestroyEnemyFigure(targetTile.OccupyingFigure, targetTile.transform.position)); 
        }

        currentTile.SetOccupyingFigure(null);

        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        currentTile.SetOccupyingFigure(this);

        Debug.Log($"✅ Фигура {gameObject.name} завершила перемещение.");

        HighlightTilesController.Instance.ClearHighlights();
        GameManager.Instance.SelectedFigure = null;
    }
    
    private IEnumerator DestroyEnemyFigure(Figure enemyFigure, Vector3 deathPosition)
    {
        // Активируем анимацию смерти
        if (deathAnimationObject != null)
        {
            Debug.Log($"✅ Фигура {enemyFigure.gameObject.name} запустила анимацию смерти.");
            enemyFigure.deathAnimationObject.SetActive(true);
        }

        // Ждем указанное время перед уничтожением фигуры
        yield return new WaitForSeconds(deathDelay);

        // Уничтожаем вражескую фигуру
        if (enemyFigure != null)
        {
            Destroy(enemyFigure.gameObject);
            Debug.Log($"✅ Фигура {enemyFigure.gameObject.name} уничтожена.");
        }
    }
}