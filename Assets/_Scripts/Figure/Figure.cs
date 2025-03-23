using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation;
    public bool isKing;
    public bool isPawn;

    [FormerlySerializedAs("neighborSelectionSettings")]
    public NeighborTilesSelectionSettings neighborTilesSelectionSettings;
    [FormerlySerializedAs("fogNeighborTilesSelectionSettings")]
    public NeighborTilesSelectionSettings fogNeighborTilesSelectionSettings;
    
    [Header("Death Animation")]
    public GameObject deathAnimationObject;
    public float deathDelay = 0.5f;
    public float delayBeforePassingTheMove = 0.51f;
    
    private FigureMover figureMover;
    
    public Vector2Int CurrentPosition { get; set; }
    public Tile CurrentTile { get; set; }
    public bool isFirstMove { get; set; }
    public bool hasMovedThisTurn { get; set; }
    public int countOfMovesIsOnEventTriggeringTile { get; set; }
    
    private void Start()
    {
        figureMover = GetComponent<FigureMover>();

        CurrentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        
        CurrentTile = TilesRepository.Instance.GetTileAt(CurrentPosition);
        if (CurrentTile != null)
        {
            FiguresRepository.Instance.RegisterFigure(this, CurrentPosition);
            CurrentTile.SetOccupyingFigure(this);
        }
        else
        {
            Debug.LogWarning($"[Start] Фигура {gameObject.name} не нашла свою клетку!");
        }
        
        if (neighborTilesSelectionSettings != null)
        {
            neighborTilesSelectionSettings = Instantiate(neighborTilesSelectionSettings);
        }
        
        FogOfWarManager.Instance.UpdateFogOfWar();
    }

    private void OnDestroy()
    {
        if (TilesRepository.Instance != null)
        {
            FiguresRepository.Instance.UnregisterFigure(this);
        }
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.UpdateFogOfWar();
        }
    }

    /// <summary>
    /// Возвращает список доступных клеток для перемещения.
    /// Проверяется только наличие стены и фигуры (с учётом команды).
    /// </summary>
    public List<Tile> GetAvailableToMoveTiles()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"[GetAvailableToMoveTiles] Фигура {gameObject.name} не может найти свою текущую клетку!");
            return new List<Tile>();
        }
        
        MoveCalculator moveCalculator = GetMoveCalculator();
        List<Tile> moves = moveCalculator.CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation)
                              .Where(tile => !tile.isWall) 
                              .ToList();
        
        return moves.Where(tile => tile.OccupyingFigure == null ||
                                    (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation))
                    .ToList();
    }

    /// <summary>
    /// Подсвечивает доступные для перемещения клетки для игрока,
    /// фильтруя те, на которых присутствует туман.
    /// </summary>
    public void HighlightAvailableToMoveTilesForPlayer()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"[HighlightAvailableToMoveTilesForPlayer] Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }
        
        List<Tile> moves = GetAvailableToMoveTiles();
        
        List<Tile> visibleMoves = moves.Where(tile => !tile.HiddenByFog).ToList();

        List<Tile> emptyTiles = visibleMoves.Where(tile => tile.OccupyingFigure == null).ToList();
        List<Tile> enemyTiles = visibleMoves.Where(tile => tile.OccupyingFigure != null &&
                                                            tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                                            .ToList();

        HighlightTilesManager.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesManager.Instance.HighlightEnemyTiles(enemyTiles);
    }
    
    /// <summary>
    /// Подсвечивает доступные для перемещения клетки для AI,
    /// не учитывая наличие тумана.
    /// </summary>
    public void HighlightAvailableToMoveTilesForAI()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"[HighlightAvailableToMoveTilesForAI] Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }
        
        List<Tile> moves = GetAvailableToMoveTiles();

        List<Tile> emptyTiles = moves.Where(tile => tile.OccupyingFigure == null).ToList();
        List<Tile> enemyTiles = moves.Where(tile => tile.OccupyingFigure != null &&
                                                    tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                                     .ToList();
        
        
        HighlightTilesManager.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesManager.Instance.HighlightEnemyTiles(enemyTiles);
    }

    /// <summary>
    /// Выбирает нужный алгоритм подсчёта ходов в зависимости от настроек.
    /// </summary>
    private MoveCalculator GetMoveCalculator()
    {
        if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Rectangle ||
                                                                        rule.neighborType == NeighborType.Knight ||
                                                                        rule.neighborType == NeighborType.KnightDouble ||
                                                                        rule.neighborType == NeighborType.KnightPlus || 
                                                                        rule.neighborType == NeighborType.ZigZag ||
                                                                        rule.neighborType == NeighborType.Circular ||
                                                                        rule.neighborType == NeighborType.Star))
            return new RectangleMoveCalculator();
        else if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.PawnWhite || 
                                                                             rule.neighborType == NeighborType.PawnBlack))
            return new PawnMoveCalculator();
        else
            return new DefaultMoveCalculator();
    }
    
    public int GetAvailableMovesCount()
    {
        if (CurrentTile == null)
            return 0;
        return GetMoveCalculator().CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation).Count;
    }

    public bool IsCurrentTileHighlighted() => CurrentTile?.IsHighlighted ?? false;
    
    public string GetCurrentTilePosition() => CurrentTile?.Position.ToString() ?? "None";
}

