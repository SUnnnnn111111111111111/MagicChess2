using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "Chess/PieceData")]
public class PieceData : ScriptableObject
{
    public string PieceName; // Название фигуры
    public MovementRule MovementRule; // Правила перемещения фигуры
}
