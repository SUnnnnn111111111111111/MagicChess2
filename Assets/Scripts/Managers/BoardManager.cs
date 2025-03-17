using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, Figure> figures = new Dictionary<Vector2Int, Figure>();

    [SerializeField] private List<GameObject> registeredTiles = new List<GameObject>(); 
    [SerializeField] private List<GameObject> registeredFigures = new List<GameObject>();
    private TileNeighborUpdater _tileNeighborUpdater = new TileNeighborUpdater();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterTile(Tile tile, Vector2Int position)
    {
        tiles[position] = tile;
        registeredTiles.Add(tile.gameObject);
    }
    
    public void RegisterFigure(Figure figure, Vector2Int position)
    {
        figures[position] = figure;
        registeredFigures.Add(figure.gameObject);
    }
    
    public void UnregisterFigure(Figure figure)
    {
        if (figures.ContainsValue(figure))
        {
            var key = figures.FirstOrDefault(x => x.Value == figure).Key;
            figures.Remove(key);
            registeredFigures.Remove(figure.gameObject);
        }
    }

    public Tile GetTileAt(Vector2Int position)
    {
        tiles.TryGetValue(position, out Tile tile);
        return tile;
    }

    public void UpdateNeighbors()
    {
        _tileNeighborUpdater.UpdateNeighbors(tiles);
    }
    
    public void UpdateFogOfWar()
    {
        foreach (var tile in tiles.Values)
        {
            tile.SetHiddenByFog(true);
        }

        bool isWhiteTurn = GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitePlaying;

        foreach (var fig in registeredFigures)
        {
            if (fig == null) continue;

            Figure figComponent = fig.GetComponent<Figure>();
            if (figComponent == null || figComponent.CurrentTile == null || figComponent.fogNeighborTilesSelectionSettings == null)
                continue;
            
            if (figComponent.whiteTeamAffiliation != isWhiteTurn) 
                continue;
            
            figComponent.CurrentTile.SetHiddenByFog(false);
            
            List<Tile> fogTiles = figComponent.CurrentTile.GetNeighbors(figComponent.fogNeighborTilesSelectionSettings);
            foreach (var tile in fogTiles)
            {
                tile.SetHiddenByFog(false);
            }
        }
    }
}
