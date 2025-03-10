using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public List<Tile> Neighbors { get; private set; } = new List<Tile>();
    public Figure OccupyingFigure { get; private set; } = null; 
    public bool IsHighlighted { get; private set; } = false;

    [SerializeField] private GameObject highlightEmptyTile;
    [SerializeField] private GameObject highlightEnemyTile;
    

    private void Start()
    {
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        BoardManager.Instance.RegisterTile(this, Position);

        if (highlightEmptyTile != null)
        {
            highlightEmptyTile.SetActive(false);
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

    public GameObject GetAvailableHighlightObject()
    {
        return highlightEmptyTile;
    }

    public GameObject GetEnemyHighlightObject()
    {
        return highlightEnemyTile;
    }

    public void SetHighlighted(bool state)
    {
        if (highlightEmptyTile != null)
        {
            highlightEmptyTile.SetActive(state);
            IsHighlighted = state;
        }
    }
    

    public void SetOccupyingFigure(Figure figure)
    {
        OccupyingFigure = figure;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.SelectedFigure != null)
        {
            GameManager.Instance.SelectedFigure.MoveToTile(this);
        }
    }
}

