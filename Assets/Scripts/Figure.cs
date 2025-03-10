using UnityEngine;
using System.Collections.Generic;

public class Figure : MonoBehaviour
{
    public bool whiteTeamAffiliation; 
    public NeighborSelectionSettings neighborSelectionSettings; 
    private Tile currentTile; 

    private void Start()
    {
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        currentTile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (currentTile != null)
        {
            currentTile.SetOccupyingFigure(this);
            // Debug.Log($"✅ Фигура {gameObject.name} зарегистрирована на клетке {currentTile.Position}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
    }
    
    public void HighlightAvailableToMoveTiles()
    {
        if (currentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }

        List<Tile> emptyTiles = new List<Tile>(); 
        List<Tile> enemyTiles = new List<Tile>(); 
        List<Tile> possibleToMoveTiles = currentTile.GetNeighbors(neighborSelectionSettings); 

        // Логика для пешки
        if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.WhitePawn || rule.neighborType == NeighborType.BlackPawn))
        {
            foreach (var tile in possibleToMoveTiles)
            {
                if (tile == null) continue; 
                
                if (tile.OccupyingFigure == null)
                {
                    emptyTiles.Add(tile);
                }
                else if (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                {
                    enemyTiles.Add(tile);
                }
            }
        }
        // Логика для коня
        else if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.KnightMove))
        {
            foreach (var tile in possibleToMoveTiles)
            {
                if (tile == null) continue;
                
                if (tile.OccupyingFigure == null)
                {
                    emptyTiles.Add(tile);
                }
                else if (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                {
                    enemyTiles.Add(tile);
                }
            }
        }
        // Логика для других фигур (ладья, слон, ферзь, король)
        else
        {
            Dictionary<Vector2Int, List<Tile>> directionalMoves = new Dictionary<Vector2Int, List<Tile>>();
            
            foreach (var offset in neighborSelectionSettings.GetOffsets())
            {
                directionalMoves[offset] = new List<Tile>();
            }

            foreach (var tile in possibleToMoveTiles)
            {
                if (tile == null) continue; 

                Vector2Int direction = GetDirection(tile.Position, currentTile.Position);
                if (directionalMoves.ContainsKey(direction))
                {
                    directionalMoves[direction].Add(tile);
                }
            }

            foreach (var entry in directionalMoves)
            {
                bool foundObstacle = false;
                foreach (var tile in entry.Value)
                {
                    if (foundObstacle)
                    {
                        break;
                    }

                    if (tile.OccupyingFigure != null)
                    {
                        if (tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                        {
                            enemyTiles.Add(tile);
                        }
                        foundObstacle = true;
                    }
                    else
                    {
                        emptyTiles.Add(tile);
                    }
                }
            }
        }

        // Удаляем из emptyTiles те клетки, которые уже есть в enemyTiles
        emptyTiles.RemoveAll(tile => enemyTiles.Contains(tile));

        Debug.Log($"✨ Фигура {gameObject.name} подсветила {emptyTiles.Count} клеток и {enemyTiles.Count} вражеских клеток.");
        HighlightTilesController.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesController.Instance.HighlightEnemyTiles(enemyTiles);
    }
    
    private Vector2Int GetDirection(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        return new Vector2Int(
            diff.x == 0 ? 0 : diff.x / Mathf.Abs(diff.x), 
            diff.y == 0 ? 0 : diff.y / Mathf.Abs(diff.y)
        );
    }
    
    public void MoveToTile(Tile targetTile)
    {
        if (targetTile == null)
        {
            return;
        }

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation == whiteTeamAffiliation)
        {
            return;
        }

        if (!targetTile.IsHighlighted)
        {
            return;
        }

        Debug.Log($"🔄 Фигура {gameObject.name} перемещается на клетку {targetTile.Position}.");

        currentTile.SetOccupyingFigure(null);

        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        currentTile.SetOccupyingFigure(this);

        Debug.Log($"✅ Фигура {gameObject.name} завершила перемещение.");

        HighlightTilesController.Instance.ClearHighlights();
        GameManager.Instance.SelectedFigure = null;
    }
}