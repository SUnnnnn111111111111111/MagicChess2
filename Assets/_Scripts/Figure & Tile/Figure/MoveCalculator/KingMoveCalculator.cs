using UnityEngine;
using System.Collections.Generic;

public class KingMoveCalculator : MoveCalculator
{
    public override List<Tile> CalculateMoves(Tile currentTile, NeighborTilesSelectionSettings settings, bool isWhite)
    {
        List<Tile> moves = new List<Tile>();
        Figure figure = currentTile.OccupyingFigure;
        if (figure == null) return moves;

        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);
        Dictionary<Vector2Int, List<Tile>> directionalMoves = new Dictionary<Vector2Int, List<Tile>>();

        foreach (var offset in settings.GetOffsets())
        {
            Vector2Int dir = GetDirection(Vector2Int.zero, offset);
            if (directionalMoves.ContainsKey(dir) == false)
                directionalMoves[dir] = new List<Tile>();
        }

        foreach (var tile in possibleMoves)
        {
            if (tile == null) continue;
            Vector2Int dir = GetDirection(currentTile.Position, tile.Position);
            if (directionalMoves.ContainsKey(dir))
                directionalMoves[dir].Add(tile);
        }

        foreach (var entry in directionalMoves)
        {
            bool blocked = false;
            foreach (var tile in entry.Value)
            {
                if (blocked) break;
                if (tile.IsWall)
                {
                    blocked = true;
                    continue;
                }
                if (tile.OccupyingFigure != null)
                {
                    if (tile.OccupyingFigure.WhiteTeamAffiliation != isWhite)
                        moves.Add(tile);
                    blocked = true;
                }
                else
                {
                    moves.Add(tile);
                }
            }
        }
        
        if (figure.IsKing)
        {
            var castlingMoves = CastlingService.GetCastlingTilesForKing(figure);
            moves.AddRange(castlingMoves);
        }

        return moves;
    }
}
