using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PawnMoveCalculator : MoveCalculator
{
    public override List<Tile> CalculateMoves(Tile currentTile, NeighborTilesSelectionSettings settings, bool isWhite)
    {
        List<Tile> moves = new List<Tile>();
        List<Tile> possibleMoves = currentTile.GetNeighbors(settings);
        Figure figure = currentTile.OccupyingFigure;
        
        if(figure == null)
        {
            Debug.LogWarning($"Текущая клетка {currentTile.Position} не содержит фигуру.");
            return moves;
        }

        foreach (var tile in possibleMoves)
        {
            if (tile == null || tile.isWall) continue;

            Vector2Int direction = tile.Position - currentTile.Position;
            bool isForward = direction.y == (isWhite ? 2 : -2) && direction.x == 0;
            bool isDoubleMove = direction.y == (isWhite ? 4 : -4) && direction.x == 0;
            bool isDiagonalAttack = Mathf.Abs(direction.x) == 2 && direction.y == (isWhite ? 2 : -2);
            
            if (isDoubleMove)
            {
                if (figure.isFirstMove && IsPathClear(currentTile, tile, isWhite))
                    moves.Add(tile);
                continue;
            }
            
            if (tile.OccupyingFigure == null && isForward)
                moves.Add(tile);
            
            else if (tile.OccupyingFigure != null && isDiagonalAttack)
                if (tile.OccupyingFigure.whiteTeamAffiliation != isWhite)
                    moves.Add(tile);
        }
        return moves;
    }

    private bool IsPathClear(Tile currentTile, Tile targetTile, bool isWhite)
    {
        // Промежуточная клетка между currentTile и targetTile
        Vector2Int midPosition = currentTile.Position + new Vector2Int(0, isWhite ? 2 : -2);
        Tile midTile = TilesRepository.Instance.GetTileAt(midPosition);
        
        bool isClear = midTile != null 
                       && midTile.OccupyingFigure == null 
                       && targetTile.OccupyingFigure == null;
        
        return isClear;
    }
}