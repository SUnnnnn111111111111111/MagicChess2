using UnityEngine;
using System.Collections.Generic;

public class Figure : MonoBehaviour
{
    public NeighborSelectionSettings neighborSelectionSettings;
    private Tile currentTile;

    private void Start()
    {
        // Ожидаем 0.1 секунды перед установкой клетки (чтобы дать BoardManager зарегистрировать клетки)
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
        if (targetTile == null) return;

        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        HighlightController.Instance.ClearHighlights();
        GameManager.Instance.SelectedFigure = null;
    }
}
