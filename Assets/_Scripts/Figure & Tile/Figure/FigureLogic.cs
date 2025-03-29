using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class FigureLogic : MonoBehaviour
{
    private Figure figure;

    private void Awake()
    {
        figure = GetComponent<Figure>();
    }

    public void HighlightAvailableToMoveTiles(bool includeFog)
    {
        if (figure.CurrentTile == null)
        {
            Debug.LogWarning($"[FigureLogic] Фигура {gameObject.name} не может найти текущую клетку!");
            return;
        }

        List<Tile> moves = FigureMoveService.GetAvailableToMoveTiles(figure);

        if (!figure.isKing)
        {
            moves = MoveFilterService.FilterByCheck(figure, moves);
        }
        else
        {
            Vector2Int kingPos = figure.CurrentTile.Position;
            List<Tile> filteredMoves = new();

            foreach (Tile move in moves)
            {
                if (TileThreatAnalyzer.IsTileUnderThreat(move, figure.whiteTeamAffiliation))
                    continue;

                if (TileThreatAnalyzer.IsTileUnderFutureThreat(move, figure))
                    continue;

                filteredMoves.Add(move);
            }

            moves = filteredMoves;
        }

        if (includeFog)
            moves = moves.Where(tile => !tile.HiddenByFog).ToList();

        TileHighlightService.HighlightTiles(figure, moves);
    }
}