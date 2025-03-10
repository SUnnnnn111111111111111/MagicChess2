using UnityEngine;

public class TileHoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject; 
    [SerializeField] private GameObject hoverHighlightObject; 

    private Tile parentTile;

    private void Start()
    {
        parentTile = GetComponentInParent<Tile>();

        if (highlightObject == null)
        {
            Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект подсветки (HighlightAvailableNeighbourTiles)!");
        }

        if (hoverHighlightObject == null)
        {
            Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект hover-подсветки!");
        }
        
        if (hoverHighlightObject != null)
        {
            hoverHighlightObject.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        if (parentTile != null && parentTile.IsHighlighted)
        {
            if (highlightObject != null)
            {
                highlightObject.SetActive(false); 
            }

            if (hoverHighlightObject != null)
            {
                hoverHighlightObject.SetActive(true); 
            }
        }
    }

    private void OnMouseExit()
    {
        ResetHoverEffect(); 
    }

    public void ResetHoverEffect()
    {
        if (hoverHighlightObject != null)
        {
            hoverHighlightObject.SetActive(false); 
        }

        if (highlightObject != null && parentTile.IsHighlighted)
        {
            highlightObject.SetActive(true); 
        }
    }
}
