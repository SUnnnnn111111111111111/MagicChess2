using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoveFilterService
{
    public static List<Tile> FilterAll(Figure figure, List<Tile> inputMoves)
    {
        if (figure.isKing) return inputMoves;

        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing && f.CurrentTile != null);

        if (king == null) return inputMoves;

        var result = KingThreatStateCache.Instance.GetThreatState(king);
        if (result == null) return inputMoves;
        
        if (!result.isUnderAttack)
            return inputMoves;

        if (result.isDoubleCheck)
            return new();

        if (!result.coveringPieces.Contains(figure))
            return new();

        return inputMoves
            .Where(t => result.blockableTiles.Any(b => b.Position == t.Position))
            .ToList();
    }

    public static List<Tile> FilterByRayThreatProtection(Figure figure, List<Tile> inputMoves)
    {
        if (figure.isKing || figure.CurrentTile == null)
            return inputMoves;

        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing && f.CurrentTile != null);

        if (king == null)
            return inputMoves;

        var originalTile = figure.CurrentTile;
        var originalOccupant = originalTile.OccupyingFigure;

        originalTile.OccupyingFigure = null;

        bool kingNowUnderThreat = TileThreatAnalyzer.IsTileUnderThreat(
            king.CurrentTile,
            king.whiteTeamAffiliation
        );

        originalTile.OccupyingFigure = originalOccupant;

        if (!kingNowUnderThreat)
            return inputMoves;

        List<Tile> safeMoves = new();

        foreach (var move in inputMoves)
        {
            MoveSimulationHelper.SimulateMove(figure, originalTile, move, out var moveOriginalOccupant);

            var enemies = FiguresRepository.Instance.GetFiguresByTeam(!figure.whiteTeamAffiliation);

            bool destroyedThreatSource = enemies.Any(enemy =>
            {
                if (!FigureTypeHelper.IsLongRange(enemy)) return false;
                if (enemy.CurrentTile == null || king.CurrentTile == null) return false;
                if (enemy.CurrentTile.Position != move.Position) return false;

                var path = KingThreatAnalyzer.Analyze(king, new() { enemy }, new() { figure }).blockableTiles;
                return path.Any(t => t.Position == king.CurrentTile.Position);
            });

            if (destroyedThreatSource)
            {
                safeMoves.Add(move);
                MoveSimulationHelper.RestoreMove(figure, originalTile, move, moveOriginalOccupant);
                continue;
            }

            bool threatAfterMove = TileThreatAnalyzer.IsTileUnderThreat(
                king.CurrentTile,
                king.whiteTeamAffiliation
            );

            if (!threatAfterMove) safeMoves.Add(move);
            
            MoveSimulationHelper.RestoreMove(figure, originalTile, move, moveOriginalOccupant);
        }

        return safeMoves;
    }
}
