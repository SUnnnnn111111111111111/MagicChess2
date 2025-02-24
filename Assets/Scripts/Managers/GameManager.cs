using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Figure selectedFigure; // Видно в инспекторе

    public Figure SelectedFigure
    {
        get { return selectedFigure; }
        set
        {
            selectedFigure = value;
            // Debug.Log(selectedFigure != null ? $"✅ Выбрана фигура: {selectedFigure.name}" : "⚠️ Фигура снята с выбора.");
        }
    }

    private void Awake()
    {
        Instance = this;
    }
}
