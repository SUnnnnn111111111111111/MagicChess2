using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation;
    public bool isKing;
    [FormerlySerializedAs("neighborSelectionSettings")] public NeighborTilesSelectionSettings neighborTilesSelectionSettings; 
    [FormerlySerializedAs("fogNeighborSelectionSettings")] public NeighborTilesSelectionSettings fogNeighborTilesSelectionSettings;
    
    [Header("Death Animation")]
    public GameObject deathAnimationObject; 
    public float deathDelay = 1.0f; 
    
    private FigureMover figureMover;
    
    public Vector2Int Position { get; private set; }
    public Tile CurrentTile { get; set; }
    public bool HasMoved { get; set; }
    

    private void Start()
    {
        figureMover = GetComponent<FigureMover>();
        
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        
        CurrentTile = BoardManager.Instance.GetTileAt(Position);
        
        if (CurrentTile != null)
        {
            BoardManager.Instance.RegisterFigure(this, Position);
            CurrentTile.SetOccupyingFigure(this);
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою клетку!");
        }
        
        BoardManager.Instance.UpdateFogOfWar();
    }

    private void OnDestroy()
    {
        BoardManager.Instance.UnregisterFigure(this);
        BoardManager.Instance.UpdateFogOfWar();
    }

    public void HighlightAvailableToMoveTiles()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }

        MoveCalculator moveCalculator = GetMoveCalculator();
        List<Tile> moves = moveCalculator.CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation);
        
        moves = moves.Where(tile => tile.HiddenByFog == false).ToList();
        moves = moves.Where(tile => !tile.HiddenByFog && !tile.isWall).ToList();

        List<Tile> emptyTiles = moves
            .Where(tile => tile.OccupyingFigure == null)
            .ToList();
        List<Tile> enemyTiles = moves
            .Where(tile => tile.OccupyingFigure != null &&
                           tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
            .ToList();

        HighlightTilesManager.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesManager.Instance.HighlightEnemyTiles(enemyTiles);
    }
    
    private MoveCalculator GetMoveCalculator()
    {
        if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Rectangle))
        {
            return new RectangleMoveCalculator();
        }
        else if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.PawnWhite || rule.neighborType == NeighborType.PawnBlack))
        {
            return new PawnMoveCalculator();
        }
        else if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Knight))
        {
            return new KnightMoveCalculator();
        }
        else
        {
            return new DefaultMoveCalculator(); 
        }
    }
    
    public List<Tile> GetAvailableMoveTiles()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"Фигура {gameObject.name} не может найти свою текущую клетку!");
            return new List<Tile>();
        }
        
        MoveCalculator moveCalculator = GetMoveCalculator(); 
        List<Tile> moves = moveCalculator.CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation);
        
        moves = moves.Where(tile => !tile.HiddenByFog && !tile.isWall).ToList();
        
        List<Tile> availableTiles = moves
            .Where(tile => 
                tile.OccupyingFigure == null || 
                (tile.OccupyingFigure != null && 
                 tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation))
            .ToList();
        return availableTiles;
    }
    
    public int GetAvailableMovesCount()
    {
        if (CurrentTile == null) return 0;
        return GetMoveCalculator().CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation).Count;
    }

    public bool IsCurrentTileHighlighted() => CurrentTile?.IsHighlighted ?? false;
    
    public string GetCurrentTilePosition() => CurrentTile?.Position.ToString() ?? "None";
}