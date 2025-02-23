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
            Renderer tileRenderer = tile.GetComponentInChildren<Renderer>(); // 🟢 Ищем Renderer в дочерних объектах
            if (tileRenderer != null)
            {
                tileRenderer.material.color = Color.yellow;
                highlightedTiles.Add(tile);
            }
            else
            {
                Debug.LogWarning($"⚠️ У клетки {tile.name} нет Renderer!");
            }
        }
        Debug.Log($"✅ Подсвечено {highlightedTiles.Count} клеток");
    }

    public void ClearHighlights()
    {
        foreach (var tile in highlightedTiles)
        {
            Renderer tileRenderer = tile.GetComponentInChildren<Renderer>(); // 🟢 Ищем Renderer в дочерних объектах
            if (tileRenderer != null)
            {
                tileRenderer.material.color = Color.white;
            }
        }
        highlightedTiles.Clear();
    }
}
