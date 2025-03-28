using System.Collections.Generic;

public static class FigureValueUtility
{
    private static readonly Dictionary<NeighborType, int> values = new()
    {
        { NeighborType.King, 10000 },
        { NeighborType.HorizontalVerticalDiagonal, 500 }, // Queen-like
        { NeighborType.HorizontalVertical, 350 },         // Rook-like
        { NeighborType.Diagonal, 300 },                   // Bishop-like
        { NeighborType.Knight, 300 },                     // Knight
        { NeighborType.KnightPlus, 350 },
        { NeighborType.KnightDouble, 400 },
        { NeighborType.PawnWhite, 100 },
        { NeighborType.PawnBlack, 100 },
        
        { NeighborType.Star, 350 },      
        { NeighborType.Circular, 350 },
        { NeighborType.ZigZag, 350 },
        { NeighborType.Rectangle, 550 }
    };

    public static int GetFigureValue(Figure figure)
    {
        if (figure == null || figure.neighborTilesSelectionSettings == null)
            return 0;

        foreach (var rule in figure.neighborTilesSelectionSettings.neighborRules)
        {
            if (values.TryGetValue(rule.neighborType, out int value))
                return value;
        }

        return 0;
    }
}