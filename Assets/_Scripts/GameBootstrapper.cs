using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private string mainGameSceneName = "MainGameScene";

    private void Awake()
    {
        Debug.Log("[GameBootstrapper] Запуск Bootstrapper-а...");
        
        if (TilesRepository.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/TilesRepository"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] TilesRepository уже существует.");
        }

        if (SelectedFigureManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/SelectedFigureManager"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] SelectedFigureManager уже существует.");
        }

        if (GameStateManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/GameStateManager"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] GameStateManager уже существует.");
        }

        if (FogOfWarManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/FogOfWarManager"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] FogOfWarManager уже существует.");
        }

        if (FiguresRepository.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/FiguresRepository"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] FiguresRepository уже существует.");
        }

        if (HighlightTilesManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/HighlightTilesManager"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] HighlightTilesManager уже существует.");
        }

        if (AIEnemy.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/AIEnemy"));
        }
        else
        {
            Debug.Log("[GameBootstrapper] AIEnemy уже существует.");
        }
        
        Debug.Log("[GameBootstrapper] Все менеджеры и репозитории созданы. Загружаем основную сцену: " + mainGameSceneName);
        
        SceneManager.LoadScene(mainGameSceneName);
    }
}
