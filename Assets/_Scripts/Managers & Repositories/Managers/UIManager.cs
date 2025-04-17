using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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
    
    public void BindToGameStateManager()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged.AddListener(ShowGameState);
            ShowGameState(GameStateManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogWarning("[UIManager] GameStateManager.Instance не найден при попытке подписки.");
        }
    }
    
    public void ShowGameState(GameStateManager.GameState state)
    {
        UIFactory.Instance.gameStateUI?.UpdateGameState(state);
    }

    public void ShowFigureMoveCount(Figure figure, int current, int max)
    {
        if (figure.UIController != null)
        {
            figure.UIController.UpdateMoveCount(current, max);
        }
    }
}