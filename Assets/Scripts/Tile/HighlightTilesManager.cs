using UnityEngine;
using System.Collections.Generic;

public class HighlightTilesManager : MonoBehaviour
{
    public static HighlightTilesManager Instance { get; private set; }
    private TileHighlighter tileHighlighter;

    private void Awake()
    {
        Instance = this;
        tileHighlighter = new TileHighlighter();
    }

    public void HighlightAvailableTiles(List<Tile> tilesToHighlight)
    {
        tileHighlighter.HighlightAvailableTiles(tilesToHighlight);
    }

    public void HighlightEnemyTiles(List<Tile> tilesToHighlight)
    {
        tileHighlighter.HighlightEnemyTiles(tilesToHighlight);
    }

    public void ClearHighlights()
    {
        tileHighlighter.ClearHighlights();
    }
}

