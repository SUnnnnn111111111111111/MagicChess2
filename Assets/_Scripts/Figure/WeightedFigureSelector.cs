using System.Collections.Generic;
using UnityEngine;

public class WeightedFigureSelector
{
    // Фиксированный вес для фигур, у которых есть вражеская клетка в доступных ходах.
    private const float maxWeight = 1000f;

    /// <summary>
    /// Выбирает фигуру из списка availableFigures, оценивая её вес по доступным ходам.
    /// Если хотя бы у одной фигуры в доступных ходах присутствует вражеская клетка, ей присваивается максимальный вес.
    /// Для остальных фигур вес вычисляется как сумма весов клеток (1 / (minDistance до центральной точки + 1)).
    /// </summary>
    public static Figure SelectFigure(List<Figure> availableFigures)
    {
        if (availableFigures == null || availableFigures.Count == 0)
            return null;

        float totalWeight = 0f;
        List<float> weights = new List<float>();

        // Центральные точки, заданные явно.
        Vector2[] centralCells = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(-1, -1),
            new Vector2(1, -1)
        };

        foreach (Figure fig in availableFigures)
        {
            List<Tile> moveTiles = fig.GetAvailableMoveTiles();
            bool hasEnemyTile = false;
            float figureWeight = 0f;

            // Проходим по всем доступным клеткам фигуры
            foreach (Tile tile in moveTiles)
            {
                // В условии GetAvailableMoveTiles уже возвращаются либо пустые, либо клетки с врагами.
                if (tile.OccupyingFigure != null)
                {
                    hasEnemyTile = true;
                    break;
                }
            }

            if (hasEnemyTile)
            {
                // Если обнаружена вражеская клетка, присваиваем максимальный вес фигуре.
                figureWeight = maxWeight;
            }
            else
            {
                // Иначе вычисляем вес как сумму весов пустых клеток.
                foreach (Tile tile in moveTiles)
                {
                    Vector2 tilePos = new Vector2(tile.Position.x, tile.Position.y);
                    float minDistance = float.MaxValue;
                    foreach (Vector2 center in centralCells)
                    {
                        float distance = Vector2.Distance(tilePos, center);
                        if (distance < minDistance)
                            minDistance = distance;
                    }
                    float tileWeight = 1f / (minDistance + 1f);
                    figureWeight += tileWeight;
                }
            }

            weights.Add(figureWeight);
            totalWeight += figureWeight;
        }

        // Выполняем взвешенный случайный выбор фигуры
        float randomValue = Random.value * totalWeight;
        for (int i = 0; i < availableFigures.Count; i++)
        {
            randomValue -= weights[i];
            if (randomValue <= 0f)
                return availableFigures[i];
        }

        // На всякий случай возвращаем последнюю фигуру
        return availableFigures[availableFigures.Count - 1];
    }
}
