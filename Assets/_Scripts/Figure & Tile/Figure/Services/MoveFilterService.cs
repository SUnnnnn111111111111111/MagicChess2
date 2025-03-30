using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoveFilterService
{
    public static List<Tile> FilterByCheck(Figure figure, List<Tile> inputMoves)
    {
        if (figure.isKing) return inputMoves;

        var kingDetector = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing)
            ?.GetComponent<EnemyKingDetector>();

        if (kingDetector == null || !kingDetector.kingUnderAttack)
            return inputMoves;

        if (!kingDetector.coveringPieces.Contains(figure))
            return new(); // не может защищать — нет доступных клеток

        return inputMoves
            .Where(t => kingDetector.blockableTiles.Any(b => b.Position == t.Position))
            .ToList();
    }
    
    public static List<Tile> FilterByRayThreatProtection(Figure figure, List<Tile> inputMoves)
    {
        if (figure.isKing || figure.CurrentTile == null)
            return inputMoves;
        
        var kingDetector = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing)
            ?.GetComponent<EnemyKingDetector>();

        if (kingDetector != null && kingDetector.kingUnderAttack)
            return inputMoves;

        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing && f.CurrentTile != null);

        if (king == null)
            return inputMoves;

        var originalTile = figure.CurrentTile;
        var originalOccupant = originalTile.OccupyingFigure;

        // Временно убираем фигуру с клетки
        originalTile.OccupyingFigure = null;

        bool kingNowUnderThreat = TileThreatAnalyzer.IsTileUnderThreat(
            king.CurrentTile,
            king.whiteTeamAffiliation
        );

        // Возвращаем фигуру на место
        originalTile.OccupyingFigure = originalOccupant;

        // Если фигура НЕ прикрывает короля — возвращаем все ходы
        if (!kingNowUnderThreat)
            return inputMoves;

        Debug.Log($"[RayBlock] {figure.name} прикрывает короля. Проверка, какие ходы допустимы...");

        // Если прикрывает — проверим каждый потенциальный ход
        List<Tile> safeMoves = new();

        foreach (var move in inputMoves)
        {
            var moveOriginalOccupant = move.OccupyingFigure;

            // Симуляция хода
            originalTile.OccupyingFigure = null;
            move.OccupyingFigure = figure;
            figure.CurrentTile = move;

            bool threatAfterMove = TileThreatAnalyzer.IsTileUnderThreat(
                king.CurrentTile,
                king.whiteTeamAffiliation
            );

            // Откат
            figure.CurrentTile = originalTile;
            originalTile.OccupyingFigure = figure;
            move.OccupyingFigure = moveOriginalOccupant;

            if (!threatAfterMove)
            {
                Debug.Log($"[RayBlock] Ход на {move.Position} допустим — король в безопасности.");
                safeMoves.Add(move);
            }
            else
            {
                Debug.Log($"[RayBlock] Ход на {move.Position} ЗАПРЕЩЁН — откроется угроза королю.");
            }
        }

        return safeMoves;
    }
}