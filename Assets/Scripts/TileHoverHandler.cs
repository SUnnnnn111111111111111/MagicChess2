using UnityEngine;

public class TileHoverHandler : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject; 
    [SerializeField] private GameObject hoverHighlightObject; 

    private Tile parentTile;

    private void Start()
    {
        parentTile = GetComponentInParent<Tile>(); // 🟢 Находим родительский Tile

        if (highlightObject == null)
        {
            // Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект подсветки (HighlightAvailableNeighbourTiles)!");
        }

        if (hoverHighlightObject == null)
        {
            // Debug.LogWarning($"⚠️ [TileHoverHandler] {name} → Не назначен объект hover-подсветки!");
        }

        // Отключаем hover-подсветку при старте
        if (hoverHighlightObject != null)
        {
            hoverHighlightObject.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        // Debug.Log($"🟢 [TileHoverHandler] {name} → Наведение курсора");

        if (parentTile != null && parentTile.IsHighlighted)
        {
            if (highlightObject != null)
            {
                highlightObject.SetActive(false); // 🔴 Отключаем основную подсветку
                // Debug.Log($"🚫 [TileHoverHandler] {name} → Отключена основная подсветка");
            }

            if (hoverHighlightObject != null)
            {
                hoverHighlightObject.SetActive(true); // 🟢 Включаем hover-подсветку
                // Debug.Log($"✨ [TileHoverHandler] {name} → Включена hover-подсветка");
            }
        }
    }

    private void OnMouseExit()
    {
        ResetHoverEffect(); // 🟢 Теперь вызываем Reset при уходе курсора
    }

    public void ResetHoverEffect()
    {
        // Debug.Log($"🔵 [TileHoverHandler] {name} → Сброс hover-подсветки");

        if (hoverHighlightObject != null)
        {
            hoverHighlightObject.SetActive(false); // 🔴 Выключаем hover-подсветку
            // Debug.Log($"❌ [TileHoverHandler] {name} → Выключена hover-подсветка");
        }

        if (highlightObject != null && parentTile.IsHighlighted)
        {
            highlightObject.SetActive(true); // 🟢 Включаем обратно основную подсветку
            // Debug.Log($"✅ [TileHoverHandler] {name} → Включена основная подсветка");
        }
    }
}
