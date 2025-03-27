using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class EnemyKingDetector : MonoBehaviour
{
    private Figure figure;

    public bool kingUnderAttack { get; private set; }
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
        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing && f.CurrentTile != null);

        if (king == null) return false;

        var enemies = FiguresRepository.Instance.GetFiguresByTeam(!figure.whiteTeamAffiliation);
        var allies = FiguresRepository.Instance.GetFiguresByTeam(figure.whiteTeamAffiliation).Where(f => !f.isKing).ToList();

        var result = KingThreatAnalyzer.Analyze(king, enemies, allies);

        // UI
        if (result.isUnderAttack)
        {
            if (result.isDirect)
            {
                king.alertUIController?.ShowKingUnderDirectAttackText(true);
                king.alertUIController?.ShowKingUnderAttackText(false);
            }
            else
            {
                king.alertUIController?.ShowKingUnderAttackText(true);
                king.alertUIController?.ShowKingUnderDirectAttackText(false);
            }

            foreach (var attacker in result.attackers)
                attacker.alertUIController?.ShowKingDiscoveryText(true);

            foreach (var protector in result.coveringPieces)
                protector.alertUIController?.ShowKingProtectingText(true);
        }
        else
        {
            king.alertUIController?.HideAll();
            foreach (var enemy in enemies)
                enemy.alertUIController?.ShowKingDiscoveryText(false);

            foreach (var ally in allies)
                ally.alertUIController?.ShowKingProtectingText(false);
        }

        kingUnderAttack = result.isUnderAttack;
        blockableTiles = result.blockableTiles;
        coveringPieces = result.coveringPieces;

        return kingUnderAttack;
    }
}
