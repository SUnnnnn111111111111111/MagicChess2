using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BoardFactory : MonoBehaviour
{
    public static BoardFactory Instance { get; private set; }

    [SerializeField] private GameObject defaultBoardPrefab;
    [SerializeField] private GameObject testBoardPrefab;

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

    public void LoadTestBoard()
    {
        Instantiate(testBoardPrefab);
    }
}