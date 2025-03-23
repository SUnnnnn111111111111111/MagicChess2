using System.Collections.Generic;
using UnityEngine;

public class WeightedFigureSelector
{
    // Константы для оценки ходов
    private const float kingBonus = 1000f;           // Максимальный бонус для хода, когда цель – король
    private const float enemyNonKingModifier = 2f;   // Коэффициент для пониженного веса при наличии обычного врага

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
            List<Tile> moveTiles = fig.GetAvailableToMoveTiles();
            float figureWeight = 0f;

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

                if (tile.OccupyingFigure != null)
                {
                    if (tile.OccupyingFigure.isKing)
                    {
                        // Если на клетке стоит король, добавляем максимальный бонус
                        figureWeight += kingBonus;
                    }
                    else
                    {
                        // Если на клетке стоит обычный враг, учитываем пониженный вес
                        figureWeight += (1f / (minDistance + 1f)) * enemyNonKingModifier;
                    }
                }
                else
                {
                    figureWeight += 1f / (minDistance + 1f);
                }
            }

            weights.Add(figureWeight);
            totalWeight += figureWeight;
        }

        float randomValue = Random.value * totalWeight;
        for (int i = 0; i < availableFigures.Count; i++)
        {
            randomValue -= weights[i];
            if (randomValue <= 0f)
                return availableFigures[i];
        }
        return availableFigures[availableFigures.Count - 1];
    }
}
