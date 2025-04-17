using UnityEngine;
using TMPro;

public enum AlertType
{
    KingDiscovery,
    KingUnderAttack,
    KingProtecting,
    KingUnderDirectAttack,
    KingUnderCheckmate
}

public class FigureAlertUIController : MonoBehaviour
{
    [Header("UI References (Text Alerts)")]
    [SerializeField] private TextMeshProUGUI kingDiscoveryText;
    [SerializeField] private TextMeshProUGUI kingUnderAttackText;
    [SerializeField] private TextMeshProUGUI kingProtectingText;
    [SerializeField] private TextMeshProUGUI kingUnderDirectAttackText;
    [SerializeField] private TextMeshProUGUI kingUnderCheckmateText;

    private void Awake()
    {
        HideAllAlerts();
    }

    private void SetAlertVisibility(TextMeshProUGUI textElement, bool show)
    {
        if (textElement != null)
            textElement.gameObject.SetActive(show);
    }
    
    public void SetAlertVisibility(AlertType alertType, bool visible)
    {
        switch(alertType)
        {
            case AlertType.KingDiscovery:
                SetAlertVisibility(kingDiscoveryText, visible);
                break;
            case AlertType.KingUnderAttack:
                SetAlertVisibility(kingUnderAttackText, visible);
                break;
            case AlertType.KingProtecting:
                SetAlertVisibility(kingProtectingText, visible);
                break;
            case AlertType.KingUnderDirectAttack:
                SetAlertVisibility(kingUnderDirectAttackText, visible);
                break;
            case AlertType.KingUnderCheckmate:
                SetAlertVisibility(kingUnderCheckmateText, visible);
                break;
            default:
                break;
        }
    }

    public void HideAllAlerts()
    {
        SetAlertVisibility(AlertType.KingDiscovery, false);
        SetAlertVisibility(AlertType.KingUnderAttack, false);
        SetAlertVisibility(AlertType.KingProtecting, false);
        SetAlertVisibility(AlertType.KingUnderDirectAttack, false);
        SetAlertVisibility(AlertType.KingUnderCheckmate, false);
    }
}
