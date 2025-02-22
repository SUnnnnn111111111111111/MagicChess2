using UnityEngine;

public class CellInitializer : MonoBehaviour
{
    public CellStorage CellStorage; // Ссылка на хранилище клеток
    public float CellSize = 2.0f; // Размер клетки (шаг)

    void Start()
    {
        CellStorage.CellSize = CellSize; // Устанавливаем размер клетки
        InitializeCells();
    }

    void InitializeCells()
    {
        CellView[] cellViews = FindObjectsOfType<CellView>();

        foreach (CellView cellView in cellViews)
        {
            Vector3Int cellPosition = CalculateCellPosition(cellView.transform.position);
            Debug.Log($"Инициализация клетки на позиции {cellPosition}.");

            CellData cellData = new CellData
            {
                Position = cellPosition,
                IsOccupied = false,
                OccupyingPiece = null
            };

            cellView.Initialize(cellData);
            CellStorage.AddCell(cellPosition, cellData, cellView.gameObject);
        }

        ConnectCells();
    }

    Vector3Int CalculateCellPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / CellSize);
        int y = Mathf.RoundToInt(worldPosition.y / CellSize);
        int z = Mathf.RoundToInt(worldPosition.z / CellSize);
        return new Vector3Int(x, y, z);
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
            if (CellStorage.AllCells.ContainsKey(neighborPosition))
            {
                cellData.Neighbors.Add(CellStorage.AllCells[neighborPosition]);
            }
        }
    }
}
