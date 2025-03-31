public static class MoveSimulationHelper
{
    public static void SimulateMove(Figure figure, Tile from, Tile to, out Figure originalOnTo)
    {
        originalOnTo = to.OccupyingFigure;
        from.OccupyingFigure = null;
        to.OccupyingFigure = figure;
        figure.CurrentTile = to;
    }

    public static void RestoreMove(Figure figure, Tile from, Tile to, Figure originalOnTo)
    {
        figure.CurrentTile = from;
        from.OccupyingFigure = figure;
        to.OccupyingFigure = originalOnTo;
    }
}