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
            Debug.Log($"✅ Фигура {gameObject.name} зарегистрирована на клетке {currentTile.Position}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
    }
    
    public void HighlightAvailableMoves()
    {
        if (currentTile == null)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может найти текущую клетку, ходы не просчитаны!");
            return;
        }

        List<Tile> availableMoves = new List<Tile>(); 
        List<Tile> enemyMoves = new List<Tile>(); 
        List<Tile> possibleMoves = currentTile.GetNeighbors(neighborSelectionSettings); 

        Debug.Log($"🔍 Фигура {gameObject.name} нашла {possibleMoves.Count} возможных ходов.");

        // Логика для пешки
        if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.WhitePawn || rule.neighborType == NeighborType.BlackPawn))
        {
            foreach (var tile in possibleMoves)
            {
                if (tile == null) continue; 
                
                if (tile.OccupyingFigure == null)
                {
                    availableMoves.Add(tile);
                    Debug.Log($"✅ Клетка {tile.Position} добавлена в список доступных ходов для пешки.");
                }
                else if (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                {
                    enemyMoves.Add(tile);
                    Debug.Log($"✅ Клетка {tile.Position} добавлена в список вражеских ходов для пешки.");
                }
            }
        }
        // Логика для коня
        else if (neighborSelectionSettings.neighborRules.Exists(rule => rule.neighborType == NeighborType.KnightMove))
        {
            foreach (var tile in possibleMoves)
            {
                if (tile == null) continue;
                
                if (tile.OccupyingFigure == null)
                {
                    availableMoves.Add(tile);
                    Debug.Log($"✅ Клетка {tile.Position} добавлена в список доступных ходов для коня.");
                }
                else if (tile.OccupyingFigure != null && tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                {
                    enemyMoves.Add(tile);
                    Debug.Log($"✅ Клетка {tile.Position} добавлена в список вражеских ходов для коня.");
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

            foreach (var tile in possibleMoves)
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
                        Debug.Log($"🚧 Преграда найдена, клетка {tile.Position} больше не проверяется.");
                        break;
                    }

                    if (tile.OccupyingFigure != null)
                    {
                        if (tile.OccupyingFigure.whiteTeamAffiliation != whiteTeamAffiliation)
                        {
                            enemyMoves.Add(tile);
                            Debug.Log($"✅ Клетка {tile.Position} добавлена в список вражеских ходов.");
                        }
                        foundObstacle = true;
                        Debug.Log($"🚧 Клетка {tile.Position} занята другой фигурой.");
                    }
                    else
                    {
                        availableMoves.Add(tile);
                        Debug.Log($"✅ Клетка {tile.Position} добавлена в список доступных ходов.");
                    }
                }
            }
        }

        Debug.Log($"✨ Фигура {gameObject.name} подсветила {availableMoves.Count} клеток и {enemyMoves.Count} вражеских клеток.");
        HighlightController.Instance.HighlightAvailableTiles(availableMoves);
        HighlightController.Instance.HighlightEnemyTiles(enemyMoves);
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
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться: цель null!");
            return;
        }

        if (targetTile.OccupyingFigure != null && targetTile.OccupyingFigure.whiteTeamAffiliation == whiteTeamAffiliation)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться: клетка {targetTile.Position} занята своей фигурой!");
            return;
        }

        if (!targetTile.IsHighlighted)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться: клетка {targetTile.Position} не подсвечена!");
            return;
        }

        Debug.Log($"🔄 Фигура {gameObject.name} перемещается на клетку {targetTile.Position}.");

        currentTile.SetOccupyingFigure(null);

        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        currentTile.SetOccupyingFigure(this);

        Debug.Log($"✅ Фигура {gameObject.name} завершила перемещение.");

        HighlightController.Instance.ClearHighlights();
        GameManager.Instance.SelectedFigure = null;
    }
}