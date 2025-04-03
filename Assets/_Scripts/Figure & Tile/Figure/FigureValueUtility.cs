using System.Collections.Generic;

public static class FigureValueUtility
{
    private static readonly Dictionary<NeighborType, int> values = new()
    {
        { NeighborType.King, 20000 },
        { NeighborType.HorizontalVerticalDiagonal, 900 },
        { NeighborType.HorizontalVertical, 500 },
        { NeighborType.Diagonal, 350 },
        { NeighborType.Knight, 300 },
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

        if (figure.isKing)
            return values[NeighborType.King];

        foreach (var rule in figure.neighborTilesSelectionSettings.neighborRules)
        {
            if (values.TryGetValue(rule.neighborType, out int value))
                return value;
        }

        return 0;
    }

    public const float KingKillWeight = 20000f;
    public const float KingBonusWeight = 150f;
    public const float CoverKingBonus = 150f;
    public const float GivesCheckBonus = 250f;

    public const float EventTileBonus = 50f;
    public const float RegionBonus = 30f;

    public const float ThreatenedTilePenalty = 100f;
    public const float EnemyFigureBonusBase = 300f;
}