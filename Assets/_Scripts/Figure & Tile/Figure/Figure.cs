using UnityEngine;
using UnityEngine.Serialization;

public class Figure : MonoBehaviour
{
    [field: Header("Командная принадлежность")]
    [field: SerializeField]
    public bool WhiteTeamAffiliation { get; private set; }
    [field: SerializeField] 
    public bool IsKing { get; private set; }
    [field: SerializeField] 
    public bool IsPawn { get; private set; }

    [field: Header("Состояние фигуры")] 
    [field: SerializeField] 
    public bool IsFirstMove { get; set; }

    [field: SerializeField] 
    public bool HasMovedThisTurn { get; set; }
    [field: SerializeField] 
    public int CountOfMovesIsOnEventTriggeringTile { get; set; }

    [field: Header("Настройки движения")] 
    [field: SerializeField] 
    public NeighborTilesSelectionSettings TilesSelectionSettings { get; set; }

    [field: SerializeField] 
    public NeighborTilesSelectionSettings FogNeighborTilesSelectionSettings { get; private set; }

    [field: Header("UI")] 
    [field: SerializeField] 
    public FigureUIController UIPrefab { get; private set; }
    [field: SerializeField] 
    public FigureUIController UIController { get; set; }
    [field: SerializeField] 
    public FigureAlertUIController AlertUIController { get; private set; }

    public Vector2Int CurrentPosition { get; set; }
    public Tile CurrentTile { get; set; }
    
    public FigureInitializer Initializer { get; private set; }
    public FigureLogic Logic { get; private set; }
    public FigureMover Mover { get; private set; }
    public EnemyKingDetector KingDetector { get; private set; }

    private void Awake()
    {
        Initializer = new FigureInitializer(this);
        Logic = new FigureLogic(this);
        Mover = new FigureMover(this);
        KingDetector = new EnemyKingDetector(this);
    }

    private void Start()
    {
        Initializer.Initialize();
    }

    private void OnDisable()
    {
        KingDetector.HideAlertsOnDisable();
    }
}