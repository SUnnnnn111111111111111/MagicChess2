using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class EnemyKingDetector : MonoBehaviour
{
    private Figure figure;

    public bool kingUnderAttack { get; private set; } = false;
    public List<Tile> blockableTiles { get; private set; } = new();
    public List<Figure> coveringPieces { get; private set; } = new();

    private void Awake()
    {
        figure = GetComponent<Figure>();
    }

    private void OnDisable()
    {
        figure.alertUIController?.HideAll();
    }

    public bool isKingIsUnderAttack()
    {
        kingUnderAttack = false;
        blockableTiles.Clear();
        coveringPieces.Clear();

        Figure friendlyKing = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing);

        if (friendlyKing == null || friendlyKing.CurrentTile == null)
            return false;

        Vector2Int kingPos = friendlyKing.CurrentTile.Position;
        List<Figure> enemyFigures = FiguresRepository.Instance.GetFiguresByTeam(!figure.whiteTeamAffiliation);

        List<Figure> attackers = new();
        List<Tile> threatLine = new();

        foreach (var enemy in enemyFigures)
        {
            var logic = enemy.GetComponent<FigureLogic>();
            if (logic == null) continue;

            List<Tile> enemyMoves = logic.GetAvailableToMoveTiles();
            bool isAttackingKing = enemyMoves.Any(t => t.Position == kingPos && !t.HiddenByFog);

            if (isAttackingKing)
            {
                attackers.Add(enemy);

                var rules = enemy.neighborTilesSelectionSettings.neighborRules;
                bool isShortRangePiece = rules.Any(r =>
                    r.neighborType == NeighborType.PawnWhite ||
                    r.neighborType == NeighborType.PawnBlack ||
                    r.neighborType == NeighborType.Knight);

                if (isShortRangePiece)
                {
                    threatLine.Add(friendlyKing.CurrentTile);
                }
                else
                {
                    var between = CalculateIntermediateTiles(enemy.CurrentTile, friendlyKing.CurrentTile);
                    if (between.Count > 0)
                        threatLine.AddRange(between);

                    threatLine.Add(friendlyKing.CurrentTile);
                }
            }
        }

        blockableTiles = threatLine.Distinct().ToList();

        var friendFigures = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .Where(f => !f.isKing)
            .ToList();

        foreach (var f in friendFigures)
        {
            var logic = f.GetComponent<FigureLogic>();
            if (logic == null) continue;

            var moves = logic.GetAvailableToMoveTiles();
            if (moves.Any(m => blockableTiles.Any(b => b.Position == m.Position)))
            {
                coveringPieces.Add(f);
            }
        }

        if (attackers.Count > 0)
        {
            if (coveringPieces.Count == 0)
            {
                kingUnderAttack = true;
                friendlyKing.alertUIController?.ShowKingUnderDirectAttackText(true);
                friendlyKing.alertUIController?.ShowKingUnderAttackText(false);
            }
            else
            {
                kingUnderAttack = false;
                friendlyKing.alertUIController?.ShowKingUnderAttackText(true);
                friendlyKing.alertUIController?.ShowKingUnderDirectAttackText(false);
            }

            foreach (var attacker in attackers)
                attacker.alertUIController?.ShowKingDiscoveryText(true);
        }
        else
        {
            kingUnderAttack = false;
            friendlyKing.alertUIController?.ShowKingUnderAttackText(false);
            friendlyKing.alertUIController?.ShowKingUnderDirectAttackText(false);

            foreach (var enemy in enemyFigures)
                enemy.alertUIController?.ShowKingDiscoveryText(false);
        }

        return kingUnderAttack;
    }

    private List<Tile> CalculateIntermediateTiles(Tile from, Tile to)
    {
        List<Tile> intermediate = new();
        Vector2Int start = from.Position;
        Vector2Int end = to.Position;

        int deltaX = end.x - start.x;
        int deltaY = end.y - start.y;

        int stepX = deltaX == 0 ? 0 : deltaX / Mathf.Abs(deltaX);
        int stepY = deltaY == 0 ? 0 : deltaY / Mathf.Abs(deltaY);

        if (!(stepX == 0 || stepY == 0 || Mathf.Abs(deltaX) == Mathf.Abs(deltaY)))
            return intermediate;

        Vector2Int pos = new(start.x + stepX, start.y + stepY);
        while (pos != end)
        {
            Tile t = TilesRepository.Instance.GetTileAt(pos);
            if (t != null) intermediate.Add(t);
            pos += new Vector2Int(stepX, stepY);
        }

        return intermediate;
    }
}
