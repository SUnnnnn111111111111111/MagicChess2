using System;
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        RemoveFigureFromDict(whiteFigures, figure);
        RemoveFigureFromDict(blackFigures, figure);

        if (figure.isKing || GetFiguresCount(figure.whiteTeamAffiliation) == 0)
        {
            var state = figure.whiteTeamAffiliation
                ? GameStateManager.GameState.WhitesLost
                : GameStateManager.GameState.BlacksLost;

            GameStateManager.Instance?.SetGameState(state);
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
    
    public void UpdateDebugLists()
    {
        whiteFiguresDebug = new List<Figure>(whiteFigures.Values);
        blackFiguresDebug = new List<Figure>(blackFigures.Values);
    }
    

    private void RemoveFigureFromDict(Dictionary<Vector2Int, Figure> dict, Figure target)
    {
        foreach (var kvp in dict)
        {
            if (kvp.Value == target)
            {
                dict.Remove(kvp.Key);
                break;
            }
        }
    }
    
    private int GetFiguresCount(bool white)
    {
        return white ? whiteFigures.Count : blackFigures.Count;
    }
}
