using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        StartMenu,
        WhitePlaying,
        BlackPlaying,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }
    public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CurrentState = GameState.WhitePlaying;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchTurn()
    {
        if (CurrentState == GameState.WhitePlaying)
            CurrentState = GameState.BlackPlaying;
        else if (CurrentState == GameState.BlackPlaying)
            CurrentState = GameState.WhitePlaying;
        
        OnGameStateChanged.Invoke(CurrentState);
        BoardManager.Instance.UpdateFogOfWar();
    }
    
    public void SetPaused(bool isPaused)
    {
        CurrentState = isPaused ? GameState.Paused : (CurrentState == GameState.WhitePlaying ? GameState.WhitePlaying : GameState.BlackPlaying);
        OnGameStateChanged.Invoke(CurrentState);
    }

    
    public void EndGame()
    {
        CurrentState = GameState.GameOver;
        OnGameStateChanged.Invoke(CurrentState);
    }
}