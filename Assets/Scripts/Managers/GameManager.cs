using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Figure selectedFigure;

    public Figure SelectedFigure
    {
        get => selectedFigure;
        set
        {
            selectedFigure = value;
            if (selectedFigure == null)
            {
                HighlightTilesController.Instance.ClearHighlights();
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
