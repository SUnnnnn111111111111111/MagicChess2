using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        StartMenu,
        WhitesPlaying,
        BlacksPlaying,
        Paused,
        WhitesLost,
        BlacksLost
    }
    [SerializeField] private GameState debugCurrentState;
    public GameState CurrentState
    {
        get => debugCurrentState;
        private set => debugCurrentState = value;
    }
    public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (Instance == null)
        {
            Instance = this;
            CurrentState = GameState.WhitesPlaying;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnGameStateChanged.Invoke(CurrentState);
    }

    public void SwitchTurn()
    {
        if (CurrentState == GameState.WhitesPlaying)
            SetGameState(GameState.BlacksPlaying);
        else if (CurrentState == GameState.BlacksPlaying)
            SetGameState(GameState.WhitesPlaying);

        // Обновляем туман войны через новый менеджер.
        FogOfWarManager.Instance.UpdateFogOfWar();
    }

    public void SetPaused(bool isPaused)
    {
        CurrentState = isPaused ? GameState.Paused : (CurrentState == GameState.WhitesPlaying ? GameState.WhitesPlaying : GameState.BlacksPlaying);
        OnGameStateChanged.Invoke(CurrentState);
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged.Invoke(CurrentState);
        HandleGameState(newState);
    }

    private void HandleGameState(GameState state)
    {
        switch (state)
        {
            case GameState.WhitesLost:
                // Debug.Log("Белые проиграли! Завершаем игру для белых.");
                break;
            case GameState.BlacksLost:
                // Debug.Log("Чёрные проиграли! Завершаем игру для чёрных.");
                break;
            case GameState.Paused:
                Debug.Log("Игра на паузе.");
                break;
            default:
                break;
        }
    }
}
