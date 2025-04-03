﻿using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class TileThreatAnalyzer
{
    public static bool IsTileUnderThreat(Tile tile, bool whiteTeamAffiliation)
    {
        var enemies = FiguresRepository.Instance.GetFiguresByTeam(!whiteTeamAffiliation);
        foreach (var enemy in enemies)
        {
            if (enemy.isKing) continue;
            
            var logic = enemy.GetComponent<FigureLogic>();
            if (logic == null) continue;

            var moves = FigureMoveService.GetAvailableToMoveTiles(enemy);
            if (moves.Any(t => t.Position == tile.Position && !t.HiddenByFog))
                return true;
        }
        return false;
    }

    public static bool IsTileUnderThreatAfterCapture(Tile tile, Figure capturingFigure)
    {
        if (tile == null || capturingFigure == null || tile.OccupyingFigure == null)
            return IsTileUnderThreat(tile, capturingFigure?.whiteTeamAffiliation ?? true);

        var from = capturingFigure.CurrentTile;
        MoveSimulationHelper.SimulateMove(capturingFigure, from, tile, out var original);
        bool result = IsTileUnderThreat(tile, capturingFigure.whiteTeamAffiliation);
        MoveSimulationHelper.RestoreMove(capturingFigure, from, tile, original);

        return result;
    }

    public static bool IsTileUnderFutureThreat(Tile tile, Figure potentialBlocker)
    {
        Vector2Int tilePos = tile.Position;
        Vector2Int blockerPos = potentialBlocker.CurrentTile.Position;
        Vector2Int dirToTile = tilePos - blockerPos;
        if (dirToTile == Vector2Int.zero) return false;

        Vector2Int threatDir = new Vector2Int(-dirToTile.x, -dirToTile.y);
        Tile checkTile = TilesRepository.Instance.GetTileAt(blockerPos + threatDir);

        while (checkTile != null)
        {
            var fig = checkTile.OccupyingFigure;
            if (fig != null)
            {
                if (fig.whiteTeamAffiliation != potentialBlocker.whiteTeamAffiliation && FigureTypeHelper.IsLongRange(fig))
                {
                    Vector2Int attackVector = checkTile.Position - tilePos;
                    Vector2Int normalized = new(
                        attackVector.x == 0 ? 0 : attackVector.x / Mathf.Abs(attackVector.x),
                        attackVector.y == 0 ? 0 : attackVector.y / Mathf.Abs(attackVector.y));

                    List<NeighborType> types = fig.neighborTilesSelectionSettings.neighborRules.Select(r => r.neighborType).ToList();

                    bool valid =
                        (normalized.x == 0 || normalized.y == 0) && (types.Contains(NeighborType.Horizontal) || types.Contains(NeighborType.Vertical) || types.Contains(NeighborType.HorizontalVertical) || types.Contains(NeighborType.HorizontalVerticalDiagonal))
                        ||
                        (Mathf.Abs(normalized.x) == Mathf.Abs(normalized.y)) && (types.Contains(NeighborType.Diagonal) || types.Contains(NeighborType.HorizontalVerticalDiagonal));

                    if (valid)
                        return true;
                }
                break;
            }

            checkTile = TilesRepository.Instance.GetTileAt(checkTile.Position + threatDir);
        }

        return false;
    }

    public static List<Tile> FilterKingMoves(List<Tile> input, Figure king)
    {
        var from = king.CurrentTile;
        var filtered = new List<Tile>();
        foreach (var tile in input)
        {
            MoveSimulationHelper.SimulateMove(king, from, tile, out var occupantOnTo);
            bool threatened = TileThreatAnalyzer.IsTileUnderThreat(king.CurrentTile, king.whiteTeamAffiliation);
            MoveSimulationHelper.RestoreMove(king, from, tile, occupantOnTo);
        
            if (!threatened) 
                filtered.Add(tile);
        }
        return filtered;
    }
}
