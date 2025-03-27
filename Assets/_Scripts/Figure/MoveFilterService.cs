using System.Collections.Generic;
using System.Linq;

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
}