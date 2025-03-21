using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PreGameBootstrapper : MonoBehaviour
{
    [SerializeField] private string gameSettingsSceneName = "GameSettingsScene";

    private void Awake()
    {
        
        if (TilesRepository.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/TilesRepository"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] TilesRepository уже существует.");
        }

        if (SelectedFigureManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/SelectedFigureManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] SelectedFigureManager уже существует.");
        }

        if (GameStateManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/GameStateManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] GameStateManager уже существует.");
        }

        if (FogOfWarManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/FogOfWarManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] FogOfWarManager уже существует.");
        }

        if (FiguresRepository.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/FiguresRepository"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] FiguresRepository уже существует.");
        }

        if (HighlightTilesManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/HighlightTilesManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] HighlightTilesManager уже существует.");
        }
        SceneManager.LoadScene(gameSettingsSceneName);
    }
}
