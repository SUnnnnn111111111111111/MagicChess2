using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public List<Tile> Neighbors { get; private set; } = new List<Tile>();

    [SerializeField] private GameObject highlightObject; // 🔵 Основная подсветка
    public bool IsHighlighted { get; private set; } = false; // 🚀 Теперь `Figure` проверяет этот флаг

    private void Start()
    {
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        BoardManager.Instance.RegisterTile(this, Position);

        if (highlightObject == null)
        {
            // Debug.LogWarning($"⚠️ [Tile] {name} → Не назначен объект подсветки (HighlightAvailableNeighbourTiles)!");
        }
        else
        {
            highlightObject.SetActive(false); // 🔴 Отключаем по умолчанию
        }
    }

    public void SetNeighbors(List<Tile> neighbors)
    {
        Neighbors = neighbors;
    }

    public List<Tile> GetNeighbors(NeighborSelectionSettings settings)
    {
        List<Tile> result = new List<Tile>();
        foreach (var offset in settings.GetOffsets())
        {
            Tile neighbor = BoardManager.Instance.GetTileAt(Position + offset);
            if (neighbor != null)
                result.Add(neighbor);
        }
        return result;
    }

    public GameObject GetHighlightObject()
    {
        return highlightObject;
    }

    public void SetHighlighted(bool state)
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(state);
            IsHighlighted = state;
            // Debug.Log($"✅ [Tile] {name} → Основная подсветка {(state ? "Включена" : "Выключена")}");
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.SelectedFigure != null)
        {
            GameManager.Instance.SelectedFigure.MoveToTile(this);
        }
    }
}
