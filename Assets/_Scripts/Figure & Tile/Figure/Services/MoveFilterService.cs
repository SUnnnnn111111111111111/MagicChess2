﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoveFilterService
{
    public static List<Tile> FilterAll(Figure figure, List<Tile> inputMoves)
    {
        if (figure.IsKing) return inputMoves;

        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.WhiteTeamAffiliation)
            .FirstOrDefault(f => f.IsKing && f.CurrentTile != null);

        if (king == null) return inputMoves;

        var result = KingThreatStateCache.Instance.GetThreatState(king);
        if (result == null) return inputMoves;
        
        if (result.isUnderAttack == false)
            return inputMoves;

        if (result.isDoubleCheck)
            return new();

        if (result.coveringPieces.Contains(figure) == false)
            return new();

        return inputMoves
            .Where(t => result.blockableTiles.Any(b => b.Position == t.Position))
            .ToList();
    }

    public static List<Tile> FilterByRayThreatProtection(Figure figure, List<Tile> inputMoves)
    {
        if (figure.IsKing || figure.CurrentTile == null)
            return inputMoves;

        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.WhiteTeamAffiliation)
            .FirstOrDefault(f => f.IsKing && f.CurrentTile != null);

        if (king == null)
            return inputMoves;

        var originalTile = figure.CurrentTile;
        var originalOccupant = originalTile.OccupyingFigure;

        originalTile.OccupyingFigure = null;

        bool kingNowUnderThreat = TileThreatAnalyzer.IsTileUnderThreat(
            king.CurrentTile,
            king.WhiteTeamAffiliation
        );

        originalTile.OccupyingFigure = originalOccupant;

        if (kingNowUnderThreat == false)
            return inputMoves;

        List<Tile> safeMoves = new();

        foreach (var move in inputMoves)
        {
            MoveSimulationHelper.SimulateMove(figure, originalTile, move, out var moveOriginalOccupant);

            var enemies = FiguresRepository.Instance.GetFiguresByTeam(figure.WhiteTeamAffiliation == false);

            bool destroyedThreatSource = enemies.Any(enemy =>
            {
                if (FigureTypeHelper.IsLongRange(enemy) == false) return false;
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
                king.WhiteTeamAffiliation
            );

            if (threatAfterMove == false) safeMoves.Add(move);
            
            MoveSimulationHelper.RestoreMove(figure, originalTile, move, moveOriginalOccupant);
        }

        return safeMoves;
    }
}
