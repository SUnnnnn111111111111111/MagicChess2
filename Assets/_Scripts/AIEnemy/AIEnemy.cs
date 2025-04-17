using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    public static AIEnemy Instance { get; private set; }
    public enum AITeam
    {
        White,
        Black
    }

    [SerializeField] private AITeam aiTeam;
    [SerializeField] private float delayBeforeMove = 0.5f;
    [SerializeField] private List<Figure> availableFigures = new List<Figure>();
    [SerializeField] private Figure selectedAIFigure;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (availableFigures == null)
            availableFigures = new List<Figure>();
    }
    
    private void OnEnable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private IEnumerator Start()
    {
        yield return null;
        UpdateAvailableFigures();
        HandleGameStateChanged(GameStateManager.Instance.CurrentState);
    }
    
    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged.RemoveListener(HandleGameStateChanged);
    }
    
    /// <summary>
    /// Обработчик изменения игрового состояния. При наступлении хода для AI инициируется выполнение хода.
    /// </summary>
    private void HandleGameStateChanged(GameStateManager.GameState newState)
    {
        if ((aiTeam == AITeam.Black && newState == GameStateManager.GameState.BlacksPlaying) ||
            (aiTeam == AITeam.White && newState == GameStateManager.GameState.WhitesPlaying))
        {
            StartCoroutine(AIMoveRoutine());
        }
    }

    public void SetTeam(AITeam team)
    {
        aiTeam = team;
    }
    
    /// <summary>
    /// Получает список фигур для AI через FiguresRepository.
    /// </summary>
    public List<Figure> GetFiguresList()
    {
        if (FiguresRepository.Instance == null)
        {
            Debug.LogWarning("FiguresRepository.Instance is null.");
            return new List<Figure>();
        }
        
        return aiTeam == AITeam.White 
            ? FiguresRepository.Instance.GetFiguresByTeam(true) 
            : FiguresRepository.Instance.GetFiguresByTeam(false);
    }
    
    /// <summary>
    /// Обновляет список фигур, доступных для AI.
    /// </summary>
    public void UpdateAvailableFigures()
    {
        availableFigures = GetFiguresList()
            .Where(f => f != null && f.gameObject != null)
            .ToList();
    }
    
    /// <summary>
    /// Основной цикл выполнения хода AI с задержкой.
    /// </summary>
    private IEnumerator AIMoveRoutine()
    {
        yield return new WaitForSeconds(delayBeforeMove);
    
        selectedAIFigure = GetWeightedFigure();
        if (selectedAIFigure == null)
        {
            Debug.LogWarning("Нет выбранной фигуры для хода AI");
            yield break;
        }
        
        var evaluationResult = TileScoringService.SelectBestTile(selectedAIFigure);
        Tile selectedTile = evaluationResult.tile;
        float selectedTileWeight = evaluationResult.weight;

        if (selectedTile == null)
        {
            Debug.LogWarning("Нет подходящей глобальной клетки для хода AI");
            yield break;
        }
        
        selectedAIFigure.Mover.TryMoveToTile(selectedTile);
    }
    
    /// <summary>
    /// Выбирает фигуру для хода AI с учетом веса. При выборе происходит подсветка доступных ходов.
    /// </summary>
    public Figure GetWeightedFigure()
    {
        UpdateAvailableFigures();
        
        if (availableFigures.Count == 0)
        {
            Debug.LogWarning("Нет фигур для команды " + aiTeam);
            return null;
        }
    
        Figure selectedFigure = FigureScoringService.SelectFigure(availableFigures);
        
        if (selectedFigure != null)
        {
            selectedFigure.Logic.HighlightAvailableToMoveTiles(includeFog: false);
        }
        
        return selectedFigure;
    }
}
