using System.Collections.Generic;

public class UnblockableMoveCalculator : MoveCalculator
{
    public override List<Tile> CalculateMoves(Tile currentTile, NeighborTilesSelectionSettings settings, bool isWhite)
    {
        List<Tile> moves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);

        foreach (var tile in possibleMoves)
        {
            if (tile == null) continue;
            if (tile.IsWall) continue; 
            if (tile.OccupyingFigure == null 
                || tile.OccupyingFigure.WhiteTeamAffiliation != isWhite)
            {
                moves.Add(tile);
            }
        }

        return moves;
    }
}