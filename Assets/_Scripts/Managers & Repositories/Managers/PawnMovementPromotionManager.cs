using UnityEngine;
using System.Collections.Generic;

public class PawnMovementPromotionManager : MonoBehaviour
{
    public static PawnMovementPromotionManager Instance { get; private set; }

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
    
    public void HandlePawnMovementPromotion(Figure figure, Tile tile)
    {
        if (figure == null || tile == null)
            return;
        
        if (figure.IsPawn == false)
            return;
        
        if (tile.IsAPawnMovementRandomPromotion)
        {
            NeighborType[] allTypes = (NeighborType[])System.Enum.GetValues(typeof(NeighborType));
            List<NeighborType> validTypes = new List<NeighborType>();
            foreach (var type in allTypes)
            {
                if (type != NeighborType.PawnWhite && type != NeighborType.PawnBlack)
                {
                    validTypes.Add(type);
                }
            }
            
            NeighborType randomType = validTypes[Random.Range(0, validTypes.Count)];
            
            // Создаем новый экземпляр настроек
            var newSettings = ScriptableObject.CreateInstance<NeighborTilesSelectionSettings>();
            newSettings.neighborRules.Clear();
            var newRule = new NeighborTilesSelectionSettings.NeighborRule();
            newRule.neighborType = randomType;
            
            if (randomType == NeighborType.Rectangle)
            {
                newRule.rectangleWidth = 2; 
                newRule.rectangleHeight = 2;
            }
            else
            {
                newRule.maxDistance = 4;
            }
            
            newSettings.neighborRules.Add(newRule);
            figure.TilesSelectionSettings = newSettings;

            tile.IsAPawnMovementRandomPromotion = false;
            tile.GetComponent<TileIconController>().UpdateIcons();
            // Debug.Log($"PawnMovementPromotionManager: Для пешки настройки соседей изменены на случайный тип: {randomType}.");
        }
        else if (tile.IsAPawnMovementPromotion)
        {
            var newSettings = ScriptableObject.CreateInstance<NeighborTilesSelectionSettings>();
            newSettings.neighborRules.Clear();
            var newRule = new NeighborTilesSelectionSettings.NeighborRule();
            newRule.neighborType = NeighborType.Rectangle;
            newRule.rectangleWidth = 4; 
            newRule.rectangleHeight = 4;
            newSettings.neighborRules.Add(newRule);
            figure.TilesSelectionSettings = newSettings;

            tile.IsAPawnMovementPromotion = false;
            tile.GetComponent<TileIconController>().UpdateIcons();
            // Debug.Log("PawnMovementPromotionManager: Для пешки настройки соседей изменены на тип Rectangle.");
        }
    }
}
