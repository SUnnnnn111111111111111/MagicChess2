using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightedFigureSelector
{
    private const float kingBonus = 1000f;
    private const float enemyNonKingModifier = 2f;
    private const float coverKingBonus = 100f;
    private const float givesCheckBonus = 120f;
    
    public static Figure SelectFigure(List<Figure> availableFigures)
{
    if (availableFigures.Count == 0)
        return null;

    float totalWeight = 0f;
    List<float> weights = new List<float>();
    Vector2[] centralCells = new Vector2[]
    {
        new Vector2(1, 1),
        new Vector2(-1, 1),
        new Vector2(-1, -1),
        new Vector2(1, -1)
    };

    foreach (Figure fig in availableFigures)
    {
        var teamFigures = FiguresRepository.Instance.GetFiguresByTeam(fig.whiteTeamAffiliation);
        Figure king = teamFigures.FirstOrDefault(f => f.isKing);
        var detector = king?.GetComponent<EnemyKingDetector>();
        
        List<Tile> rawMoves = FigureMoveService.GetAvailableToMoveTiles(fig);
        List<Tile> filteredMoves = MoveFilterService.FilterByCheck(fig, rawMoves);
        
        if (filteredMoves.Count == 0) continue;
        
        if (detector != null && detector.kingUnderAttack)
        {
            if (!detector.coveringPieces.Contains(fig) && !fig.isKing)
                continue; 
        }

        float figureWeight = 0f;
        
        if (detector != null && detector.kingUnderAttack && detector.coveringPieces.Contains(fig))
        {
            figureWeight += coverKingBonus;
        }

        foreach (Tile tile in filteredMoves)
        {
            Vector2 tilePos = new Vector2(tile.Position.x, tile.Position.y);
            float minDistance = float.MaxValue;
            foreach (Vector2 center in centralCells)
            {
                float distance = Vector2.Distance(tilePos, center);
                if (distance < minDistance)
                    minDistance = distance;
            }
            
            if (tile.OccupyingFigure != null && tile.OccupyingFigure.isKing && tile.Position == tile.OccupyingFigure?.CurrentPosition)
            {
                figureWeight += givesCheckBonus;
            }

            if (tile.OccupyingFigure != null)
            {
                if (tile.OccupyingFigure.isKing)
                {
                    figureWeight += kingBonus;
                }
                else
                {
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

    if (weights.Count == 0)
    {
        Debug.LogWarning("У AI нет ни одной фигуры с допустимыми ходами");
        return null;
    }

    float randomValue = Random.value * totalWeight;
    int index = 0;
    foreach (float weight in weights)
    {
        randomValue -= weight;
        if (randomValue <= 0f)
            return availableFigures[index];
        index++;
    }

    return availableFigures[availableFigures.Count - 1];
}

}
