using System.Collections.Generic;
using System.Linq;

public static class FigureMoveService
{
    public static List<Tile> GetAvailableToMoveTiles(Figure figure)
    {
        if (figure.CurrentTile == null)
            return new List<Tile>();

        MoveCalculator calculator = MoveCalculatorFactory.Create(figure.TilesSelectionSettings);
        List<Tile> rawMoves = calculator.CalculateMoves(
            figure.CurrentTile,
            figure.TilesSelectionSettings,
            figure.WhiteTeamAffiliation
        );

        return rawMoves
            .Where(tile => !tile.IsWall)
            .Where(tile => tile.OccupyingFigure == null ||
                           tile.OccupyingFigure.WhiteTeamAffiliation != figure.WhiteTeamAffiliation)
            .ToList();
    }
    
    
}