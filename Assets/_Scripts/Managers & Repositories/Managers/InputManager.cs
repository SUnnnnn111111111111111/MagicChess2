using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

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
    
    private void OnEnable()
    {
        ClickHandler.FigureClicked += OnFigureClicked;
        ClickHandler.TileClicked   += OnTileClicked;
    }

    private void OnDisable()
    {
        ClickHandler.FigureClicked -= OnFigureClicked;
        ClickHandler.TileClicked   -= OnTileClicked;
    }
    
    private void OnFigureClicked(Figure clickedFigure)
    {
        if (FigureSelector.CanSelect(clickedFigure) == false)
            return;
        
        if (SelectedFigureManager.Instance.SelectedFigure == clickedFigure)
        {
            SelectedFigureManager.Instance.ResetSelection();
        }
        else
        {
            SelectedFigureManager.Instance.SelectedFigure = clickedFigure;
            clickedFigure.Logic.HighlightAvailableToMoveTiles(includeFog: false);
        }
    }
    
    private void OnTileClicked(Tile clickedTile)
    {
        if (clickedTile.HiddenByFog)
            return;
        
        Figure selectedFigure = SelectedFigureManager.Instance.SelectedFigure;
        if (selectedFigure != null)
        {
            selectedFigure.Mover.TryMoveToTile(clickedTile);
        }
    }
}