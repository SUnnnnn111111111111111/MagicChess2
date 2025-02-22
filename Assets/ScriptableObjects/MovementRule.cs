using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementRule", menuName = "Chess/MovementRule")]
public class MovementRule : ScriptableObject
{
    public List<Vector3Int> Offsets; // Список смещений для хода фигуры

    public List<Vector3Int> GetPossibleMoves(Vector3Int currentPosition)
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();
        foreach (Vector3Int offset in Offsets)
        {
            possibleMoves.Add(currentPosition + offset);
        }
        return possibleMoves;
    }
}
