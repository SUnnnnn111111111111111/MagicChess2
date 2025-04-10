using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(FigureInitializer))]
[RequireComponent(typeof(FigureLogic))]
[RequireComponent(typeof(FigureMover))]
[RequireComponent(typeof(EnemyKingDetector))]

public class Figure : MonoBehaviour
{
    [Header("Командная принадлежность")]
    public bool whiteTeamAffiliation;
    public bool isKing;
    public bool isPawn;

    [Header("Состояние фигуры")]
    public bool isFirstMove = true;
    public bool hasMovedThisTurn;
    public int countOfMovesIsOnEventTriggeringTile;

    [Header("Настройки движения")]
    public NeighborTilesSelectionSettings neighborTilesSelectionSettings;
    public NeighborTilesSelectionSettings fogNeighborTilesSelectionSettings;

    [Header("UI")]
    public FigureUIController uiPrefab;
    [HideInInspector] public FigureUIController uiController;

    [Header("UI Alerts")]
    public FigureAlertUIController alertUIController;

    [Header("Длительности")]
    public float delayBeforePassingTheMove = 0.5f;

    public Vector2Int CurrentPosition { get; set; }
    public Tile CurrentTile { get; set; }
    
    
}