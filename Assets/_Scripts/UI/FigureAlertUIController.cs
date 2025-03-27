using UnityEngine;
using TMPro;

public class FigureAlertUIController : MonoBehaviour
{
    [Header("UI References (Text Alerts)")]
    [SerializeField] private TextMeshProUGUI kingDiscoveryText;
    [SerializeField] private TextMeshProUGUI kingUnderAttackText;
    [SerializeField] private TextMeshProUGUI kingProtectingText;
    [SerializeField] private TextMeshProUGUI kingUnderDirectAttackText;

    private void Awake()
    {
        if (kingDiscoveryText != null)
            kingDiscoveryText.gameObject.SetActive(false);
        if (kingUnderAttackText != null)
            kingUnderAttackText.gameObject.SetActive(false);
        if (kingProtectingText != null)
            kingProtectingText.gameObject.SetActive(false);
        if (kingUnderDirectAttackText != null)
            kingUnderDirectAttackText.gameObject.SetActive(false);
    }

    public void ShowKingDiscoveryText(bool show)
    {
        if (kingDiscoveryText == null) return;
        kingDiscoveryText.gameObject.SetActive(show);
    }

    public void ShowKingUnderAttackText(bool show)
    {
        if (kingUnderAttackText == null) return;
        kingUnderAttackText.gameObject.SetActive(show);
    }
    
    public void ShowKingProtectingText(bool show)
    {
        if (kingProtectingText == null) return;
        kingProtectingText.gameObject.SetActive(show);
    }

    public void ShowKingUnderDirectAttackText(bool show)
    {
        if (kingUnderDirectAttackText == null) return;
        kingUnderDirectAttackText.gameObject.SetActive(show);
    }

    public void HideAll()
    {
        ShowKingDiscoveryText(false);
        ShowKingUnderAttackText(false);
        ShowKingProtectingText(false);
        ShowKingUnderDirectAttackText(false);
    }
}