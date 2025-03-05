using UnityEngine;

public class FigureMovement : MonoBehaviour
{
    private Tile currentTile;

    public void MoveToTile(Tile targetTile)
    {
        if (targetTile == null || targetTile.IsOccupied)
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не может двигаться на клетку {targetTile?.Position}!");
            return;
        }

        Debug.Log($"🔄 Фигура {gameObject.name} перемещается на клетку {targetTile.Position}.");

        currentTile?.SetOccupied(false);
        transform.position = targetTile.transform.position;
        currentTile = targetTile;
        currentTile.SetOccupied(true);

        Debug.Log($"✅ Фигура {gameObject.name} завершила перемещение.");
    }

    private Vector2Int GetDirection(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        return new Vector2Int(
            diff.x == 0 ? 0 : diff.x / Mathf.Abs(diff.x),
            diff.y == 0 ? 0 : diff.y / Mathf.Abs(diff.y)
        );
    }
}