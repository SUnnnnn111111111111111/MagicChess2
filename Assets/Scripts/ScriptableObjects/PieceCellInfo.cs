using UnityEngine;

[System.Serializable]
public class PieceCellInfo
{
    public PieceData PieceData; // Данные фигуры
    public Vector3Int PiecePosition; // Позиция фигуры
    public CellData CellData; // Данные клетки, на которой стоит фигура
}
