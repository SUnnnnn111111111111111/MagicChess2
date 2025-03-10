using UnityEngine;
using UnityEngine.Serialization;

public class TileHoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject hoverHighlightTile; 
    [SerializeField] private GameObject highlightEmptyTile; 
    [SerializeField] private GameObject highlightEnemyTile;

    private Tile parentTile;
    private bool wasEnemyHighlighted;

    private void Start()
    {
        parentTile = GetComponentInParent<Tile>();

        if (highlightEmptyTile == null)
        {
            Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект (highlightEmptyTile)!");
        }

        if (hoverHighlightTile == null)
        {
            Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект (hoverHighlightTile)!");
        }

        if (highlightEnemyTile == null)
        {
            Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект (highlightEnemyTile)!");
        }
    }

    private void OnMouseEnter()
    {
        if (parentTile != null && parentTile.IsHighlighted)
        {
            if (highlightEnemyTile != null && highlightEnemyTile.activeSelf)
            {
                wasEnemyHighlighted = true; 
                highlightEnemyTile.SetActive(false);
            }
            else if (highlightEmptyTile != null && highlightEmptyTile.activeSelf)
            {
                wasEnemyHighlighted = false; 
                highlightEmptyTile.SetActive(false);
            }

            if (hoverHighlightTile != null)
            {
                hoverHighlightTile.SetActive(true);
            }
        }
    }

    private void OnMouseExit()
    {
        ResetHoverEffect();
    }

    public void ResetHoverEffect()
    {
        if (hoverHighlightTile != null)
        {
            hoverHighlightTile.SetActive(false);
        }

        if (parentTile != null && parentTile.IsHighlighted)
        {
            if (wasEnemyHighlighted && highlightEnemyTile != null)
            {
                highlightEnemyTile.SetActive(true); 
            }
            else if (!wasEnemyHighlighted && highlightEmptyTile != null)
            {
                highlightEmptyTile.SetActive(true); 
            }
        }
    }
}