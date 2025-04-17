using UnityEngine;

public class AnimationSettingsFactory
{
    public static AnimationSettings GetAnimationSettings(NeighborTilesSelectionSettings neighborTilesSelectionSettings)
    {
        if (neighborTilesSelectionSettings != null && neighborTilesSelectionSettings.neighborRules.Count > 0)
        {
            var neighborType = neighborTilesSelectionSettings.neighborRules[0].neighborType;
            
            switch (neighborType)
            {
                case NeighborType.PawnWhite:
                    return Resources.Load<AnimationSettings>("MovementAnimations/PawnAnimationSettings");
                case NeighborType.PawnBlack:
                    return Resources.Load<AnimationSettings>("MovementAnimations/PawnAnimationSettings");
                case NeighborType.Knight:
                    return Resources.Load<AnimationSettings>("MovementAnimations/KnightAnimationSettings");
                default:
                    return Resources.Load<AnimationSettings>("MovementAnimations/DefaultAnimationSettings");
            }
        }

        return Resources.Load<AnimationSettings>("MovementAnimations/DefaultAnimationSettings");
    }
}