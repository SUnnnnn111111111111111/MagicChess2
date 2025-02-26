using UnityEngine;
using System.Collections.Generic;


public class Figure : MonoBehaviour
{
    public NeighborSelectionSettings neighborSelectionSettings;
    private Tile currentTile;

    private void Start()
    {
        // Даем время BoardManager зарегистрировать все клетки перед поиском
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        currentTile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (currentTile != null)
        {
            currentTile.SetOccupied(true);
        }
        else
        {
            // Debug.Log($"✅ Фигура {gameObject.name} стоит на клетке {currentTile.Position}");
        }
    }

    public void HighlightAvailableMoves()
    {
        if (currentTile == null) return;

        List<Tile> availableMoves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(neighborSelectionSettings);

        // Группируем клетки по направлению (по осям X, Z и диагоналям)
        Dictionary<Vector2Int, List<Tile>> directionalMoves = new Dictionary<Vector2Int, List<Tile>>();

        foreach (var offset in neighborSelectionSettings.GetOffsets())
        {
            directionalMoves[offset] = new List<Tile>();
        }

        foreach (var tile in possibleMoves)
        {
            Vector2Int direction = GetDirection(tile.Position, currentTile.Position);
            if (directionalMoves.ContainsKey(direction))
            {
                directionalMoves[direction].Add(tile);
            }
        }

        // Проверяем каждое направление на наличие преграды
        foreach (var entry in directionalMoves)
        {
            bool foundObstacle = false;
            foreach (var tile in entry.Value)
            {
                if (foundObstacle) break; // Если нашли препятствие, дальше не проверяем

                if (tile.IsOccupied)
                {
                    foundObstacle = true; // Преграда найдена, дальше клетки не подсвечиваем
                }
                else
                {
                    availableMoves.Add(tile);
                }
            }
        }

        HighlightController.Instance.HighlightTiles(availableMoves);
    }

    private Vector2Int GetDirection(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        return new Vector2Int(
            diff.x == 0 ? 0 : diff.x / Mathf.Abs(diff.x), 
            diff.y == 0 ? 0 : diff.y / Mathf.Abs(diff.y)
        );
    }



    public void MoveToTile(Tile targetTile)
    {
        if (targetTile == null || targetTile.IsOccupied)
        {
            // Запрет перемещения на занятые клетки
            return;
        }

        if (!targetTile.IsHighlighted)
        {
            return;
        }

        // Освобождаем текущую клетку
        currentTile.SetOccupied(false);

        // Двигаем фигуру
        transform.position = targetTile.transform.position;
        currentTile = targetTile;

        // Занимаем новую клетку
        currentTile.SetOccupied(true);

        HighlightController.Instance.ClearHighlights();
        GameManager.Instance.SelectedFigure = null;
    }

}
