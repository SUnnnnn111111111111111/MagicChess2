using System.Collections.Generic;

public static class EnemyKingDetectorUIController
{
    public static void UpdateAlerts(
        KingThreatAnalyzer.Result result,
        Figure king,
        List<Figure> enemies,
        List<Figure> allies)
    {
        if (result.isDoubleCheck)
        {
            king.alertUIController?.ShowKingUnderCheckmateText(true);
            king.alertUIController?.ShowKingUnderAttackText(false);

            foreach (var attacker in result.attackers)
                attacker.alertUIController?.ShowKingDiscoveryText(true);

            foreach (var protector in allies)
                protector.alertUIController?.ShowKingProtectingText(false);

            return;
        }

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
    }
}