using System.Linq;

public static class TileThreatAnalyzer
{
    public static bool IsTileUnderThreat(Tile tile, bool whiteTeamAffiliation)
    {
        var enemies = FiguresRepository.Instance.GetFiguresByTeam(!whiteTeamAffiliation);
        foreach (var enemy in enemies)
        {
            var logic = enemy.GetComponent<FigureLogic>();
            if (logic == null) continue;

            var moves = FigureMoveService.GetAvailableToMoveTiles(enemy);
            if (moves.Any(t => t.Position == tile.Position && !t.HiddenByFog))
                return true;
        }
        return false;
    }
}