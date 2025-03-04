using UnityEngine;
using System.Collections.Generic;

public class HighlightController : MonoBehaviour
{
    public static HighlightController Instance { get; private set; }
    private List<Tile> highlightedTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
    }

    public void HighlightTiles(List<Tile> tilesToHighlight)
    {
        ClearHighlights(); 

        foreach (var tile in tilesToHighlight)
        {
            GameObject highlightObject = tile.GetHighlightObject();
            if (highlightObject != null)
            {
                highlightObject.SetActive(true); 
                tile.SetHighlighted(true); 
                highlightedTiles.Add(tile);
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var tile in highlightedTiles)
        {
            GameObject highlightObject = tile.GetHighlightObject();
            if (highlightObject != null)
            {
                highlightObject.SetActive(false);
                tile.SetHighlighted(false);
            }
            
            TileHoverHandler hoverHandler = tile.GetComponentInChildren<TileHoverHandler>();
            if (hoverHandler != null)
            {
                hoverHandler.ResetHoverEffect();
            }
        }
        highlightedTiles.Clear();
    }
}
