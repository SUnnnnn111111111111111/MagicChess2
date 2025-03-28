using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FigureScoringService
{
    private const float kingBonus = 10000f;
    private const float coverKingBonus = 150f;
    private const float givesCheckBonus = 250f;
    
    private const bool debug = false;

    public static float EvaluateFigure(Figure fig)
    {
        float figureWeight = 0f;

        var teamFigures = FiguresRepository.Instance.GetFiguresByTeam(fig.whiteTeamAffiliation);
        Figure king = teamFigures.FirstOrDefault(f => f.isKing);
        var detector = king?.GetComponent<EnemyKingDetector>();

        List<Tile> rawMoves = FigureMoveService.GetAvailableToMoveTiles(fig);
        List<Tile> filteredMoves = MoveFilterService.FilterByCheck(fig, rawMoves);

        if (filteredMoves.Count == 0) return 0f;

        if (detector != null && detector.kingUnderAttack)
        {
            if (!detector.coveringPieces.Contains(fig) && !fig.isKing)
                return 0f;

            if (detector.coveringPieces.Contains(fig))
                figureWeight += coverKingBonus;
        }

        Figure enemyKing = FiguresRepository.Instance
            .GetFiguresByTeam(!fig.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing);

        Vector2[] centralCells = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(-1, -1),
            new Vector2(1, -1)
        };

        foreach (Tile tile in filteredMoves)
        {
            Vector2 tilePos = new Vector2(tile.Position.x, tile.Position.y);
            float minDistance = centralCells.Min(center => Vector2.Distance(tilePos, center));

            // Бонус за шах
            if (enemyKing != null && tile.Position == enemyKing.CurrentTile?.Position)
            {
                figureWeight += givesCheckBonus;
            }

            // Атака фигуры
            if (tile.OccupyingFigure != null)
            {
                if (tile.OccupyingFigure.isKing)
                {
                    figureWeight += kingBonus;
                }
                else
                {
                    int value = FigureValueUtility.GetFigureValue(tile.OccupyingFigure);
                    figureWeight += (1f / (minDistance + 1f)) * value;
                }
            }
            else
            {
                figureWeight += 1f / (minDistance + 1f);
            }
        }
        
        if (debug)
        {
            Debug.Log($"[AI] {fig.name} ({fig.CurrentTile?.Position}) = {figureWeight:F1} очков");
        }
        
        return figureWeight;
    }

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
        float random = Random.value * total;

        foreach (var entry in scored)
        {
            random -= entry.weight;
            if (random <= 0f)
            {
                if (debug)
                {
                    Debug.Log($"[AI] 👉 Выбрана фигура: {entry.figure.name} с весом {entry.weight:F1}");
                }
                
                return entry.figure;
            }
        }

        return scored.Last().figure;
    }
}
