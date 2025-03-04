using UnityEngine;
using System.Collections.Generic;
using System;

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
            Debug.Log($"✅ Фигура {gameObject.name} зарегистрирована на клетке {currentTile.Position}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
    }

    public void HighlightAvailableMoves()
{
    if (currentTile == null)
    {
        Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
        return;
    }

    List<Tile> availableMoves = new List<Tile>();
    List<Tile> possibleMoves = currentTile.GetNeighbors(neighborSelectionSettings);

    Debug.Log($"🔍 Фигура {gameObject.name} нашла {possibleMoves.Count} возможных ходов.");

    // Отладочный лог для проверки возможных ходов
    foreach (var tile in possibleMoves)
    {
        Debug.Log($"🔍 Проверка клетки {tile.Position} для {gameObject.name}. Занята: {tile.IsOccupied}, Подсвечена: {tile.IsHighlighted}");
    }

    // Обработка ходов для пешки
    if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.WhitePawn || rule.neighborType == NeighborType.BlackPawn))
    {
        foreach (var tile in possibleMoves)
        {
            // Пешка может двигаться только на пустые клетки вперед
            if (!tile.IsOccupied)
            {
                availableMoves.Add(tile);
                Debug.Log($"✅ Клетка {tile.Position} добавлена в список доступных ходов для пешки.");
            }
        }
    }
    // Обработка ходов для коня
    else if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.KnightMove))
    {
        foreach (var tile in possibleMoves)
        {
            // Конь может ходить на любую клетку, если она не занята
            if (!tile.IsOccupied)
            {
                availableMoves.Add(tile);
                Debug.Log($"✅ Клетка {tile.Position} добавлена в список доступных ходов для коня.");
            }
        }
    }
    // Обработка ходов для других фигур
    else
    {
        Dictionary<Vector2Int, List<Tile>> directionalMoves = new Dictionary<Vector2Int, List<Tile>>();

        // Группируем клетки по направлениям
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

        // Проверяем клетки в каждом направлении
        foreach (var entry in directionalMoves)
        {
            bool foundObstacle = false;
            foreach (var tile in entry.Value)
            {
                if (foundObstacle)
                {
                    Debug.Log($"🚧 Преграда найдена, клетка {tile.Position} больше не проверяется.");
                    break;
                }

                if (tile.IsOccupied)
                {
                    foundObstacle = true;
                    Debug.Log($"🚧 Клетка {tile.Position} занята другой фигурой.");
                }
                else
                {
                    Debug.Log($"🔎 Проверка клетки {tile.Position} для {gameObject.name}. Занята: {tile.IsOccupied}, Подсвечена: {tile.IsHighlighted}");
                    availableMoves.Add(tile);
                    Debug.Log($"✅ Клетка {tile.Position} добавлена в список доступных ходов.");
                }
            }
        }
    }

    Debug.Log($"✨ Фигура {gameObject.name} подсветила {availableMoves.Count} клеток.");
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
        if (targetTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться: цель null!");
            return;
        }
        
        if (targetTile.IsOccupied)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться: клетка {targetTile.Position} занята!");
            return;
        }

        if (!targetTile.IsHighlighted)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться: клетка {targetTile.Position} не подсвечена!");
            return;
        }

        Debug.Log($"🔄 Фигура {gameObject.name} перемещается на клетку {targetTile.Position}.");

        currentTile.SetOccupied(false);
        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        currentTile.SetOccupied(true);

        Debug.Log($"✅ Фигура {gameObject.name} завершила перемещение.");

        HighlightController.Instance.ClearHighlights();
        GameManager.Instance.SelectedFigure = null;
    }
}
