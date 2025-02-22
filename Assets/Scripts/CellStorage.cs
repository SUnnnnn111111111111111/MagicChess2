using System.Collections.Generic;
using UnityEngine;

public class CellStorage : MonoBehaviour
{
    public Dictionary<Vector3Int, CellData> AllCells { get; private set; } = new Dictionary<Vector3Int, CellData>();

    [SerializeField]
    private List<GameObject> cellObjects = new List<GameObject>();

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
        cellObjects.Add(cellObject);
        Debug.Log($"Клетка добавлена на позицию {position}.");
    }

    public CellData GetCellData(Vector3Int position)
    {
        if (AllCells.TryGetValue(position, out CellData cellData))
        {
            return cellData;
        }

        Debug.LogWarning($"Клетка с позицией {position} не найдена.");
        return null;
    }
}
