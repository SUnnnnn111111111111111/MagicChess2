using UnityEngine;

public class SelectedFigureManager : MonoBehaviour
{
    public static SelectedFigureManager Instance { get; private set; }

    private Figure selectedFigure;

    public Figure SelectedFigure
    {
        get => selectedFigure;
        set
        {
            selectedFigure = value;
            if (selectedFigure == null)
            {
                HighlightTilesManager.Instance.ClearHighlights();
            }
        }
    }

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

    public void ResetSelection()
    {
        SelectedFigure = null;
    }
}
