using System.Collections.Generic;
using UnityEngine;

public abstract class MoveCalculator
{
    // Абстрактный метод для вычисления доступных ходов
    public abstract List<Tile> CalculateMoves(Tile currentTile, NeighborSelectionSettings settings, bool isWhite);

    // Метод для вычисления направления между двумя клетками
    protected Vector2Int GetDirection(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        return new Vector2Int(
            diff.x == 0 ? 0 : diff.x / Mathf.Abs(diff.x),
            diff.y == 0 ? 0 : diff.y / Mathf.Abs(diff.y)
        );
    }
}