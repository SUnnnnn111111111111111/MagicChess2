using System.Collections.Generic;
using UnityEngine;

public class FiguresRepository : MonoBehaviour
{
    public static FiguresRepository Instance { get; private set; }

    private Dictionary<Vector2Int, Figure> whiteFigures = new Dictionary<Vector2Int, Figure>();
    private Dictionary<Vector2Int, Figure> blackFigures = new Dictionary<Vector2Int, Figure>();

#if UNITY_EDITOR
    [SerializeField] private List<Figure> whiteFiguresDebug = new List<Figure>();
    [SerializeField] private List<Figure> blackFiguresDebug = new List<Figure>();
#endif

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
    
    public List<Figure> AllFigures
    {
        get
        {
            List<Figure> all = new List<Figure>();
            all.AddRange(whiteFigures.Values);
            all.AddRange(blackFigures.Values);
            return all;
        }
    }

    public List<Figure> GetFiguresByTeam(bool white)
    {
        return white ? new List<Figure>(whiteFigures.Values)
                     : new List<Figure>(blackFigures.Values);
    }

    public void RegisterFigure(Figure figure, Vector2Int position)
    {
        figure.CurrentPosition = position;

        if (figure.WhiteTeamAffiliation)
            whiteFigures[position] = figure;
        else
            blackFigures[position] = figure;

#if UNITY_EDITOR
        UpdateDebugLists();
#endif
    }

    public void UnregisterFigure(Figure figure)
    {
        RemoveFigureFromDict(whiteFigures, figure);
        RemoveFigureFromDict(blackFigures, figure);


#if UNITY_EDITOR
        UpdateDebugLists();
#endif
    }

    public void ClearAll()
    {
        whiteFigures.Clear();
        blackFigures.Clear();

#if UNITY_EDITOR
        whiteFiguresDebug.Clear();
        blackFiguresDebug.Clear();
#endif
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

#if UNITY_EDITOR
    public void UpdateDebugLists()
    {
        whiteFiguresDebug = new List<Figure>(whiteFigures.Values);
        blackFiguresDebug = new List<Figure>(blackFigures.Values);
    }
#endif
}
