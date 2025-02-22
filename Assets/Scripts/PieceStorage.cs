using System.Collections.Generic;
using UnityEngine;

public class PieceStorage : MonoBehaviour
{
    public static PieceStorage Instance; // Синглтон

    public Dictionary<Vector3Int, GameObject> AllPieces { get; private set; } = new Dictionary<Vector3Int, GameObject>();

    [SerializeField]
    private List<GameObject> pieceObjects = new List<GameObject>(); // Список GameObject фигур для отображения в инспекторе

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Инициализация синглтона
        }
        else
        {
            Debug.LogError("Обнаружено несколько экземпляров PieceStorage! Убедитесь, что на сцене только один PieceStorage.");
            Destroy(gameObject);
        }
    }

    // Регистрация фигуры
    public void RegisterPiece(GameObject piece, Vector3Int position, CellData cellData)
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
        pieceObjects.Add(piece); // Добавляем GameObject в список для отображения в инспекторе
        cellData.IsOccupied = true;
        cellData.OccupyingPiece = piece;
        Debug.Log($"Фигура {piece.name} зарегистрирована на позиции {position}.");
    }

    // Удаление фигуры
    public void UnregisterPiece(Vector3Int position)
    {
        if (AllPieces.ContainsKey(position))
        {
            GameObject piece = AllPieces[position];
            pieceObjects.Remove(piece); // Удаляем GameObject из списка
            AllPieces.Remove(position);

            CellData cellData = CellStorage.Instance.GetCellData(position);
            if (cellData != null)
            {
                cellData.IsOccupied = false;
                cellData.OccupyingPiece = null;
            }

            Debug.Log($"Фигура удалена с позиции {position}.");
        }
    }

    // Получение фигуры по позиции
    public GameObject GetPieceAtPosition(Vector3Int position)
    {
        if (AllPieces.ContainsKey(position))
        {
            return AllPieces[position];
        }
        return null;
    }

    // Проверка, занята ли клетка
    public bool IsCellOccupied(Vector3Int position)
    {
        return AllPieces.ContainsKey(position);
    }
}
