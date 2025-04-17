using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TileIconController : MonoBehaviour
{
    [SerializeField] private GameObject pawnPromotionIcon;
    [SerializeField] private GameObject randomPromotionIcon;

    private Tile tile;

    private void Awake()
    {
        tile = GetComponent<Tile>();
        UpdateIcons();
    }

    public void UpdateIcons()
    {
        if (pawnPromotionIcon != null)
            pawnPromotionIcon.SetActive(tile.IsAPawnMovementPromotion);

        if (randomPromotionIcon != null)
            randomPromotionIcon.SetActive(tile.IsAPawnMovementRandomPromotion);
    }
}