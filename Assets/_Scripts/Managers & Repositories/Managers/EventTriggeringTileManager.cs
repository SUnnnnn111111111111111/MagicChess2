using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class EventTriggeringTileManager : MonoBehaviour
{
    public static EventTriggeringTileManager Instance { get; private set; }

    [SerializeField] private int countReplacesTiles = 3;
    [SerializeField] private int maxCountOfMovesIsOnSideEventTriggeringTile;
    [SerializeField] private int maxCountOfMovesIsOnMiddleEventTriggeringTile;
    
    [SerializeField] private Material sideEventTriggerMaterial;
    [SerializeField] private Material middleEventTriggerMaterial;
    
    private Dictionary<Tile, Material> originalMaterials = new Dictionary<Tile, Material>();
    
    private Dictionary<Vector2Int, Tile> firstDictionary = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, Tile> secondDictionary = new Dictionary<Vector2Int, Tile>();
    
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

    public void ReplaseTiles()
    {
        BuildDictionaries();
        ReplaceRandomTiles(countReplacesTiles);
        ReplaceCentralTiles();
    }
    
    private void BuildDictionaries()
    {
        firstDictionary.Clear();
        secondDictionary.Clear();
        
        foreach (var kvp in TilesRepository.Instance.GetTiles())
        {
            Tile tile = kvp.Value;
            if (tile == null || tile.gameObject == null || tile.isWall)
                continue;
                
            Vector2Int pos = tile.Position;
            
            if (pos.x >= -17 && pos.x <= -3 && pos.y >= -9 && pos.y <= 17)
            {
                firstDictionary[pos] = tile;
            }
            
            if (pos.x >= 3 && pos.x <= 17 && pos.y >= -17 && pos.y <= 9)
            {
                secondDictionary[pos] = tile;
            }
        }
    }
    
    /// <summary>
    /// Обрабатывает боковые (side) event‑тайлы: выбирает случайные клетки из словаря,
    /// сохраняет их исходный материал, меняет материал на sideEventTriggerMaterial и устанавливает флаг isSideEventTriggering.
    /// </summary>
    public void ReplaceRandomTiles(int count)
    {
        ReplaceRandomTilesFromDictionary(firstDictionary, count);
        ReplaceRandomTilesFromDictionary(secondDictionary, count);
    }
    
    private void ReplaceRandomTilesFromDictionary(Dictionary<Vector2Int, Tile> dictionary, int count)
    {
        List<Tile> candidates = new List<Tile>();
        foreach (var kvp in dictionary)
        {
            Tile tile = kvp.Value;
            if (!tile.isSideEventTriggering)
            {
                candidates.Add(tile);
            }
        }
        
        int numberToReplace = Mathf.Min(count, candidates.Count);
        
        for (int i = 0; i < numberToReplace; i++)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            Tile selectedTile = candidates[randomIndex];
            candidates.RemoveAt(randomIndex);
            
            Renderer rend = selectedTile.GetComponentInChildren<Renderer>();
            if (rend != null && sideEventTriggerMaterial != null)
            {
                if (!originalMaterials.ContainsKey(selectedTile))
                {
                    originalMaterials[selectedTile] = rend.material;
                }
                rend.material = sideEventTriggerMaterial;
            }
            
            selectedTile.isSideEventTriggering = true;
        }
    }
    
    /// <summary>
    /// Обрабатывает центральные тайлы: для клеток с позициями (-1,1), (1,1), (-1,-1), (1,-1) 
    /// если флаг isMiddleEventTriggering не установлен, сохраняет исходный материал, заменяет материал на middleEventTriggerMaterial
    /// и устанавливает флаг isMiddleEventTriggering в true.
    /// </summary>
    private void ReplaceCentralTiles()
    {
        Vector2Int[] centralPositions = new Vector2Int[]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -1)
        };
        
        foreach (var kvp in TilesRepository.Instance.GetTiles())
        {
            Tile tile = kvp.Value;
            foreach (var pos in centralPositions)
            {
                if (tile.Position == pos && !tile.isMiddleEventTriggering)
                {
                    Renderer rend = tile.GetComponentInChildren<Renderer>();
                    if (rend != null && middleEventTriggerMaterial != null)
                    {
                        if (!originalMaterials.ContainsKey(tile))
                        {
                            originalMaterials[tile] = rend.material;
                        }
                        rend.material = middleEventTriggerMaterial;
                    }
                    tile.isMiddleEventTriggering = true;
                }
            }
        }
    }
    
    /// <summary>
    /// Обрабатывает тайлы, на которых стоит фигура.
    /// Если фигура стоит на боковом event‑тайле, после 3 ходов флаг сбрасывается, счётчик обнуляется и материал возвращается.
    /// Если фигура стоит на центральном тайле, после 5 ходов выводится лог, флаг сбрасывается, соответствующий счетчик обнуляется и материал возвращается.
    /// </summary>
    public void HandleEventTrigger(Figure figure, Tile newTile)
    {
        if (figure.hasMovedThisTurn)
        {
            figure.countOfMovesIsOnEventTriggeringTile = 0;
        }
        else if (newTile.isSideEventTriggering || newTile.isMiddleEventTriggering)
        {
            figure.countOfMovesIsOnEventTriggeringTile++;
            
            if (newTile.isSideEventTriggering)
            {
                UIManager.Instance?.ShowFigureMoveCount(
                    figure,
                    figure.countOfMovesIsOnEventTriggeringTile,
                    maxCountOfMovesIsOnSideEventTriggeringTile
                );
                // Если достигнут лимит ходов на боковом тайле – сбрасываем флаг и восстанавливаем материал
                if (figure.countOfMovesIsOnEventTriggeringTile >= maxCountOfMovesIsOnSideEventTriggeringTile)
                {
                    newTile.isSideEventTriggering = false;
                    figure.countOfMovesIsOnEventTriggeringTile = 0;
                    Renderer rend = newTile.GetComponentInChildren<Renderer>();
                    if (rend != null && originalMaterials.ContainsKey(newTile))
                    {
                        rend.material = originalMaterials[newTile];
                        originalMaterials.Remove(newTile);
                    }
                }
            }
            if (newTile.isMiddleEventTriggering)
            {
                UIManager.Instance?.ShowFigureMoveCount(
                    figure,
                    figure.countOfMovesIsOnEventTriggeringTile,
                    maxCountOfMovesIsOnMiddleEventTriggeringTile
                );
                // Если достигнут лимит ходов на центральном тайле – сбрасываем флаг и восстанавливаем материал
                if (figure.countOfMovesIsOnEventTriggeringTile >= maxCountOfMovesIsOnMiddleEventTriggeringTile)
                {
                    newTile.isMiddleEventTriggering = false;
                    figure.countOfMovesIsOnEventTriggeringTile = 0;
                    Renderer rend = newTile.GetComponentInChildren<Renderer>();
                    if (rend != null && originalMaterials.ContainsKey(newTile))
                    {
                        rend.material = originalMaterials[newTile];
                        originalMaterials.Remove(newTile);
                    }
                }
            }
        }
    }
}
