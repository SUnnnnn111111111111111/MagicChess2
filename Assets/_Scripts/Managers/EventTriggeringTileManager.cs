using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventTriggeringTileManager : MonoBehaviour
{
    public static EventTriggeringTileManager Instance { get; private set; }
    
    // Префаб для создания event-триггерных клеток.
    [SerializeField] private GameObject eventTriggeringTilePrefab;
    
    private Dictionary<Vector2Int, Tile> firstDictionary = new Dictionary<Vector2Int, Tile>();
    private Dictionary<Vector2Int, Tile> secondDictionary = new Dictionary<Vector2Int, Tile>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Если префаб не назначен через инспектор, загрузим его из Resources.
            if (eventTriggeringTilePrefab == null)
            {
                eventTriggeringTilePrefab = Resources.Load<GameObject>("Prefabs/EventTriggeringTilePrefab");
                if (eventTriggeringTilePrefab == null)
                {
                    Debug.LogError("Не удалось загрузить EventTriggeringTilePrefab из Resources/Prefabs!");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Подписываемся на событие загрузки сцены.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // BuildDictionaries();
        // ReplaceRandomTiles(5);
    }
    
    private void BuildDictionaries()
    {
        firstDictionary.Clear();
        secondDictionary.Clear();
        
        foreach (var kvp in TilesRepository.Instance.GetTiles())
        {
            Tile tile = kvp.Value;
            if (tile.isWall)
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
    /// Метод, который для каждого словаря выбирает указанное количество случайных клеток,
    /// у которых isEventTriggering == false, заменяет их и создаёт на их месте новые клетки с флагом isEventTriggering == true.
    /// </summary>
    /// <param name="count">Количество клеток для замены в каждом словаре.</param>
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
            if (!tile.isEventTriggering)
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
            
            dictionary.Remove(selectedTile.Position);
            Destroy(selectedTile.gameObject);
            
            GameObject newTileObj = Instantiate(eventTriggeringTilePrefab, selectedTile.transform.position, selectedTile.transform.rotation);
            Tile newTile = newTileObj.GetComponent<Tile>();
            if (newTile != null)
            {
                newTile.isEventTriggering = true;
            }
        }
    }
    
    public void HandleEventTrigger(Figure figure, Tile previousTile, Tile newTile)
{
    if (previousTile == null || previousTile != newTile)
    {
        figure.countOfMovesIsOnEventTriggeringTile = 0;
    }
    if (newTile.isEventTriggering)
    {
        figure.countOfMovesIsOnEventTriggeringTile++;
        Debug.Log($"Фигура простояла на тайле {figure.countOfMovesIsOnEventTriggeringTile} ходов");
        if (figure.countOfMovesIsOnEventTriggeringTile >= 3)
        {
            Debug.Log("Фигура провела на тайле больше 3 ходов");
            newTile.isEventTriggering = false;
            
            // Новая логика: задаём случайный тип для neighborTilesSelectionSettings.
            if (figure.neighborTilesSelectionSettings != null)
            {
                // Очищаем текущие правила
                figure.neighborTilesSelectionSettings.neighborRules.Clear();
                
                // Получаем все возможные значения NeighborType
                var neighborTypes = System.Enum.GetValues(typeof(NeighborType));
                // Выбираем случайное значение
                NeighborType randomType = (NeighborType)neighborTypes.GetValue(Random.Range(0, neighborTypes.Length));
                
                // Создаем новое правило и задаем выбранный тип
                var newRule = new NeighborTilesSelectionSettings.NeighborRule();
                newRule.neighborType = randomType;
                
                // Устанавливаем дополнительные параметры в зависимости от выбранного типа
                if (randomType == NeighborType.Rectangle)
                {
                    newRule.rectangleWidth = 4;
                    newRule.rectangleHeight = 4;
                }
                else
                {
                    newRule.maxDistance = 4; // примерное значение для остальных типов
                }
                
                figure.neighborTilesSelectionSettings.neighborRules.Add(newRule);
                Debug.Log($"Настройки соседних клеток изменены на случайный тип: {randomType}");
            }
            
            figure.countOfMovesIsOnEventTriggeringTile = 0;
        }
    }
}

}
