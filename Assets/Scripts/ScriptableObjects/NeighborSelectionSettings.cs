using System.Collections.Generic;
using UnityEngine;

public enum NeighborType
{
    Rectangle,
    Horizontal, 
    Vertical, 
    HorizontalVertical, 
    Diagonal, 
    HorizontalVerticalDiagonal, 
    PawnWhite, 
    PawnBlack, 
    Knight
}
[CreateAssetMenu(fileName = "NeighborSelectionSettings", menuName = "Chess/NeighborSelectionSettings")]
public class NeighborSelectionSettings : ScriptableObject
{
    

    [System.Serializable]
    public class NeighborRule
    {
        public NeighborType neighborType;
        public int maxDistance = 2;
        public int rectangleWidth = 0;
        public int rectangleHeight = 0;
    }

    public List<NeighborRule> neighborRules = new List<NeighborRule>();

    public List<Vector2Int> GetOffsets()
    {
        List<Vector2Int> offsets = new List<Vector2Int>();

        foreach (var rule in neighborRules)
        {
            switch (rule.neighborType)
            {
                case NeighborType.Rectangle:
                    for (int x = 0; x <= rule.rectangleWidth; x++)
                    {
                        for (int y = 0; y <= rule.rectangleHeight; y++)
                        {
                            if (x != 0 || y != 0) 
                            {
                                offsets.Add(new Vector2Int(x, y));
                                offsets.Add(new Vector2Int(-x, y));
                                offsets.Add(new Vector2Int(x, -y));
                                offsets.Add(new Vector2Int(-x, -y));
                            }
                        }
                    }
                    break;
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
                case NeighborType.PawnWhite:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(0, i));
                    }
                    break;
                case NeighborType.PawnBlack:
                    for (int i = 1; i <= rule.maxDistance; i++)
                    {
                        offsets.Add(new Vector2Int(0, -i));
                    }
                    break;
                case NeighborType.Knight:
                    offsets.Add(new Vector2Int(4, 2));
                    offsets.Add(new Vector2Int(4, -2));
                    offsets.Add(new Vector2Int(-4, 2));
                    offsets.Add(new Vector2Int(-4, -2));
                    offsets.Add(new Vector2Int(2, 4));
                    offsets.Add(new Vector2Int(2, -4));
                    offsets.Add(new Vector2Int(-2, 4));
                    offsets.Add(new Vector2Int(-2, -4));
                    break;
            }
        }
        return offsets;
    }
}
