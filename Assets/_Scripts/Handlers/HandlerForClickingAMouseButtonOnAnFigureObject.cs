using UnityEngine;
using UltEvents;

public class HandlerForClickingAMouseButtonOnAnFigureObject : MonoBehaviour
{
    public bool isActive;
    [SerializeField] private UltEvent WhenClickOnAnObject;
    private Figure figure;

    private void Start()
    {
        figure = GetComponentInParent<Figure>();

        if (figure == null)
        {
            Debug.LogWarning($" {gameObject.name} (HandlerForClickingAMouseButtonOnAnObject) не нашёл Figure!");
        }
    }

    private void OnMouseDown()
    {
        if (GameStateManager.Instance.CurrentState == GameStateManager.GameState.Paused || 
            GameStateManager.Instance.CurrentState == GameStateManager.GameState.GameOver)
            return;
        
        bool isWhiteTurn = GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitePlaying;
        if (isWhiteTurn != figure.whiteTeamAffiliation) return;
        
        if (!isActive) return;

        WhenClickOnAnObject.Invoke();

        if (figure != null)
        {
            if (FigureManager.Instance.SelectedFigure == figure)
            {
                FigureManager.Instance.ResetSelection();
            }
            else
            {
                FigureManager.Instance.SelectedFigure = figure;
            }
        }
    }
}