using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFactory : MonoBehaviour
{
    public static UIFactory Instance { get; private set; }

    public GameStateUIController gameStateUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGameStateUI()
    {
        GameObject uiObj = Instantiate(gameStateUI.gameObject);
        gameStateUI = uiObj.GetComponent<GameStateUIController>();
    }
}