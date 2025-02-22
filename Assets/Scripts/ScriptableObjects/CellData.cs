using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellData
{
    public Vector3 Position; // Позиция клетки в мировых координатах
    public bool IsOccupied => OccupiedBy != null; // Занята ли клетка
    public GameObject OccupiedBy; // Фигура, которая занимает клетку

    [System.NonSerialized] // Это поле не будет сериализоваться
    public List<CellData> Neighbors = new List<CellData>(); // Список соседей
}
