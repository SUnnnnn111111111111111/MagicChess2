using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FigureLogic
{
    private Figure figure;

    public FigureLogic(Figure owner)
    {
        figure = owner;
    }

    public void HighlightAvailableToMoveTiles(bool includeFog)
    {
        if (figure.CurrentTile == null)
        {
            Debug.LogWarning($"[FigureLogic] Фигура {figure.gameObject.name} не может найти текущую клетку!");
            return;
        }

        List<Tile> moves = FigureMoveService.GetAvailableToMoveTiles(figure);

        if (figure.IsKing == false)
        {
            moves = MoveFilterService.FilterAll(figure, moves);
        }
        else
        {
            moves = TileThreatAnalyzer.FilterKingMoves(moves, figure);
        }

        if (includeFog)
            moves = moves.Where(tile => tile.HiddenByFog == false).ToList();

        TileHighlightService.HighlightTiles(figure, moves);
    }
}