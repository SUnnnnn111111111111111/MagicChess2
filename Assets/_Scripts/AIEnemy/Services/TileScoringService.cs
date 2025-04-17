using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TileScoringService
{
    private const float immediateKingWeight = 1000f;
    private const float globalKingBonus = 50f;
    private const float eventTileBonus = 20f;
    private const float regionBonus = 20f;
    private const float enemyFigureBonus = 150f;
    private const float threatenedTilePenalty = 80f;
    
    private const bool debug = false;

    public static float EvaluateTile(Figure figure, Tile tile, Vector2 enemyKingPosition)
    {
        Vector2 currentPos = new Vector2(figure.CurrentTile.Position.x, figure.CurrentTile.Position.y);
        Vector2 tilePos = new Vector2(tile.Position.x, tile.Position.y);

        float distance = Vector2.Distance(currentPos, tilePos);
        float weight = 1f / (distance + 1f);

        if (enemyKingPosition != Vector2.zero)
        {
            float distToKing = Vector2.Distance(tilePos, enemyKingPosition);
            weight += globalKingBonus / (distToKing + 1f);
        }

        if (tile.IsSideEventTriggering && !figure.IsKing && figure.CountOfMovesIsOnEventTriggeringTile < 3)
        {
            weight += eventTileBonus;
        }

        if (figure.WhiteTeamAffiliation)
        {
            if (tile.Position.x >= -17 && tile.Position.x <= -3 &&
                tile.Position.y >= -9 && tile.Position.y <= 17)
            {
                weight += regionBonus;
            }
        }
        else
        {
            if (tile.Position.x >= 3 && tile.Position.x <= 17 &&
                tile.Position.y >= -17 && tile.Position.y <= 9)
            {
                weight += regionBonus;
            }
        }

        if (tile.OccupyingFigure != null &&
            tile.OccupyingFigure.IsKing == false &&
            tile.OccupyingFigure.WhiteTeamAffiliation != figure.WhiteTeamAffiliation)
        {
            weight += enemyFigureBonus;
        }

        // Наказание за опасную клетку
        if (TileThreatAnalyzer.IsTileUnderThreat(tile, figure.WhiteTeamAffiliation))
        {
            weight -= threatenedTilePenalty;
        }

        return weight;
    }

    public static (Tile tile, float weight) SelectBestTile(Figure figure)
    {
        List<Tile> availableTiles = FigureMoveService.GetAvailableToMoveTiles(figure);
        if (availableTiles == null || availableTiles.Count == 0)
            return (null, 0f);

        foreach (var tile in availableTiles)
        {
            if (tile.OccupyingFigure != null &&
                tile.OccupyingFigure.IsKing &&
                tile.OccupyingFigure.WhiteTeamAffiliation != figure.WhiteTeamAffiliation)
            {
                return (tile, immediateKingWeight);
            }
        }

        Vector2 enemyKingPos = Vector2.zero;
        Figure enemyKing = FiguresRepository.Instance
            .GetFiguresByTeam(!figure.WhiteTeamAffiliation)
            .FirstOrDefault(f => f.IsKing);
        if (enemyKing != null && enemyKing.CurrentTile != null)
        {
            enemyKingPos = new Vector2(enemyKing.CurrentTile.Position.x, enemyKing.CurrentTile.Position.y);
        }

        Tile bestTile = null;
        float bestWeight = float.MinValue;

        foreach (var tile in availableTiles)
        {
            float w = EvaluateTile(figure, tile, enemyKingPos);

            if (debug)
            {
                string log = $"[AI] {figure.name} → {tile.Position} = {w:F1}";
                if (tile.OccupyingFigure != null)
                {
                    log += $" (атакует: {tile.OccupyingFigure.name})";
                }

                if (TileThreatAnalyzer.IsTileUnderThreat(tile, figure.WhiteTeamAffiliation))
                {
                    log += " [⚠ под угрозой]";
                }

                Debug.Log(log);
            }

            if (w > bestWeight)
            {
                bestWeight = w;
                bestTile = tile;
            }
        }
        
        if (debug && bestTile != null)
        {
            Debug.Log($"[AI] 👉 {figure.name} выбрал {bestTile.Position} с весом {bestWeight:F1}");
        }
        
        return (bestTile, bestWeight);
    }
}
