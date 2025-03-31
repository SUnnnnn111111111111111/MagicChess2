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

        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing && f.CurrentTile != null);

        if (king == null)
            return inputMoves;

        var originalTile = figure.CurrentTile;
        var originalOccupant = originalTile.OccupyingFigure;

        // Симулируем исчезновение фигуры с клетки
        originalTile.OccupyingFigure = null;

        bool kingNowUnderThreat = TileThreatAnalyzer.IsTileUnderThreat(
            king.CurrentTile,
            king.whiteTeamAffiliation
        );

        // Откат
        originalTile.OccupyingFigure = originalOccupant;

        if (!kingNowUnderThreat)
            return inputMoves;

        Debug.Log($"[RayBlock] {figure.name} прикрывает короля. Проверка, какие ходы допустимы...");

        List<Tile> safeMoves = new();

        foreach (var move in inputMoves)
        {
            var moveOriginalOccupant = move.OccupyingFigure;

            // Симуляция хода
            originalTile.OccupyingFigure = null;
            move.OccupyingFigure = figure;
            figure.CurrentTile = move;

            // Получаем врагов
            var enemies = FiguresRepository.Instance.GetFiguresByTeam(!figure.whiteTeamAffiliation);

            // Проверка: уничтожаем ли потенциального рентген-угрожателя?
            bool destroyedThreatSource = enemies.Any(enemy =>
            {
                if (!FigureTypeHelper.IsLongRange(enemy)) return false;
                if (enemy.CurrentTile == null || king.CurrentTile == null) return false;
                if (enemy.CurrentTile.Position != move.Position) return false;

                // Проверка: стоял ли враг на линии рентгена
                var path = KingThreatAnalyzer.Analyze(king, new() { enemy }, new() { figure }).blockableTiles;
                return path.Any(t => t.Position == king.CurrentTile.Position);
            });

            if (destroyedThreatSource)
            {
                Debug.Log($"[RayBlock] Ход на {move.Position} ДОПУЩЕН — уничтожает потенциальную рентген-угрозу.");
                safeMoves.Add(move);
                goto Restore;
            }

            // Обычная проверка угрозы после ухода
            bool threatAfterMove = TileThreatAnalyzer.IsTileUnderThreat(
                king.CurrentTile,
                king.whiteTeamAffiliation
            );

            if (!threatAfterMove)
            {
                Debug.Log($"[RayBlock] Ход на {move.Position} ДОПУЩЕН — король в безопасности.");
                safeMoves.Add(move);
            }
            else
            {
                Debug.Log($"[RayBlock] Ход на {move.Position} ЗАПРЕЩЁН — откроется угроза королю.");
            }

        Restore:
            // Откат состояния
            figure.CurrentTile = originalTile;
            originalTile.OccupyingFigure = figure;
            move.OccupyingFigure = moveOriginalOccupant;
        }

        return safeMoves;
    }
}