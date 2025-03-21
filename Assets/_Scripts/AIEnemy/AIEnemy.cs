using System.Collections;
using System.Collections.Generic;
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
        DontDestroyOnLoad(gameObject);
        
        if (availableFigures == null)
            availableFigures = new List<Figure>();
    }

    private IEnumerator Start()
    {
        yield return null;
        UpdateAvailableFigures();
    }
    
    private void OnEnable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }
    
    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnGameStateChanged.RemoveListener(HandleGameStateChanged);
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
            ? FiguresRepository.Instance.GetWhiteFigures() 
            : FiguresRepository.Instance.GetBlackFigures();
    }
    
    /// <summary>
    /// Обновляет список фигур, доступных для AI.
    /// </summary>
    public void UpdateAvailableFigures()
    {
        availableFigures = GetFiguresList();
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
        
        List<Tile> availableTiles = selectedAIFigure.GetAvailableToMoveTiles();
        if (availableTiles.Count == 0)
        {
            Debug.LogWarning("Нет доступных клеток для выбранной фигуры");
            yield break;
        }

        // Используем WeightedTileSelector для выбора клетки
        Tile selectedTile = WeightedTileSelector.SelectTile(availableTiles);
        FigureMover mover = selectedAIFigure.GetComponent<FigureMover>();
        if (mover != null)
        {
            mover.MoveToTile(selectedTile);
        }
        else
        {
            Debug.LogWarning("Не найден компонент FigureMover на выбранной фигуре");
        }
    }
    
    /// <summary>
    /// Выбирает фигуру для хода AI с учетом веса. При выборе происходит подсветка доступных ходов.
    /// </summary>
    public Figure GetWeightedFigure()
    {
        UpdateAvailableFigures();
        if (availableFigures == null || availableFigures.Count == 0)
        {
            Debug.LogWarning("Нет фигур для команды " + aiTeam);
            return null;
        }
    
        Figure selectedFigure = WeightedFigureSelector.SelectFigure(availableFigures);
        if (selectedFigure != null)
        {
            selectedFigure.HighlightAvailableToMoveTilesForAI();
        }
        return selectedFigure;
    }
}
