﻿using System.Collections.Generic;
using System.Linq;
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
    public bool madeAFigureMoveAtThisTurn { get; set; }

    
    public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            OnGameStateChanged = new UnityEvent<GameState>();
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
                Debug.Log("Белые проиграли! Завершаем игру для белых.");
                break;
            case GameState.BlacksLost:
                Debug.Log("Чёрные проиграли! Завершаем игру для чёрных.");
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

    public void EndTurn()
    {
        if (CurrentState == GameState.WhitesPlaying) 
        {
            SetGameState(GameState.BlacksPlaying);
            madeAFigureMoveAtThisTurn = false;
            foreach (var figure in FiguresRepository.Instance.GetFiguresByTeam(false))
                figure.HasMovedThisTurn = false;
        }
        else if (CurrentState == GameState.BlacksPlaying)
        {
            SetGameState(GameState.WhitesPlaying);
            madeAFigureMoveAtThisTurn = false;
            foreach (var figure in FiguresRepository.Instance.GetFiguresByTeam(true))
                figure.HasMovedThisTurn = false;
        }
    
        KingThreatStateCache.Instance.UpdateThreats();
        
        foreach (var figure in FiguresRepository.Instance.AllFigures)
        {
            if (figure.KingDetector != null)
                figure.KingDetector.IsKingUnderAttack();
        }
    
        List<Figure> currentTeamFigures = (CurrentState == GameState.WhitesPlaying)
            ? FiguresRepository.Instance.GetFiguresByTeam(true) 
            : FiguresRepository.Instance.GetFiguresByTeam(false);
    
        foreach (var figure in currentTeamFigures)
        {
            EventTriggeringTileManager.Instance.HandleEventTrigger(figure, figure.CurrentTile);
        }

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
