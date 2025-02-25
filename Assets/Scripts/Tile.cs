using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public Vector2Int Position { get; private set; }
    public List<Tile> Neighbors { get; private set; } = new List<Tile>();

    [SerializeField] private GameObject highlightObject;
    [SerializeField] private GameObject enemyHighlightObject;
    
    [SerializeField] private int teamAffiliation = -1; // -1: пусто, 0: белая, 1: черная
    public int TeamAffiliation => teamAffiliation;

    public bool IsHighlighted { get; private set; } = false;

    private void Start()
    {
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        TileManager.Instance.RegisterTile(this, Position);

        if (highlightObject != null)
            highlightObject.SetActive(false);

        if (enemyHighlightObject != null)
            enemyHighlightObject.SetActive(false);
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
            Tile neighbor = TileManager.Instance.GetTileAt(Position + offset);
            if (neighbor != null)
                result.Add(neighbor);
        }
        return result;
    }

    public void SetBlackTeamAffiliation(int affiliation)
    {
        teamAffiliation = affiliation;
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
        }
    }

    public void HighlightEnemy()
    {
        if (enemyHighlightObject != null)
        {
            enemyHighlightObject.SetActive(true);
        }
    }

    public void ResetEnemyHighlight()
    {
        if (enemyHighlightObject != null)
        {
            enemyHighlightObject.SetActive(false);
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
