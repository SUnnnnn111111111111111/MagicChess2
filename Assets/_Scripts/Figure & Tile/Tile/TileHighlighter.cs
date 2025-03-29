using UnityEngine;
using System.Collections.Generic;

public class TileHighlighter
{
    private List<Tile> highlightedTiles = new List<Tile>();

    public void HighlightAvailableTiles(List<Tile> tiles)
    {
        ClearHighlights();

        foreach (var tile in tiles)
        {
            var visuals = tile.GetComponent<TileHighlightVisuals>();
            if (visuals == null) continue;

            visuals.highlightEmptyTile?.SetActive(true);
            visuals.highlightEnemyTile?.SetActive(false);

            tile.SetHighlighted(true);
            highlightedTiles.Add(tile);
        }
    }

    public void HighlightEnemyTiles(List<Tile> tiles)
    {
        foreach (var tile in tiles)
        {
            var visuals = tile.GetComponent<TileHighlightVisuals>();
            if (visuals == null) continue;

            visuals.highlightEmptyTile?.SetActive(false);
            visuals.highlightEnemyTile?.SetActive(true);

            tile.SetHighlighted(true);
            highlightedTiles.Add(tile);
        }
    }

    public void ClearHighlights()
    {
        foreach (var tile in highlightedTiles)
        {
            tile.SetHighlighted(false);
            
            var visuals = tile.GetComponent<TileHighlightVisuals>();
            if (visuals == null) continue;

            visuals.highlightEmptyTile?.SetActive(false);
            visuals.highlightEnemyTile?.SetActive(false);

            var hover = tile.GetComponentInChildren<TileHoverHandler>();
            hover?.ResetHoverEffect();

            
        }

        highlightedTiles.Clear();
    }
}