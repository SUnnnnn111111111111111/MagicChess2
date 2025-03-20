using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsBootstrapper : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "MainGameScene";
    
    
    public void LaunchSettingsUI()
    {
        Debug.Log("[GameSettingsBootstrapper] SettingsUI режим еще не реализован.");
    }
    
    public void LaunchGameWithWhiteEnemy()
    {
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
        
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.WhitesPlaying);
        }
        else
        {
            Debug.LogWarning("[GameSettingsBootstrapper] GameStateManager не найден!");
        }
        
        Debug.Log("[GameSettingsBootstrapper] Игрок выбрал играть с компьютером за белых. Загружаем сцену: " + mainGameSceneName);
        SceneManager.LoadScene(mainGameSceneName);
    }
    
    public void LaunchGameWithBlackEnemy()
    {
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
        
        Debug.Log("[GameSettingsBootstrapper] Игрок выбрал играть с компьютером за черных. Загружаем сцену: " + mainGameSceneName);
        SceneManager.LoadScene(mainGameSceneName);
    }
    
    public void LaunchLocalMultiplayer()
    {
        Debug.Log("[GameSettingsBootstrapper] Игрок выбрал локальный мультиплеер. Загружаем сцену: " + mainGameSceneName);
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
