using UnityEngine;
using System;
using System.Collections.Generic;

public class FigureHighlight : MonoBehaviour
{
    public event Action<Tile> OnHighlightCompleted; // Событие завершения подсветки

    public void HighlightAvailableMoves(Tile currentTile, NeighborSelectionSettings settings)
    {
        List<Tile> availableMoves = CalculateAvailableMoves(currentTile, settings);

        // Подсветка клеток (логика подсветки)
        Debug.Log($"✨ Фигура {gameObject.name} подсветила {availableMoves.Count} клеток.");

        // Пример вызова события (когда игрок выбирает клетку)
        if (availableMoves.Count > 0)
        {
            Tile selectedTile = availableMoves[0]; // Здесь должна быть логика выбора клетки игроком
            OnHighlightCompleted?.Invoke(selectedTile);
        }
    }

    private List<Tile> CalculateAvailableMoves(Tile currentTile, NeighborSelectionSettings settings)
    {
        // Логика расчета доступных ходов
        List<Tile> availableMoves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);

        foreach (var tile in possibleMoves)
        {
            if (!tile.IsOccupied)
            {
                availableMoves.Add(tile);
            }
        }

        return availableMoves;
    }
}