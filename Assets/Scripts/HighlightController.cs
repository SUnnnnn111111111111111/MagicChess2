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
        ClearHighlights(); // Убираем старую подсветку

        foreach (var tile in tilesToHighlight)
        {
            GameObject highlightObject = tile.GetHighlightObject();
            if (highlightObject != null)
            {
                highlightObject.SetActive(true); // 🟢 Включаем подсветку доступных ходов
                tile.SetHighlighted(true); // 🚀 Теперь Figure может проверить этот флаг
                highlightedTiles.Add(tile);
                Debug.Log($"✅ [Highlight] Подсвечена клетка {tile.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ [Highlight] У клетки {tile.name} не назначен объект подсветки!");
            }
        }
        Debug.Log($"🔆 [Highlight] Подсвечено {highlightedTiles.Count} клеток");
    }

    public void ClearHighlights()
    {
        foreach (var tile in highlightedTiles)
        {
            GameObject highlightObject = tile.GetHighlightObject();
            if (highlightObject != null)
            {
                highlightObject.SetActive(false); // 🔴 Выключаем подсветку доступных ходов
                tile.SetHighlighted(false);
                Debug.Log($"❌ [Highlight] Убрана подсветка у {tile.name}");
            }

            // 🟢 Также сбрасываем Hover-подсветку
            TileHoverHandler hoverHandler = tile.GetComponentInChildren<TileHoverHandler>();
            if (hoverHandler != null)
            {
                hoverHandler.ResetHoverEffect();
            }
        }
        highlightedTiles.Clear();
    }

}
