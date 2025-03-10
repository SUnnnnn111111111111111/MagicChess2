using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Figure selectedFigure; 

    public Figure SelectedFigure
    {
        get { return selectedFigure; }
        set
        {
            selectedFigure = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
}
