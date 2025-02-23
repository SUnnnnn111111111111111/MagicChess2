using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public List<Tile> Neighbors { get; private set; } = new List<Tile>();

    private void Start()
    {
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        BoardManager.Instance.RegisterTile(this, Position);
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

    private void OnMouseDown()
    {
        if (GameManager.Instance.SelectedFigure != null)
        {
            Debug.Log($"🏁 Клик по клетке: {Position}");
            GameManager.Instance.SelectedFigure.MoveToTile(this);
        }
        else
        {
            Debug.LogWarning("⚠️ Клик по клетке, но фигура не выбрана!");
        }
    }
}
