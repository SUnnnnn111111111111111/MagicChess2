using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardFactory : MonoBehaviour
{
    public static BoardFactory Instance { get; private set; }

    [SerializeField] private GameObject defaultBoardPrefab;
    [SerializeField] private GameObject customBoardPrefab;

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

    public void LoadDefaultBoard()
    {
        Instantiate(defaultBoardPrefab);
    }

    public void LoadCustomBoard()
    {
        Instantiate(customBoardPrefab);
    }
}