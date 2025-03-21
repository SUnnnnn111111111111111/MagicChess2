using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsBootstrapper : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "MainGameScene";
    
    public void LaunchSettingsUI()
    {
        Debug.Log("[GameSettingsBootstrapper] SettingsUI режим еще не реализован.");
    }
    
    public void LaunchGameWithBlackEnemy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.CurrentGameMode = GameStateManager.GameMode.VsAiEnemy;
            GameStateManager.Instance.humanPlaysWhite = true;
        }
        
        if (AIEnemy.Instance == null)
        {
            GameObject aiObj = Instantiate(Resources.Load("Prefabs/AIEnemy")) as GameObject;
            AIEnemy ai = aiObj.GetComponent<AIEnemy>();
            if (ai != null)
            {
                ai.SetTeam(AIEnemy.AITeam.Black);
            }
        }
        else
        {
            Debug.Log("[GameSettingsBootstrapper] AIEnemy уже существует.");
        }
        SceneManager.LoadScene(mainGameSceneName);
    }
    
    public void LaunchGameWithWhiteEnemy()
    {
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
            GameObject aiObj = Instantiate(Resources.Load("Prefabs/AIEnemy")) as GameObject;
            AIEnemy ai = aiObj.GetComponent<AIEnemy>();
            if (ai != null)
            {
                ai.SetTeam(AIEnemy.AITeam.White);
            }
        }
        else
        {
            Debug.Log("[GameSettingsBootstrapper] AIEnemy уже существует.");
        }
        GameStateManager.Instance.SetGameState(GameStateManager.GameState.WhitesPlaying);
        
        SceneManager.LoadScene(mainGameSceneName);
    }
    
    public void LaunchLocalMultiplayer()
    {
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
    
    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
