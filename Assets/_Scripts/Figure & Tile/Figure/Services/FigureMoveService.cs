using System.Collections.Generic;
using System.Linq;

public static class FigureMoveService
{
    public static List<Tile> GetAvailableToMoveTiles(Figure figure)
    {
        if (figure.CurrentTile == null)
            return new List<Tile>();

        MoveCalculator calculator = MoveCalculatorFactory.Create(figure.neighborTilesSelectionSettings);
        List<Tile> rawMoves = calculator.CalculateMoves(
            figure.CurrentTile,
            figure.neighborTilesSelectionSettings,
            figure.whiteTeamAffiliation
        );

        return rawMoves
            .Where(tile => !tile.isWall)
            .Where(tile => tile.OccupyingFigure == null ||
                           tile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation)
            .ToList();
    }
    
    
}