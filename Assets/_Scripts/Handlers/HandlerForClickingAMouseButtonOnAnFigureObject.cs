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
            GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitesLost || 
            GameStateManager.Instance.CurrentState == GameStateManager.GameState.BlacksLost)
            return;

        if (GameStateManager.Instance.CurrentGameMode == GameStateManager.GameMode.VsAiEnemy)
        {
            if (!GameStateManager.Instance.IsPlayersTurn())
                return;
        }
    
        bool isWhiteTurn = GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitesPlaying;
        if (isWhiteTurn != figure.whiteTeamAffiliation) return;
    
        if (!isActive) return;

        WhenClickOnAnObject.Invoke();

        if (figure != null)
        {
            if (SelectedFigureManager.Instance.SelectedFigure == figure)
            {
                SelectedFigureManager.Instance.ResetSelection();
            }
            else
            {
                SelectedFigureManager.Instance.SelectedFigure = figure;
            }
        }
    }
}