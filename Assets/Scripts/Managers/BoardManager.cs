using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    [SerializeField] private List<GameObject> registeredTiles = new List<GameObject>(); 
    private NeighborUpdater neighborUpdater = new NeighborUpdater();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterTile(Tile tile, Vector2Int position)
    {
        tiles[position] = tile;
        registeredTiles.Add(tile.gameObject); 
        // Debug.Log($" Все клетки инициализированы: {tiles.Count} шт.");
    }

    public Tile GetTileAt(Vector2Int position)
    {
        tiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    public void UpdateNeighbors()
    {
        neighborUpdater.UpdateNeighbors(tiles);
    }
}
