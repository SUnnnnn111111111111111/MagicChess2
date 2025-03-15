using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation; 
    public NeighborSelectionSettings neighborSelectionSettings; 
    public NeighborSelectionSettings fogNeighborSelectionSettings;
    
    [Header("Death Animation")]
    public GameObject deathAnimationObject; 
    public float deathDelay = 1.0f; 
    
    private FigureMover figureMover;
    
    public Vector2Int Position { get; private set; }
    public Tile CurrentTile { get; set; } 
    

    private void Start()
    {
        figureMover = GetComponent<FigureMover>();
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        CurrentTile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));
        
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        BoardManager.Instance.RegisterFigure(this, Position);

        if (CurrentTile != null)
        {
            CurrentTile.SetOccupyingFigure(this);
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
        
        BoardManager.Instance.UpdateFogOfWar(this);
    }

    private void OnDestroy()
    {
        BoardManager.Instance.UnregisterFigure(this);
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
        
        moves = moves.Where(tile => tile.HiddenByFog == false).ToList();

        List<Tile> emptyTiles = moves
            .Where(tile => tile.OccupyingFigure == null)
            .ToList();
        List<Tile> enemyTiles = moves
            .Where(tile => tile.OccupyingFigure != null &&
                           tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
            .ToList();

        HighlightTilesController.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesController.Instance.HighlightEnemyTiles(enemyTiles);
    }
    
    
    private MoveCalculator GetMoveCalculator()
    {
        if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Rectangle))
        {
            return new RectangleMoveCalculator();
        }
        else if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.PawnWhite || rule.neighborType == NeighborType.PawnBlack))
        {
            return new PawnMoveCalculator();
        }
        else if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Knight))
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