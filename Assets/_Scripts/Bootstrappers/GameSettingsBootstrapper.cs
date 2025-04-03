using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsBootstrapper : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "MainGameScene";

    private enum BoardType { Default, Test }
    private static BoardType boardToLoad = BoardType.Default;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (boardToLoad)
        {
            case BoardType.Default:
                BoardFactory.Instance.LoadDefaultBoard();
                break;
            case BoardType.Test:
                BoardFactory.Instance.LoadTestBoard();
                break;
        }

        UIFactory.Instance.LoadGameStateUI();
        UIManager.Instance.BindToGameStateManager();
        EventTriggeringTileManager.Instance.ReplaseTiles();
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.WhitesPlaying);
        KingThreatStateCache.Instance.InvalidateCache();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LaunchSettings()
    {
        Debug.Log("[GameSettingsBootstrapper] SettingsUI режим еще не реализован.");
    }

    public void LaunchGameWithBlackAIEnemy()
    {
        boardToLoad = BoardType.Default;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.CurrentGameMode = GameStateManager.GameMode.VsAiEnemy;
            GameStateManager.Instance.humanPlaysWhite = true;
        }

        if (AIEnemy.Instance == null)
        {
            GameObject aiObj = Instantiate(Resources.Load("Prefabs/AIEnemy/AIEnemy")) as GameObject;
            AIEnemy ai = aiObj.GetComponent<AIEnemy>();
            if (ai != null)
            {
                ai.SetTeam(AIEnemy.AITeam.Black);
            }
        }
        else
        {
            Debug.LogWarning("[GameSettingsBootstrapper] AIEnemy уже существует.");
        }

        SceneManager.LoadScene(mainGameSceneName);
    }

    public void LaunchGameWithWhiteAIEnemy()
    {
        boardToLoad = BoardType.Default;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.CurrentGameMode = GameStateManager.GameMode.VsAiEnemy;
            GameStateManager.Instance.humanPlaysWhite = false;
        }
        else
        {
            Debug.LogWarning("[GameSettingsBootstrapper] GameStateManager не найден!");
        }

        if (AIEnemy.Instance == null)
        {
            GameObject aiObj = Instantiate(Resources.Load("Prefabs/AIEnemy/AIEnemy")) as GameObject;
            AIEnemy ai = aiObj.GetComponent<AIEnemy>();
            if (ai != null)
            {
                ai.SetTeam(AIEnemy.AITeam.White);
            }
        }
        else
        {
            Debug.LogWarning("[GameSettingsBootstrapper] AIEnemy уже существует.");
        }

        SceneManager.LoadScene(mainGameSceneName);
    }

    public void LaunchLocalMultiplayer()
    {
        boardToLoad = BoardType.Default;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.CurrentGameMode = GameStateManager.GameMode.LocalMultiplayer;
        }

        SceneManager.LoadScene(mainGameSceneName);
    }

    public void LaunchOnlineMultiplayer()
    {
        Debug.Log("[GameSettingsBootstrapper] OnlineMultiplayer еще не реализован.");
    }

    public void LaunchTestGame()
    {
        boardToLoad = BoardType.Test;
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
