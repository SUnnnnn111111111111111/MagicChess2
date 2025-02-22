using UnityEngine;
using System.Collections.Generic;

public class PieceController : MonoBehaviour
{
    public PieceData PieceData; // Данные фигуры
    private Vector3Int currentPosition; // Текущая позиция фигуры

    void Start()
    {
        if (PieceData == null)
        {
            Debug.LogError("PieceData не назначен в инспекторе!");
            return;
        }

        currentPosition = CalculatePiecePosition();
        Debug.Log($"Фигура {PieceData.PieceName} пытается зарегистрироваться на позиции {currentPosition}.");

        CellData cellData = CellStorage.Instance.GetCellData(currentPosition);
        if (cellData == null)
        {
            Debug.LogError($"Клетка с позицией {currentPosition} не найдена в CellStorage!");
            return;
        }

        PieceStorage.Instance.RegisterPiece(gameObject, currentPosition, cellData);
    }

    Vector3Int CalculatePiecePosition()
    {
        Vector3 worldPosition = transform.position;
        int x = Mathf.RoundToInt(worldPosition.x / CellStorage.Instance.CellSize);
        int y = Mathf.RoundToInt(worldPosition.y / CellStorage.Instance.CellSize);
        int z = Mathf.RoundToInt(worldPosition.z / CellStorage.Instance.CellSize);
        Debug.Log($"Мировые координаты фигуры: {worldPosition}, вычисленные координаты: ({x}, {y}, {z})");
        return new Vector3Int(x, y, z);
    }

    void OnMouseDown()
    {
        List<Vector3Int> possibleMoves = PieceData.MovementRule.GetPossibleMoves(currentPosition);
        CellStorage.Instance.HighlightAvailableCells(possibleMoves);
    }
}
