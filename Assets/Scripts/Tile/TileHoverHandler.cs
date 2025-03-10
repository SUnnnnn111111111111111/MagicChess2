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
    }

    private void OnMouseEnter()
    {
        if (parentTile != null && parentTile.IsHighlighted)
        {
            wasEnemyHighlighted = highlightEnemyTile != null && highlightEnemyTile.activeSelf;
            highlightEnemyTile?.SetActive(false);
            highlightEmptyTile?.SetActive(false);
            hoverHighlightTile?.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        ResetHoverEffect();
    }

    public void ResetHoverEffect()
    {
        hoverHighlightTile?.SetActive(false);

        if (parentTile != null && parentTile.IsHighlighted)
        {
            if (wasEnemyHighlighted)
            {
                highlightEnemyTile?.SetActive(true);
            }
            else
            {
                highlightEmptyTile?.SetActive(true);
            }
        }
    }
}