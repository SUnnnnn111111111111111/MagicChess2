using UnityEngine;
using System.Collections.Generic;

public class KnightMoveCalculator : MoveCalculator
{
    public override List<Tile> CalculateMoves(Tile currentTile, NeighborSelectionSettings settings, bool isWhite)
    {
        List<Tile> moves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);

        foreach (var tile in possibleMoves)
        {
            if (tile == null) continue;

            // Конь может перемещаться на клетки, если они пусты или заняты фигурой противника
            if (tile.OccupyingFigure == null || tile.OccupyingFigure.whiteTeamAffiliation != isWhite)
            {
                moves.Add(tile);
            }
        }

        return moves;
    }
}