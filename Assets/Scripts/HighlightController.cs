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

    public void HighlightAvailableTiles(List<Tile> tilesToHighlight)
    {
        ClearHighlights(); 

        foreach (var tile in tilesToHighlight)
        {
            GameObject highlightObject = tile.GetAvailableHighlightObject();
            if (highlightObject != null)
            {
                highlightObject.SetActive(true); 
                tile.SetHighlighted(true); 
                highlightedTiles.Add(tile);
            }
        }
    }
    
    public void HighlightEnemyTiles(List<Tile> tilesToHighlight)
    {
        foreach (var tile in tilesToHighlight)
        {
            GameObject highlightObject = tile.GetEnemyHighlightObject(); 
            if (highlightObject != null)
            {
                highlightObject.SetActive(true);
                tile.SetHighlighted(true);
                highlightedTiles.Add(tile);

                GameObject availableHighlightObject = tile.GetAvailableHighlightObject();
                if (availableHighlightObject != null)
                {
                    availableHighlightObject.SetActive(false);
                }
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var tile in highlightedTiles)
        {
            GameObject highlightAvailableTile = tile.GetAvailableHighlightObject();
            GameObject highlightEnemyTile = tile.GetEnemyHighlightObject();
            if (highlightAvailableTile != null)
            {
                if (highlightAvailableTile != null) highlightAvailableTile.SetActive(false);
                if (highlightEnemyTile != null) highlightEnemyTile.SetActive(false);
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
