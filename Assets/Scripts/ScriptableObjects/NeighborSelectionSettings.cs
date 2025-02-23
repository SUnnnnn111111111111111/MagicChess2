using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NeighborSelectionSettings", menuName = "Chess/NeighborSelectionSettings")]
public class NeighborSelectionSettings : ScriptableObject
{
    public enum NeighborType { Horizontal, Vertical, HorizontalVertical, Diagonal, HorizontalVerticalDiagonal, WhitePawn, BlackPawn, KnightMove }

    [System.Serializable]
    public class NeighborRule
    {
        public NeighborType neighborType;
        public int maxDistance = 2;
    }

    public List<NeighborRule> neighborRules = new List<NeighborRule>();

    public List<Vector2Int> GetOffsets()
    {
        List<Vector2Int> offsets = new List<Vector2Int>();

        foreach (var rule in neighborRules)
        {
            switch (rule.neighborType)
            {
                case NeighborType.Horizontal:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(i, 0));
                        offsets.Add(new Vector2Int(-i, 0));
                    }
                    break;
                case NeighborType.Vertical:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(0, i));
                        offsets.Add(new Vector2Int(0, -i));
                    }
                    break;
                case NeighborType.HorizontalVertical:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(i, 0));
                        offsets.Add(new Vector2Int(-i, 0));
                        offsets.Add(new Vector2Int(0, i));
                        offsets.Add(new Vector2Int(0, -i));
                    }
                    break;
                case NeighborType.Diagonal:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(i, i));
                        offsets.Add(new Vector2Int(-i, -i));
                        offsets.Add(new Vector2Int(i, -i));
                        offsets.Add(new Vector2Int(-i, i));
                    }
                    break;
                case NeighborType.HorizontalVerticalDiagonal:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(i, 0));
                        offsets.Add(new Vector2Int(-i, 0));
                        offsets.Add(new Vector2Int(0, i));
                        offsets.Add(new Vector2Int(0, -i));
                        offsets.Add(new Vector2Int(i, i));
                        offsets.Add(new Vector2Int(-i, -i));
                        offsets.Add(new Vector2Int(i, -i));
                        offsets.Add(new Vector2Int(-i, i));
                    }
                    break;
                case NeighborType.WhitePawn:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(0, i));
                    }
                    break;
                case NeighborType.BlackPawn:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(0, -i));
                    }
                    break;
                case NeighborType.KnightMove:
                    offsets.Add(new Vector2Int(6, 1));
                    offsets.Add(new Vector2Int(6, -1));
                    offsets.Add(new Vector2Int(-6, 1));
                    offsets.Add(new Vector2Int(-6, -1));
                    offsets.Add(new Vector2Int(1, 6));
                    offsets.Add(new Vector2Int(1, -6));
                    offsets.Add(new Vector2Int(-1, 6));
                    offsets.Add(new Vector2Int(-1, -6));
                    break;
            }
        }
        return offsets;
    }
}
