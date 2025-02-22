using UnityEngine;

public class CellInitializer : MonoBehaviour
{
    public CellStorage CellStorage;

    void Start()
    {
        InitializeCells();
    }

    void InitializeCells()
    {
        CellView[] cellViews = FindObjectsOfType<CellView>();

        foreach (CellView cellView in cellViews)
        {
            Vector3Int cellPosition = Vector3Int.RoundToInt(cellView.transform.position);
            Debug.Log($"Инициализация клетки на позиции {cellPosition}.");

            CellData cellData = new CellData
            {
                Position = cellPosition
            };

            cellView.Initialize(cellData);
            CellStorage.AddCell(cellPosition, cellData, cellView.gameObject);
        }

        ConnectCells();
    }

    void ConnectCells()
    {
        foreach (var cellData in CellStorage.AllCells.Values)
        {
            AddNeighbors(cellData);
        }
    }

    void AddNeighbors(CellData cellData)
    {
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0), // Вправо
            new Vector3Int(-1, 0, 0), // Влево
            new Vector3Int(0, 0, 1), // Вперёд
            new Vector3Int(0, 0, -1) // Назад
        };

        foreach (var offset in neighborOffsets)
        {
            Vector3Int neighborPosition = cellData.Position + offset;
            CellData neighbor = CellStorage.GetCellData(neighborPosition);
            if (neighbor != null)
            {
                cellData.Neighbors.Add(neighbor);
            }
        }
    }
}
