using System.Linq;

public static class FigureTypeHelper
{
    public static bool IsShortRange(Figure figure)
    {
        if (figure.TilesSelectionSettings == null)
            return false;

        return figure.TilesSelectionSettings.neighborRules.Any(rule =>
            rule.neighborType == NeighborType.PawnWhite ||
            rule.neighborType == NeighborType.PawnBlack ||
            rule.neighborType == NeighborType.Knight);
    }

    public static bool IsLongRange(Figure figure)
    {
        return IsShortRange(figure) == false;
    }
}