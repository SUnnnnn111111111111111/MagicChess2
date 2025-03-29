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
    
    /// <summary>
    /// Обрабатывает продвижение пешки. Если фигура с флагом isPawn встала на клетку с нужным флагом,
    /// создаёт и назначает новые neighborTilesSelectionSettings:
    /// - Если клетка имеет флаг isAPawnMovementRandomPromotion, то настройки заменяются на случайный тип 
    ///   из допустимых значений NeighborType (исключая PawnWhite и PawnBlack).
    /// - Если клетка имеет флаг isAPawnMovementPromotion, то настройки заменяются на тип Rectangle.
    /// </summary>
    /// <param name="figure">Фигура, которая должна быть обработана</param>
    /// <param name="tile">Клетка, на которой находится фигура</param>
    public void HandlePawnMovementPromotion(Figure figure, Tile tile)
    {
        if (figure == null || tile == null)
            return;
        
        if (!figure.isPawn)
            return;
        
        if (tile.isAPawnMovementRandomPromotion)
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
            figure.neighborTilesSelectionSettings = newSettings;

            tile.isAPawnMovementRandomPromotion = false;
            tile.GetComponent<TileIconController>().UpdateIcons();
            // Debug.Log($"PawnMovementPromotionManager: Для пешки настройки соседей изменены на случайный тип: {randomType}.");
        }
        else if (tile.isAPawnMovementPromotion)
        {
            var newSettings = ScriptableObject.CreateInstance<NeighborTilesSelectionSettings>();
            newSettings.neighborRules.Clear();
            var newRule = new NeighborTilesSelectionSettings.NeighborRule();
            newRule.neighborType = NeighborType.Rectangle;
            newRule.rectangleWidth = 4; 
            newRule.rectangleHeight = 4;
            newSettings.neighborRules.Add(newRule);
            figure.neighborTilesSelectionSettings = newSettings;

            tile.isAPawnMovementPromotion = false;
            tile.GetComponent<TileIconController>().UpdateIcons();
            // Debug.Log("PawnMovementPromotionManager: Для пешки настройки соседей изменены на тип Rectangle.");
        }
    }
}
