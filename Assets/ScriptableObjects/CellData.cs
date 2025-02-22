using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellData
{
    public Vector3Int Position; // Позиция клетки
    public bool IsOccupied; // Занята ли клетка
    public GameObject OccupyingPiece; // Фигура, которая занимает клетку
    public List<CellData> Neighbors = new List<CellData>(); // Список соседей
}
