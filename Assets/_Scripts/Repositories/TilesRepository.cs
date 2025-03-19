using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TilesRepository : MonoBehaviour
{
    public static TilesRepository Instance { get; private set; }

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    [SerializeField] private List<GameObject> registeredTiles = new List<GameObject>();
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterTile(Tile tile, Vector2Int position)
    {
        tiles[position] = tile;
        registeredTiles.Add(tile.gameObject);
    }

    public Tile GetTileAt(Vector2Int position)
    {
        tiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    public Dictionary<Vector2Int, Tile> GetTiles()
    {
        return tiles;
    }
}