﻿public static class MoveCalculatorFactory
{
    public static MoveCalculator Create(NeighborTilesSelectionSettings settings)
    {
        if (settings == null) return new DefaultMoveCalculator();

        var rules = settings.neighborRules;

        if (rules.Exists(r => r.neighborType is
                NeighborType.Knight or 
                NeighborType.KnightDouble or
                NeighborType.KnightPlus or 
                NeighborType.ZigZag or 
                NeighborType.Circular or 
                NeighborType.Star))
            return new UnblockableMoveCalculator();

        if (rules.Exists(r => r.neighborType is NeighborType.PawnWhite or NeighborType.PawnBlack))
            return new PawnMoveCalculator();
        if (rules.Exists(r => r.neighborType is NeighborType.King))
            return new KingMoveCalculator();

        return new DefaultMoveCalculator();
    }
}