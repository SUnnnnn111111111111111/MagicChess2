using UnityEngine;
using System.Collections.Generic;

public class KnightMoveCalculator : MoveCalculator
{
    public override List<Tile> CalculateMoves(Tile currentTile, NeighborTilesSelectionSettings settings, bool isWhite)
    {
        List<Tile> moves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);

        foreach (var tile in possibleMoves)
        {
            if (tile == null) continue;
            if (tile.isWall) continue; 
            if (tile.OccupyingFigure == null 
                || tile.OccupyingFigure.whiteTeamAffiliation != isWhite)
            {
                moves.Add(tile);
            }
        }

        return moves;
    }
}