using UnityEngine;
using System.Collections;

public class LoadingUIController : MonoBehaviour
{
    public static LoadingUIController Instance { get; private set; }

    [SerializeField] private string loadingUIPrefabPath = "Prefabs/UI/LoadingUI";
    private LoadingUI loadingUIInstance;

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

    public void Show()
    {
        if (loadingUIInstance != null) return;

        GameObject prefab = Resources.Load<GameObject>(loadingUIPrefabPath);
        if (prefab == null)
        {
            Debug.LogError("LoadingUI префаб не найден!");
            return;
        }

        GameObject obj = Instantiate(prefab);
        loadingUIInstance = obj.GetComponent<LoadingUI>();
        DontDestroyOnLoad(obj);
    }

    public void SetProgress(float progress)
    {
        loadingUIInstance?.SetProgress(progress);
    }

    public void Hide()
    {
        if (loadingUIInstance != null)
        {
            StartCoroutine(HideRoutine());
        }
    }

    private IEnumerator HideRoutine()
    {
        yield return loadingUIInstance.FadeOutAndDestroy();
        loadingUIInstance = null;
    }
}