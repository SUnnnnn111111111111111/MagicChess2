using UnityEngine;
using System.Collections.Generic;

public class PieceController : MonoBehaviour
{
    public PieceData PieceData; // Данные фигуры
    private PieceStorage pieceStorage; // Ссылка на хранилище фигур
    private CellStorage cellStorage; // Ссылка на хранилище клеток
    private Vector3 currentPosition; // Текущая позиция фигуры

    void Start()
    {
        pieceStorage = FindObjectOfType<PieceStorage>();
        cellStorage = FindObjectOfType<CellStorage>();

        if (pieceStorage == null || cellStorage == null)
        {
            Debug.LogError("Хранилище фигур или клеток не найдено на сцене!");
            return;
        }

        if (PieceData == null)
        {
            Debug.LogError("PieceData не назначен в инспекторе!");
            return;
        }

        currentPosition = transform.position; // Используем мировые координаты
        Debug.Log($"Фигура {PieceData.PieceName} пытается зарегистрироваться на позиции {currentPosition}.");

        CellData cellData = cellStorage.GetCellData(currentPosition);
        if (cellData == null)
        {
            Debug.LogError($"Клетка с позицией {currentPosition} не найдена в CellStorage!");
            return;
        }

        pieceStorage.RegisterPiece(gameObject, currentPosition, cellData);
    }

    void OnMouseDown()
    {
        List<Vector3> possibleMoves = PieceData.MovementRule.GetPossibleMoves(currentPosition);
        cellStorage.HighlightAvailableCells(possibleMoves);
    }
}
