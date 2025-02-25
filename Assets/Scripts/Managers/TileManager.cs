using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    [SerializeField] private List<GameObject> registeredTiles = new List<GameObject>(); // Список клеток в инспекторе

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterTile(Tile tile, Vector2Int position)
    {
        tiles[position] = tile;
        registeredTiles.Add(tile.gameObject); // Добавляем в список для отображения в Inspector
    }

    public Tile GetTileAt(Vector2Int position)
    {
        tiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    public void UpdateNeighbors()
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
                Tile neighbor = GetTileAt(tile.Position + offset);
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
            tile.SetNeighbors(neighbors);
        }

        // Debug.Log($" Все клетки инициализированы: {tiles.Count} шт.");
    }
}
