using System.Collections.Generic;
using UnityEngine;

public class WeightedTileSelector
{
    // Константы бонусов
    private const float immediateKingWeight = 1000f;  // Если в доступных ходах есть клетка с королём – выбираем её немедленно
    private const float globalKingBonus = 2f;         // Бонус за приближение к вражескому королю (не слишком высокий)
    private const float eventTileBonus = 50f;          // Бонус для event-триггерной клетки, если фигура ещё не простаивала 3 хода
    private const float regionBonus = 1.5f;             // Дополнительный бонус для целевого региона
    private const float enemyFigureBonus = 100f;        // Дополнительный бонус для клетки, на которой стоит вражеская фигура

    /// <summary>
    /// Выбирает из доступных для передвижения клеток ту, у которой максимальный вес,
    /// и возвращает кортеж с выбранной клеткой и рассчитанным для неё весом.
    /// Если среди доступных клеток есть клетка с вражеским королём, она выбирается немедленно.
    /// </summary>
    public static (Tile tile, float weight) SelectGlobalTargetTileWithWeight(Figure figure)
    {
        // Получаем список доступных для перемещения клеток
        List<Tile> availableTiles = figure.GetAvailableToMoveTiles();
        if (availableTiles == null || availableTiles.Count == 0)
        {
            Debug.LogWarning("Нет доступных клеток для перемещения фигуры");
            return (null, 0f);
        }
        
        // Если среди доступных клеток присутствует клетка с вражеским королём, возвращаем её сразу с максимальным весом
        foreach (Tile tile in availableTiles)
        {
            if (tile.OccupyingFigure != null &&
                tile.OccupyingFigure.isKing &&
                tile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation)
            {
                return (tile, immediateKingWeight);
            }
        }
        
        // Определяем позицию вражеского короля (если найден)
        Vector2 enemyKingPosition = Vector2.zero;
        bool enemyKingFound = false;
        List<Figure> enemyFigures = figure.whiteTeamAffiliation 
                                        ? FiguresRepository.Instance.GetBlackFigures() 
                                        : FiguresRepository.Instance.GetWhiteFigures();
        foreach (Figure enemy in enemyFigures)
        {
            if (enemy.isKing)
            {
                enemyKingPosition = new Vector2(enemy.CurrentTile.Position.x, enemy.CurrentTile.Position.y);
                enemyKingFound = true;
                break;
            }
        }
        
        // Текущая позиция фигуры для расчёта базового веса
        Vector2 currentPos = new Vector2(figure.CurrentTile.Position.x, figure.CurrentTile.Position.y);
        
        Tile bestTile = null;
        float bestWeight = float.MinValue;
        
        foreach (Tile tile in availableTiles)
        {
            Vector2 tilePos = new Vector2(tile.Position.x, tile.Position.y);
            // Базовый вес: чем ближе клетка – тем выше значение
            float distance = Vector2.Distance(currentPos, tilePos);
            float weight = 1f / (distance + 1f);
            
            // Бонус за приближение к вражескому королю
            if (enemyKingFound)
            {
                float distToKing = Vector2.Distance(tilePos, enemyKingPosition);
                weight += globalKingBonus / (distToKing + 1f);
            }
            
            // Бонус за event-триггерную клетку (если фигура не король и не простаивала 3 хода)
            if (tile.isSideEventTriggering && !figure.isKing && figure.countOfMovesIsOnEventTriggeringTile < 3)
            {
                weight += eventTileBonus;
            }
            
            // Бонус за попадание в целевой регион в зависимости от команды AI
            if (figure.whiteTeamAffiliation)
            {
                // Для белых: целевой регион, например, x от -17 до -3, y от -9 до 17
                if (tile.Position.x >= -17 && tile.Position.x <= -3 &&
                    tile.Position.y >= -9 && tile.Position.y <= 17)
                {
                    weight += regionBonus;
                }
            }
            else
            {
                // Для чёрных: целевой регион, например, x от 3 до 17, y от -17 до 9
                if (tile.Position.x >= 3 && tile.Position.x <= 17 &&
                    tile.Position.y >= -17 && tile.Position.y <= 9)
                {
                    weight += regionBonus;
                }
            }
            
            // Бонус за наличие вражеской фигуры (не короля)
            if (tile.OccupyingFigure != null &&
                !tile.OccupyingFigure.isKing &&
                tile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation)
            {
                weight += enemyFigureBonus;
            }
            
            // Запоминаем клетку с максимальным весом
            if (weight > bestWeight)
            {
                bestWeight = weight;
                bestTile = tile;
            }
        }
        
        return (bestTile, bestWeight);
    }
}
