using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    public enum AITeam
    {
        White,
        Black
    }

    [SerializeField] private AITeam aiTeam;
    [SerializeField] private float delayBeforeMove = 0.5f;
    [SerializeField] private List<Figure> availableFigures;
    [SerializeField] private Figure selectedAIFigure;

    private void Awake()
    {
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
    
    public Dictionary<Vector2Int, Figure> GetFiguresDictionary()
    {
        bool isWhite = (aiTeam == AITeam.White);
        
        if (BoardManager.Instance == null)
        {
            Debug.LogWarning("BoardManager.Instance is null.");
            return new Dictionary<Vector2Int, Figure>();
        }
        
        return BoardManager.Instance.GetFiguresDictionary(isWhite);
    }
    
    public void UpdateAvailableFigures()
    {
        Dictionary<Vector2Int, Figure> figuresDict = GetFiguresDictionary();
        if (figuresDict != null && figuresDict.Count > 0)
            availableFigures = new List<Figure>(figuresDict.Values);
        else
            availableFigures.Clear();
    }
    
    private void HandleGameStateChanged(GameStateManager.GameState newState)
    {
        if ((aiTeam == AITeam.Black && newState == GameStateManager.GameState.BlacksPlaying) ||
            (aiTeam == AITeam.White && newState == GameStateManager.GameState.WhitesPlaying))
        {
            StartCoroutine(AIMoveRoutine());
        }
    }
    
    private IEnumerator AIMoveRoutine()
    {
        yield return new WaitForSeconds(delayBeforeMove);

        selectedAIFigure = GetWeightedFigure();
        if (selectedAIFigure == null)
        {
            Debug.LogWarning("Нет выбранной фигуры для хода AI");
            yield break;
        }

        List<Tile> availableTiles = selectedAIFigure.GetAvailableMoveTiles();
        if (availableTiles.Count == 0)
        {
            Debug.LogWarning("Нет доступных клеток для выбранной фигуры");
            yield break;
        }

        // Для перемещения можно использовать уже реализованный WeightedTileSelector или другой метод.
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
            // Можно подсветить доступные ходы для выбранной фигуры, если требуется.
            selectedFigure.HighlightAvailableToMoveTiles();
        }
        return selectedFigure;
    }
}