using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementRule", menuName = "Chess/MovementRule")]
public class MovementRule : ScriptableObject
{
    public List<Vector3> Offsets; // Список смещений для хода фигуры

    public List<Vector3> GetPossibleMoves(Vector3 currentPosition)
    {
        List<Vector3> possibleMoves = new List<Vector3>();
        foreach (Vector3 offset in Offsets)
        {
            possibleMoves.Add(currentPosition + offset);
        }
        return possibleMoves;
    }
}
