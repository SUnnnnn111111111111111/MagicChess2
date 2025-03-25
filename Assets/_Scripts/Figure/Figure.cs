using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

[RequireComponent(typeof(FigureMover))]
public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation;
    public bool isKing;
    public bool isPawn;
    public bool isFirstMove;
    public bool hasMovedThisTurn;
    public FigureUIController uiPrefab;
    [HideInInspector] public FigureUIController uiController;

    [FormerlySerializedAs("neighborSelectionSettings")]
    public NeighborTilesSelectionSettings neighborTilesSelectionSettings;
    [FormerlySerializedAs("fogNeighborTilesSelectionSettings")]
    public NeighborTilesSelectionSettings fogNeighborTilesSelectionSettings;
    
    public float deathDelay = 0.5f;
    public float delayBeforePassingTheMove = 0.51f;
    
    private FigureMover figureMover;
    
    public Vector2Int CurrentPosition { get; set; }
    public Tile CurrentTile { get; set; }
    public int countOfMovesIsOnEventTriggeringTile { get; set; }
    
    

    private void Start()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
        );

        CurrentPosition = new Vector2Int(
            (int)transform.position.x,
            (int)transform.position.z
        );
        
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
        
        figureMover = GetComponent<FigureMover>();
        
        if (neighborTilesSelectionSettings != null)
        {
            neighborTilesSelectionSettings = Instantiate(neighborTilesSelectionSettings);
        }
        
        if (uiPrefab != null)
        {
            uiController = Instantiate(uiPrefab, transform.position, Quaternion.identity, transform);
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
    
    public void SetGridPosition(Vector2Int newPosition)
    {
        Vector3 newPos = new Vector3(newPosition.x, transform.position.y, newPosition.y);
        transform.position = newPos;
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
    public void HighlightAvailableToMoveTiles(bool includeFog)
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"[HighlightAvailableToMoveTiles] Фигура {gameObject.name} не может найти текущую клетку!");
            return;
        }
    
        List<Tile> moves = GetAvailableToMoveTiles();
    
        if (includeFog)
            moves = moves.Where(tile => !tile.HiddenByFog).ToList();

        List<Tile> emptyTiles = moves.Where(tile => tile.OccupyingFigure == null).ToList();
        List<Tile> enemyTiles = moves.Where(tile => tile.OccupyingFigure != null && 
                                                    tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation).ToList();

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

