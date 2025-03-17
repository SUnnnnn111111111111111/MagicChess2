using UnityEngine;
using System.Collections.Generic;

public class PawnMoveCalculator : MoveCalculator
{
    public override List<Tile> CalculateMoves(Tile currentTile, NeighborTilesSelectionSettings settings, bool isWhite)
    {
        List<Tile> moves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);

        foreach (var tile in possibleMoves)
        {
            if (tile == null) continue;

            Vector2Int direction = GetDirection(currentTile.Position, tile.Position);

            // Пешка может двигаться вперед на пустые клетки
            if (tile.OccupyingFigure == null && direction.y == (isWhite ? 1 : -1))
            {
                moves.Add(tile);
            }
            // Пешка может атаковать по диагонали, если там стоит фигура противника
            else if (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != isWhite && Mathf.Abs(direction.x) == 1)
            {
                moves.Add(tile);
            }
        }

        return moves;
    }
}