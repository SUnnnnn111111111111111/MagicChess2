using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FigureScoringService
{
    private const float kingBonus = 10000f;
    private const float coverKingBonus = 150f;
    private const float givesCheckBonus = 250f;
    private const bool debug = false;

    /// <summary>
    /// Оценивает ценность хода фигуры с учётом угрозы королю.
    /// </summary>
    public static float EvaluateFigure(Figure fig)
    {
        if (fig == null)
            return 0f;
        
        var team = FiguresRepository.Instance.GetFiguresByTeam(fig.WhiteTeamAffiliation);
        var king = team.FirstOrDefault(f => f.IsKing);
        var detector = king?.KingDetector;
        bool kingUnderAttack = detector?.IsKingUnderAttack() ?? false;
        
        var rawMoves = FigureMoveService.GetAvailableToMoveTiles(fig);
        var moves = MoveFilterService.FilterAll(fig, rawMoves);
        if (moves.Count == 0)
            return 0f;

        float score = 0f;
        if (kingUnderAttack)
        {
            if (detector.CoveringPieces.Contains(fig) == false && !fig.IsKing)
                return 0f;
            if (detector.CoveringPieces.Contains(fig))
                score += coverKingBonus;
        }

        var enemyKing = FiguresRepository.Instance
            .GetFiguresByTeam(!fig.WhiteTeamAffiliation)
            .FirstOrDefault(f => f.IsKing);

        Vector2[] centralCells = {
            new Vector2(1, 1), new Vector2(-1, 1),
            new Vector2(-1, -1), new Vector2(1, -1)
        };

        foreach (var tile in moves)
        {
            var tilePos = new Vector2(tile.Position.x, tile.Position.y);
            float minDistance = centralCells.Min(c => Vector2.Distance(tilePos, c));

            // Бонус за шах
            if (enemyKing != null && tile.Position == enemyKing.CurrentTile?.Position)
                score += givesCheckBonus;

            // Оценка атаки на фигуру
            if (tile.OccupyingFigure != null)
            {
                if (tile.OccupyingFigure.IsKing)
                    score += kingBonus;
                else
                {
                    int value = FigureValueUtility.GetFigureValue(tile.OccupyingFigure);
                    score += (1f / (minDistance + 1f)) * value;
                }
            }
            else
            {
                score += 1f / (minDistance + 1f);
            }
        }

        if (debug)
            Debug.Log($"[AI] {fig.name} at {fig.CurrentTile?.Position} -> {score:F1}");

        return score;
    }

    /// <summary>
    /// Выбирает следующую фигуру для хода по весам оценок.
    /// </summary>
    public static Figure SelectFigure(List<Figure> figures)
    {
        if (figures == null || figures.Count == 0)
            return null;

        var scored = figures
            .Select(f => new { figure = f, weight = EvaluateFigure(f) })
            .Where(x => x.weight > 0f)
            .ToList();
        if (scored.Count == 0)
            return null;

        float total = scored.Sum(x => x.weight);
        float rnd = Random.value * total;
        foreach (var entry in scored)
        {
            rnd -= entry.weight;
            if (rnd <= 0f)
                return entry.figure;
        }
        return scored.Last().figure;
    }
}