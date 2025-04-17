using UnityEngine;
using UnityEngine.SceneManagement;

public class PreGameBootstrapper : MonoBehaviour
{
    private LoadingUI loadingUI;
    [SerializeField] private string gameSettingsSceneName = "GameSettingsScene";

    private void Awake()
    {
        InitRepository("Prefabs/Repositories/TilesRepository");
        InitRepository("Prefabs/Repositories/FiguresRepository");

        InitManager("Prefabs/Managers/UIManager");
        InitManager("Prefabs/Managers/EventTriggeringTileManager");
        InitManager("Prefabs/Managers/PawnMovementPromotionManager");
        InitManager("Prefabs/Managers/SelectedFigureManager");
        InitManager("Prefabs/Managers/GameStateManager");
        InitManager("Prefabs/Managers/FogOfWarManager");
        InitManager("Prefabs/Managers/HighlightTilesManager");
        InitManager("Prefabs/Managers/InputManager");

        InitFactory("Prefabs/Factories/BoardFactory");
        InitFactory("Prefabs/Factories/UIFactory");
        InitFactory("Prefabs/Factories/DeathEffectFactory");
        
        
        SceneManager.LoadScene(gameSettingsSceneName);
    }

    private void InitRepository(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[Bootstrapper] Префаб не найден: {path}");
            return;
        }

        string name = prefab.name;

        if (GameObject.FindFirstObjectByType(prefab.GetComponent<MonoBehaviour>().GetType()) == null)
        {
            GameObject instance = Instantiate(prefab);
            instance.name = name;
            instance.SetActive(true);
        }
        else
        {
            Debug.Log($"[Bootstrapper] Репозиторий {name} уже существует.");
        }
    }

    private void InitManager(string path)
    {
        InitGeneric(path, "менеджер");
    }

    private void InitFactory(string path)
    {
        InitGeneric(path, "фабрика");
    }

    private void InitGeneric(string path, string typeName)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[Bootstrapper] Префаб не найден: {path}");
            return;
        }

        string name = prefab.name;

        if (GameObject.FindFirstObjectByType(prefab.GetComponent<MonoBehaviour>().GetType()) == null)
        {
            GameObject instance = Instantiate(prefab);
            instance.name = name;
            instance.SetActive(true);
        }
        else
        {
            Debug.Log($"[Bootstrapper] {typeName} {name} уже существует.");
        }
    }
}
