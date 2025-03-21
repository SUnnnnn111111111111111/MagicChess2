using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        WhitesPlaying,
        BlacksPlaying,
        Paused,
        WhitesLost,
        BlacksLost
    }
    
    public enum GameMode
    {
        LocalMultiplayer,
        VsAiEnemy
    }
    
    public GameState CurrentState{ get; private set;}
    public GameMode CurrentGameMode { get; set; }
    public bool humanPlaysWhite { get; set; }
    public bool HasMovedThisTurn { get; set; } = false;

    
    public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        if (Instance == null)
        {
            Instance = this;
            CurrentState = GameState.WhitesPlaying;
            CurrentGameMode = GameMode.LocalMultiplayer;
            humanPlaysWhite = true;
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
    
    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged.Invoke(CurrentState);
        HandleGameState(newState);
    }

    public void SwitchTurn()
    {
        HasMovedThisTurn = false;
        
        if (CurrentState == GameState.WhitesPlaying)
            SetGameState(GameState.BlacksPlaying);
        else if (CurrentState == GameState.BlacksPlaying)
            SetGameState(GameState.WhitesPlaying);
        
        FogOfWarManager.Instance.UpdateFogOfWar();
    }

    public void SetPaused(bool isPaused)
    {
        CurrentState = isPaused ? GameState.Paused : (CurrentState == GameState.WhitesPlaying ? GameState.WhitesPlaying : GameState.BlacksPlaying);
        OnGameStateChanged.Invoke(CurrentState);
    }
    
    public bool IsPlayersTurn()
    {
        if (CurrentGameMode == GameMode.VsAiEnemy)
        {
            return humanPlaysWhite ? CurrentState == GameState.WhitesPlaying : CurrentState == GameState.BlacksPlaying;
        }
        return true;
    }
}
