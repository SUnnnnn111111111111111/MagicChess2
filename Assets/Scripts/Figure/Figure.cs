using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation; 
    public NeighborSelectionSettings neighborSelectionSettings; 
    
    [Header("Death Animation")]
    public GameObject deathAnimationObject; 
    public float deathDelay = 1.0f; 
    
    private FigureMover figureMover;
    
    public Tile CurrentTile { get; set; } 

    private void Start()
    {
        figureMover = GetComponent<FigureMover>();
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        CurrentTile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (CurrentTile != null)
        {
            CurrentTile.SetOccupyingFigure(this);
            // Debug.Log($"✅ Фигура {gameObject.name} зарегистрирована на клетке {currentTile.Position}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
    }
    
    public void HighlightAvailableToMoveTiles()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }

        MoveCalculator moveCalculator = GetMoveCalculator();
        List<Tile> moves = moveCalculator.CalculateMoves(CurrentTile, neighborSelectionSettings, whiteTeamAffiliation);

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
    

    public int GetAvailableMovesCount()
    {
        if (CurrentTile == null) return 0;
        return GetMoveCalculator().CalculateMoves(CurrentTile, neighborSelectionSettings, whiteTeamAffiliation).Count;
    }

    public bool IsCurrentTileHighlighted() => CurrentTile?.IsHighlighted ?? false;
    
    public string GetCurrentTilePosition() => CurrentTile?.Position.ToString() ?? "None";
}