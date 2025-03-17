using UnityEngine;

public class FigureManager : MonoBehaviour
{
    public static FigureManager Instance { get; private set; }

    [SerializeField] private Figure selectedFigure;

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
        Instance = this;
    }

    public void ResetSelection()
    {
        SelectedFigure = null;
    }
}
