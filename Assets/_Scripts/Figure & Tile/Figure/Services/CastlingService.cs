using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class CastlingService
{
    public static bool IsCastlingPossibleInDirection(
        Figure king,
        Vector2Int rookOffset,
        out Tile rookTile,
        out Tile kingTargetTile
    )
    {
        rookTile = null;
        kingTargetTile = null;

        if (king.IsKing == false || king.IsFirstMove == false || king.CurrentTile == null)
            return false;

        bool isWhite = king.WhiteTeamAffiliation;
        Vector2Int kingPos = king.CurrentTile.Position;
        Vector2Int rookPos = kingPos + rookOffset;

        Tile candidateRookTile = TilesRepository.Instance.GetTileAt(rookPos);
        if (candidateRookTile == null)
            return false;

        Figure rook = candidateRookTile.OccupyingFigure;
        if (rook == null || rook.IsFirstMove == false || rook.WhiteTeamAffiliation != isWhite)
            return false;

        var between = GetIntermediateTiles(king.CurrentTile, candidateRookTile);
        if (between.Exists(t => t.OccupyingFigure != null))
            return false;

        if (between.Exists(t => TileThreatAnalyzer.IsTileUnderThreat(t, isWhite)))
            return false;

        if (TileThreatAnalyzer.IsTileUnderThreat(king.CurrentTile, isWhite))
            return false;

        Vector2Int kingTargetPos = kingPos + new Vector2Int(rookOffset.x > 0 ? 4 : -4, 0);
        Tile kingTarget = TilesRepository.Instance.GetTileAt(kingTargetPos);
        if (kingTarget == null)
            return false;

        rookTile = candidateRookTile;
        kingTargetTile = kingTarget;
        return true;
    }

    public static void ExecuteCastlingWithAnimation(Figure king, Tile kingTargetTile, Tile rookTile)
    {
        if (king == null || kingTargetTile == null || rookTile == null)
        {
            return;
        }

        bool isKingside = rookTile.Position.x > king.CurrentTile.Position.x;

        Figure rook = rookTile.OccupyingFigure;
        if (rook == null || rook.IsFirstMove == false)
        {
            return;
        }

        Vector2Int rookTargetPos = kingTargetTile.Position + new Vector2Int(isKingside ? -2 : 2, 0);
        Tile rookTargetTile = TilesRepository.Instance.GetTileAt(rookTargetPos);
        if (rookTargetTile == null)
        {
            return;
        }

        GameStateManager.Instance.madeAFigureMoveAtThisTurn = true;
        HighlightTilesManager.Instance.ClearHighlights();

        rookTile.SetOccupyingFigure(null);
        king.CurrentTile.SetOccupyingFigure(null);

        king.Mover.MoveWithAnimationTo(kingTargetTile, () =>
        {
            rook.Mover.MoveWithAnimationTo(rookTargetTile, () =>
            {
                rookTargetTile.SetOccupyingFigure(rook);
                rook.CurrentTile = rookTargetTile;
                rook.CurrentPosition = rookTargetTile.Position;
                rook.IsFirstMove = false;
                king.IsFirstMove = false;

                FogOfWarManager.Instance.UpdateFogOfWar();
                SelectedFigureManager.Instance.SelectedFigure = null;
                GameStateManager.Instance.EndTurn();
                
            });
        });
    }

    private static List<Tile> GetIntermediateTiles(Tile from, Tile to)
    {
        List<Tile> between = new();
        Vector2Int start = from.Position;
        Vector2Int end = to.Position;

        int dx = end.x - start.x;
        int stepX = dx == 0 ? 0 : dx / Mathf.Abs(dx);

        for (int x = start.x + stepX * 2; Mathf.Abs(x - end.x) > 1; x += stepX * 2)
        {
            Tile t = TilesRepository.Instance.GetTileAt(new Vector2Int(x, start.y));
            if (t != null) between.Add(t);
        }

        return between;
    }
    
    public static List<Tile> GetCastlingTilesForKing(Figure king)
    {
        List<Tile> castlingTiles = new();

        if (king.IsKing == false || king.IsFirstMove == false || king.CurrentTile == null)
            return castlingTiles;
        
        Vector2Int shortOffset = new Vector2Int(6, 0); // 3 клетки вправо = ладья для короткой
        Vector2Int longOffset  = new Vector2Int(-8, 0); // 4 клетки влево = ладья для длинной

        if (IsCastlingPossibleInDirection(king, shortOffset, out _, out var shortTarget))
            castlingTiles.Add(shortTarget);

        if (IsCastlingPossibleInDirection(king, longOffset, out _, out var longTarget))
            castlingTiles.Add(longTarget);

        return castlingTiles;
    }
}
