using System.Collections.Generic;

public static class EnemyKingDetectorUIController
{
    public static void UpdateAlerts(
        KingThreatAnalyzer.Result result,
        Figure king,
        List<Figure> enemies,
        List<Figure> allies)
    {
        king.AlertUIController?.HideAllAlerts();
        foreach (var enemy in enemies)
            enemy.AlertUIController?.HideAllAlerts();
        foreach (var ally in allies)
            ally.AlertUIController?.HideAllAlerts();

        if (result.isDoubleCheck)
        {
            king.AlertUIController?.SetAlertVisibility(AlertType.KingUnderCheckmate, true);
            return;
        }

        if (result.isUnderAttack == false)
        {
            return;
        }
        
        if (result.isDirect)
            king.AlertUIController?.SetAlertVisibility(AlertType.KingUnderDirectAttack, true);
        else
            king.AlertUIController?.SetAlertVisibility(AlertType.KingUnderAttack, true);
        
        foreach (var attacker in result.attackers)
            attacker.AlertUIController?.SetAlertVisibility(AlertType.KingDiscovery, true);

        foreach (var protector in result.coveringPieces)
            protector.AlertUIController?.SetAlertVisibility(AlertType.KingProtecting, true);
    }
}