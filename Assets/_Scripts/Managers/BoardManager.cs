using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    
    private Dictionary<Vector2Int, Figure> whiteFiguresDict = new Dictionary<Vector2Int, Figure>();
    private Dictionary<Vector2Int, Figure> blackFiguresDict = new Dictionary<Vector2Int, Figure>();
    
    [SerializeField] private List<Figure> whiteFiguresList = new List<Figure>();
    [SerializeField] private List<Figure> blackFiguresList = new List<Figure>();

    [SerializeField] private List<GameObject> registeredTiles = new List<GameObject>();

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
        if (figure.whiteTeamAffiliation)
        {
            whiteFiguresDict[position] = figure;
            whiteFiguresList.Add(figure);
        }
        else
        {
            blackFiguresDict[position] = figure;
            blackFiguresList.Add(figure);
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

        bool isWhiteTurn = GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitesPlaying;

        var currentFigures = isWhiteTurn ? whiteFiguresList : blackFiguresList;

        foreach (var fig in currentFigures)
        {
            if (fig == null) continue;
            if (fig.CurrentTile == null || fig.fogNeighborTilesSelectionSettings == null)
                continue;

            fig.CurrentTile.SetHiddenByFog(false);

            List<Tile> fogTiles = fig.CurrentTile.GetNeighbors(fig.fogNeighborTilesSelectionSettings);
            foreach (var tile in fogTiles)
            {
                tile.SetHiddenByFog(false);
            }
        }
    }
    
    public void UnregisterFigure(Figure figure)
    {
        if (figure.whiteTeamAffiliation)
        {
            var key = whiteFiguresDict.FirstOrDefault(x => x.Value == figure).Key;
            whiteFiguresDict.Remove(key);
            whiteFiguresList.Remove(figure);

            if (figure.isKing || whiteFiguresList.Count == 0)
            {
                if (GameStateManager.Instance != null)
                    GameStateManager.Instance.SetGameState(GameStateManager.GameState.WhitesLost);
            }
        }
        else
        {
            var key = blackFiguresDict.FirstOrDefault(x => x.Value == figure).Key;
            blackFiguresDict.Remove(key);
            blackFiguresList.Remove(figure);

            if (figure.isKing || blackFiguresList.Count == 0)
            {
                if (GameStateManager.Instance != null)
                    GameStateManager.Instance.SetGameState(GameStateManager.GameState.BlacksLost);
            }
        }
    }
    
    public Dictionary<Vector2Int, Figure> GetFiguresDictionary(bool isWhite)
    {
        return isWhite ? whiteFiguresDict : blackFiguresDict;
    }
}
