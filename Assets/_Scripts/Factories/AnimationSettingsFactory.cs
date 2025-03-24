using UnityEngine;

public class AnimationSettingsFactory
{
    public static AnimationSettings GetAnimationSettings(NeighborTilesSelectionSettings neighborTilesSelectionSettings)
    {
        if (neighborTilesSelectionSettings != null && neighborTilesSelectionSettings.neighborRules.Count > 0)
        {
            var neighborType = neighborTilesSelectionSettings.neighborRules[0].neighborType;

            // Здесь вы выбираете нужные настройки анимации на основе neighborType
            switch (neighborType)
            {
                case NeighborType.PawnWhite:
                    return Resources.Load<AnimationSettings>("MovementAnimations/PawnWhiteAnimationSettings");
                case NeighborType.Knight:
                    return Resources.Load<AnimationSettings>("MovementAnimations/KnightAnimationSettings");
                // Добавьте другие типы фигур по аналогии
                default:
                    return Resources.Load<AnimationSettings>("MovementAnimations/DefaultAnimationSettings");
            }
        }

        return Resources.Load<AnimationSettings>("MovementAnimations/DefaultAnimationSettings");
    }
}