using UnityEngine;
using System.Collections.Generic;

public class NeighborUpdater
{
    public void UpdateNeighbors(Dictionary<Vector2Int, Tile> tiles)
    {
        foreach (var tile in tiles.Values)
        {
            List<Tile> neighbors = new List<Tile>();
            foreach (var offset in new List<Vector2Int> {
                         new Vector2Int(1, 0), new Vector2Int(-1, 0),
                         new Vector2Int(0, 1), new Vector2Int(0, -1),
                         new Vector2Int(1, 1), new Vector2Int(-1, -1),
                         new Vector2Int(1, -1), new Vector2Int(-1, 1)
                     })
            {
                Tile neighbor = BoardManager.Instance.GetTileAt(tile.Position + offset);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
            tile.SetNeighbors(neighbors);
        }
    }
}

