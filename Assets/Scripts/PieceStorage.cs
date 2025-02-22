using System.Collections.Generic;
using UnityEngine;

public class PieceStorage : MonoBehaviour
{
    public Dictionary<Vector3, GameObject> AllPieces { get; private set; } = new Dictionary<Vector3, GameObject>();

    [SerializeField]
    private List<GameObject> pieceObjects = new List<GameObject>(); // Список GameObject фигур

    // Регистрация фигуры
    public void RegisterPiece(GameObject piece, Vector3 position, CellData cellData)
    {
        if (piece == null)
        {
            Debug.LogError("Фигура не может быть null!");
            return;
        }

        if (cellData == null)
        {
            Debug.LogError("CellData не может быть null!");
            return;
        }

        if (AllPieces.ContainsKey(position))
        {
            Debug.LogWarning($"Фигура уже зарегистрирована на позиции {position}!");
            return;
        }

        AllPieces[position] = piece;
        pieceObjects.Add(piece);
        cellData.OccupiedBy = piece;
        Debug.Log($"Фигура {piece.name} зарегистрирована на позиции {position}.");
    }

    // Удаление фигуры
    public void UnregisterPiece(Vector3 position)
    {
        if (AllPieces.ContainsKey(position))
        {
            GameObject piece = AllPieces[position];
            pieceObjects.Remove(piece);
            AllPieces.Remove(position);
            Debug.Log($"Фигура удалена с позиции {position}.");
        }
    }

    // Получение фигуры по позиции
    public GameObject GetPieceAtPosition(Vector3 position)
    {
        if (AllPieces.ContainsKey(position))
        {
            return AllPieces[position];
        }
        return null;
    }
}