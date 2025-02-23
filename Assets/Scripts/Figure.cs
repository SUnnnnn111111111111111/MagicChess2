using UnityEngine;
using System.Collections.Generic;

public class Figure : MonoBehaviour
{
    public NeighborSelectionSettings neighborSelectionSettings;
    private Tile currentTile;

    private void Start()
    {
        // Даем время BoardManager зарегистрировать все клетки перед поиском
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        currentTile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (currentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не смогла найти свою клетку!");
        }
        else
        {
            Debug.Log($"✅ Фигура {gameObject.name} стоит на клетке {currentTile.Position}");
        }
    }

    public void HighlightAvailableMoves()
    {
        if (currentTile != null)
        {
            List<Tile> availableMoves = currentTile.GetNeighbors(neighborSelectionSettings);
            HighlightController.Instance.HighlightTiles(availableMoves);
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не смогла подсветить ходы: текущая клетка не найдена!");
        }
    }

    public void MoveToTile(Tile targetTile)
    {
        if (targetTile == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} → Попытка хода на несуществующую клетку!");
            return;
        }

        if (!targetTile.IsHighlighted) // 🚀 Теперь можно ходить только на доступные клетки
        {
            Debug.LogWarning($"⛔ {gameObject.name} → Клетка {targetTile.name} не является доступной для хода!");
            return;
        }

        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        
        // 🟢 Сбрасываем hover-подсветку на всех клетках перед ходом
        HighlightController.Instance.ClearHighlights();

        // 🟢 Вызываем ResetHoverEffect() у `TileHoverHandler` новой клетки
        TileHoverHandler hoverHandler = targetTile.GetComponentInChildren<TileHoverHandler>();
        if (hoverHandler != null)
        {
            hoverHandler.ResetHoverEffect();
        }

        GameManager.Instance.SelectedFigure = null;

        Debug.Log($"✅ {gameObject.name} переместился на {targetTile.name}");
    }
}
