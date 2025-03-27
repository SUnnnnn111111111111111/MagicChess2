using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class KingThreatAnalyzer
{
    public class Result
    {
        public bool isUnderAttack;
        public bool isDirect;
        public Figure king;
        public List<Figure> attackers = new();
        public List<Tile> blockableTiles = new();
        public List<Figure> coveringPieces = new();
    }

    public static Result Analyze(Figure king, List<Figure> allEnemies, List<Figure> allAllies)
    {
        Result result = new() { king = king };
        Vector2Int kingPos = king.CurrentTile.Position;

        foreach (var enemy in allEnemies)
        {
            if (enemy.CurrentTile == null) continue;

            List<Tile> moves = FigureMoveService.GetAvailableToMoveTiles(enemy);
            bool canAttack = moves.Any(t => t.Position == kingPos && !t.HiddenByFog);

            if (!canAttack) continue;

            result.attackers.Add(enemy);

            if (FigureTypeHelper.IsShortRange(enemy))
            {
                result.blockableTiles.Add(king.CurrentTile);
            }
            else
            {
                var between = CalculateIntermediateTiles(enemy.CurrentTile, king.CurrentTile);
                result.blockableTiles.AddRange(between);
                result.blockableTiles.Add(king.CurrentTile);
            }
        }

        result.blockableTiles = result.blockableTiles.Distinct().ToList();

        foreach (var ally in allAllies)
        {
            if (ally.CurrentTile == null) continue;

            List<Tile> allyMoves = FigureMoveService.GetAvailableToMoveTiles(ally);
            if (allyMoves.Any(t => result.blockableTiles.Any(b => b.Position == t.Position)))
                result.coveringPieces.Add(ally);
        }

        result.isUnderAttack = result.attackers.Count > 0;
        result.isDirect = result.isUnderAttack && result.coveringPieces.Count == 0;

        return result;
    }

    private static List<Tile> CalculateIntermediateTiles(Tile from, Tile to)
    {
        List<Tile> intermediate = new();
        Vector2Int start = from.Position;
        Vector2Int end = to.Position;

        int dx = end.x - start.x;
        int dy = end.y - start.y;

        int stepX = dx == 0 ? 0 : dx / Mathf.Abs(dx);
        int stepY = dy == 0 ? 0 : dy / Mathf.Abs(dy);

        if (!(stepX == 0 || stepY == 0 || Mathf.Abs(dx) == Mathf.Abs(dy)))
            return intermediate;

        Vector2Int pos = start + new Vector2Int(stepX, stepY);
        while (pos != end)
        {
            Tile t = TilesRepository.Instance.GetTileAt(pos);
            if (t != null) intermediate.Add(t);
            pos += new Vector2Int(stepX, stepY);
        }

        return intermediate;
    }
} 