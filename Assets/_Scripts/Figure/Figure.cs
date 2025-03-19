using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation;
    public bool isKing;

    [FormerlySerializedAs("neighborSelectionSettings")]
    public NeighborTilesSelectionSettings neighborTilesSelectionSettings;
    [FormerlySerializedAs("fogNeighborTilesSelectionSettings")]
    public NeighborTilesSelectionSettings fogNeighborTilesSelectionSettings;
    
    [Header("Death Animation")]
    public GameObject deathAnimationObject; 
    public float deathDelay = 0.5f;
    public float delayBeforePassingTheMove = 0.51f;
    
    private FigureMover figureMover;
    
    // Переименовано для согласованности с FiguresRepository (с возможностью записи)
    public Vector2Int CurrentPosition { get; set; }
    public Tile CurrentTile { get; set; }
    public bool HasMoved { get; set; }
    
    private void Start()
    {
        figureMover = GetComponent<FigureMover>();

        // Определяем позицию на основе transform.position (с учётом округления до int)
        CurrentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        
        // Получаем плитку по позиции и регистрируем фигуру через BoardManager
        CurrentTile = TilesRepository.Instance.GetTileAt(CurrentPosition);
        if (CurrentTile != null)
        {
            FiguresRepository.Instance.RegisterFigure(this, CurrentPosition);
            CurrentTile.SetOccupyingFigure(this);
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою клетку!");
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
    /// Вычисляет и подсвечивает доступные для перемещения клетки.
    /// </summary>
    public void HighlightAvailableToMoveTiles()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }

        MoveCalculator moveCalculator = GetMoveCalculator();
        // Вычисляем возможные ходы с фильтрацией по видимости и наличию стены
        List<Tile> moves = moveCalculator.CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation)
                              .Where(tile => !tile.HiddenByFog && !tile.isWall)
                              .ToList();

        // Разделяем ходы на пустые клетки и клетки с врагом
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
        if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Rectangle))
            return new RectangleMoveCalculator();
        if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.PawnWhite || 
                                                                         rule.neighborType == NeighborType.PawnBlack))
            return new PawnMoveCalculator();
        if (neighborTilesSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.Knight))
            return new KnightMoveCalculator();
        return new DefaultMoveCalculator(); 
    }
    
    /// <summary>
    /// Возвращает список доступных клеток для перемещения.
    /// </summary>
    public List<Tile> GetAvailableMoveTiles()
    {
        if (CurrentTile == null)
        {
            Debug.LogWarning($"Фигура {gameObject.name} не может найти свою текущую клетку!");
            return new List<Tile>();
        }
        
        MoveCalculator moveCalculator = GetMoveCalculator(); 
        List<Tile> moves = moveCalculator.CalculateMoves(CurrentTile, neighborTilesSelectionSettings, whiteTeamAffiliation)
                              .Where(tile => !tile.HiddenByFog && !tile.isWall)
                              .ToList();
        
        return moves.Where(tile => tile.OccupyingFigure == null ||
                                    (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation))
                    .ToList();
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
