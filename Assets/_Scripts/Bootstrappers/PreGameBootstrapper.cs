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
            Instantiate(Resources.Load("Prefabs/Repositories/TilesRepository"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] TilesRepository уже существует.");
        }
        
        if (BoardFactory.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/BoardFactory"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] BoardFactory уже существует.");
        }

        if (FiguresRepository.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Repositories/FiguresRepository"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] FiguresRepository уже существует.");
        }
        
        if (EventTriggeringTileManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Managers/EventTriggeringTileManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] EventTriggeringTileManager уже существует.");
        }
        
        if (PawnMovementPromotionManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Managers/PawnMovementPromotionManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] PawnMovementPromotionManager уже существует.");
        }

        if (SelectedFigureManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Managers/SelectedFigureManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] SelectedFigureManager уже существует.");
        }

        if (GameStateManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Managers/GameStateManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] GameStateManager уже существует.");
        }

        if (FogOfWarManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Managers/FogOfWarManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] FogOfWarManager уже существует.");
        }

        if (HighlightTilesManager.Instance == null)
        {
            Instantiate(Resources.Load("Prefabs/Managers/HighlightTilesManager"));
        }
        else
        {
            Debug.Log("[PreGameBootstrapper] HighlightTilesManager уже существует.");
        }
        
        
        SceneManager.LoadScene(gameSettingsSceneName);
    }
}
