using System.Collections.Generic;
using UnityEngine;

public class FiguresRepository : MonoBehaviour
{
    public static FiguresRepository Instance { get; private set; }
    
    private Dictionary<Vector2Int, Figure> whiteFigures = new Dictionary<Vector2Int, Figure>();
    private Dictionary<Vector2Int, Figure> blackFigures = new Dictionary<Vector2Int, Figure>();
    
    [SerializeField] private List<Figure> whiteFiguresDebug = new List<Figure>();
    [SerializeField] private List<Figure> blackFiguresDebug = new List<Figure>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterFigure(Figure figure, Vector2Int position)
    {
        figure.CurrentPosition = position;
        if (figure.whiteTeamAffiliation)
        {
            whiteFigures[position] = figure;
        }
        else
        {
            blackFigures[position] = figure;
        }
        UpdateDebugLists();
    }

    public void UnregisterFigure(Figure figure)
    {
        if (figure.whiteTeamAffiliation)
        {
            if (whiteFigures.ContainsKey(figure.CurrentPosition))
            {
                whiteFigures.Remove(figure.CurrentPosition);
            }
            if (figure.isKing || whiteFigures.Count == 0)
            {
                GameStateManager.Instance?.SetGameState(GameStateManager.GameState.WhitesLost);
            }
        }
        else
        {
            if (blackFigures.ContainsKey(figure.CurrentPosition))
            {
                blackFigures.Remove(figure.CurrentPosition);
            }
            if (figure.isKing || blackFigures.Count == 0)
            {
                GameStateManager.Instance?.SetGameState(GameStateManager.GameState.BlacksLost);
            }
        }
        UpdateDebugLists();
    }

    public List<Figure> GetWhiteFigures()
    {
        return new List<Figure>(whiteFigures.Values);
    }

    public List<Figure> GetBlackFigures()
    {
        return new List<Figure>(blackFigures.Values);
    }
    
    private void UpdateDebugLists()
    {
        whiteFiguresDebug = new List<Figure>(whiteFigures.Values);
        blackFiguresDebug = new List<Figure>(blackFigures.Values);
    }
}
