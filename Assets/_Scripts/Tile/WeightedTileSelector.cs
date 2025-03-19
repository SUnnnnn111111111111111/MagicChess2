using System.Collections.Generic;
using UnityEngine;

public class WeightedTileSelector
{
    // Центральные клетки, заданные явно
    private static readonly Vector2[] centralCells = new Vector2[]
    {
        new Vector2(1, 1),
        new Vector2(-1, 1),
        new Vector2(-1, -1),
        new Vector2(1, -1)
    };

    // Фиксированный вес для вражеских клеток – значение выбрано достаточно высоким, чтобы доминировать над весами пустых клеток.
    private const float enemyWeight = 1000f;

    /// <summary>
    /// Выбирает клетку из списка доступных, используя взвешенный случайный выбор.
    /// Если клетка содержит вражескую фигуру, её вес равен enemyWeight.
    /// Для пустых клеток вес определяется как 1 / (минимальное расстояние до центральной клетки + 1).
    /// </summary>
    public static Tile SelectTile(List<Tile> availableTiles)
    {
        if (availableTiles == null || availableTiles.Count == 0)
            return null;

        float totalWeight = 0f;
        List<float> weights = new List<float>();

        foreach (Tile tile in availableTiles)
        {
            float weight;
            // Если на клетке стоит фигура, значит она вражеская (так как союзные исключены)
            if (tile.OccupyingFigure != null)
            {
                weight = enemyWeight;
                Debug.Log($"Tile at {tile.Position} содержит вражескую фигуру, назначенный вес: {enemyWeight}");

            }
            else
            {
                // Вычисляем минимальное расстояние от клетки до одной из центральных точек
                Vector2 tilePos = new Vector2(tile.Position.x, tile.Position.y);
                float minDistance = float.MaxValue;
                foreach (Vector2 center in centralCells)
                {
                    float distance = Vector2.Distance(tilePos, center);
                    if (distance < minDistance)
                        minDistance = distance;
                }
                weight = 1f / (minDistance + 1f);
                Debug.Log($"Tile at {tile.Position} пуста. Минимальное расстояние до центра: {minDistance:F2}, вес: {weight:F2}");

            }
            weights.Add(weight);
            totalWeight += weight;
        }

        // Выбираем клетку, основываясь на распределении весов
        float randomValue = Random.value * totalWeight;
        for (int i = 0; i < availableTiles.Count; i++)
        {
            randomValue -= weights[i];
            if (randomValue <= 0f)
                return availableTiles[i];
        }

        // На всякий случай возвращаем последнюю клетку
        return availableTiles[availableTiles.Count - 1];
    }
}
