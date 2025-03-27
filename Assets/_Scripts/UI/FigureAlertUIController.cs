using UnityEngine;
using TMPro;

public class FigureAlertUIController : MonoBehaviour
{
    [Header("UI References (Text Alerts)")]
    [SerializeField] private TextMeshProUGUI kingDiscoveryText;
    [SerializeField] private TextMeshProUGUI kingUnderAttackText;
    [SerializeField] private TextMeshProUGUI kingUnderDirectAttackText;

    private void Awake()
    {
        if (kingDiscoveryText != null)
            kingDiscoveryText.gameObject.SetActive(false);
        if (kingUnderAttackText != null)
            kingUnderAttackText.gameObject.SetActive(false);
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

    public void ShowKingUnderDirectAttackText(bool show)
    {
        if (kingUnderDirectAttackText == null) return;
        kingUnderDirectAttackText.gameObject.SetActive(show);
    }

    public void HideAll()
    {
        ShowKingDiscoveryText(false);
        ShowKingUnderAttackText(false);
        ShowKingUnderDirectAttackText(false);
    }
}