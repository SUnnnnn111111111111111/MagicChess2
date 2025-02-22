using System.Collections.Generic;
using UnityEngine;

public class CellStorage : MonoBehaviour
{
    public static CellStorage Instance; // Синглтон

    public Dictionary<Vector3Int, CellData> AllCells { get; private set; } = new Dictionary<Vector3Int, CellData>();

    [SerializeField]
    private List<GameObject> cellObjects = new List<GameObject>(); // Список GameObject клеток для отображения в инспекторе

    public float CellSize = 2.0f; // Размер клетки (шаг)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Инициализация синглтона
        }
        else
        {
            Debug.LogError("Обнаружено несколько экземпляров CellStorage! Убедитесь, что на сцене только один CellStorage.");
            Destroy(gameObject);
        }
    }

    // Добавление клетки
    public void AddCell(Vector3Int position, CellData cellData, GameObject cellObject)
    {
        if (cellData == null)
        {
            Debug.LogError("CellData не может быть null!");
            return;
        }

        if (AllCells.ContainsKey(position))
        {
            Debug.LogWarning($"Клетка уже зарегистрирована на позиции {position}!");
            return;
        }

        AllCells[position] = cellData;
        cellObjects.Add(cellObject); // Добавляем GameObject в список для отображения в инспекторе
        Debug.Log($"Клетка добавлена на позицию {position}.");
    }

    // Получение данных о клетке
    public CellData GetCellData(Vector3Int position)
    {
        if (AllCells.ContainsKey(position))
        {
            return AllCells[position];
        }
        return null;
    }

    // Подсветка доступных клеток
    public void HighlightAvailableCells(List<Vector3Int> positions)
    {
        foreach (var cellData in AllCells.Values)
        {
            CellView cellView = FindCellView(cellData.Position);
            if (cellView != null)
            {
                cellView.Highlight(positions.Contains(cellData.Position));
            }
        }
    }

    // Поиск CellView по позиции
    CellView FindCellView(Vector3Int position)
    {
        foreach (var cellObject in cellObjects)
        {
            CellView cellView = cellObject.GetComponent<CellView>();
            if (cellView != null && cellView.CellData.Position == position)
            {
                return cellView;
            }
        }
        return null;
    }
}
