using System.Collections.Generic;
using System.Linq;

public static class TileHighlightService
{
    public static void HighlightTiles(Figure figure, List<Tile> moves)
    {
        List<Tile> emptyTiles = moves.Where(t => t.OccupyingFigure == null).ToList();
        List<Tile> enemyTiles = moves.Where(t =>
            t.OccupyingFigure != null &&
            t.OccupyingFigure.WhiteTeamAffiliation != figure.WhiteTeamAffiliation).ToList();

        HighlightTilesManager.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesManager.Instance.HighlightEnemyTiles(enemyTiles);
    }
}