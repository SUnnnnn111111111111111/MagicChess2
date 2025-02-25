using UnityEngine;
using System.Collections.Generic;

public class Figure : MonoBehaviour
{
    public int teamAffiliation; // -1: пусто, 0: белая команда, 1: черная команда
    public NeighborSelectionSettings neighborSelectionSettings;
    private Tile currentTile;

    private void Start()
    {
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        currentTile = TileManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (currentTile != null)
        {
            currentTile.SetBlackTeamAffiliation(teamAffiliation);
        }
    }

    public void HighlightAvailableMoves()
    {
        HighlightController.Instance.ClearHighlights(); // Очистка прошлых подсветок

        if (currentTile == null) return;

        List<Tile> availableMoves = new List<Tile>();
        List<Vector2Int> moveOffsets = neighborSelectionSettings.GetOffsets();

        foreach (var offset in moveOffsets)
        {
            Tile nextTile = TileManager.Instance.GetTileAt(currentTile.Position + offset);
            if (nextTile == null) continue;

            if (nextTile.TeamAffiliation == teamAffiliation)
                break;

            if (nextTile.TeamAffiliation != -1 && nextTile.TeamAffiliation != teamAffiliation)
            {
                nextTile.HighlightEnemy();
                availableMoves.Add(nextTile);
                break;
            }

            if (nextTile.TeamAffiliation == -1)
            {
                availableMoves.Add(nextTile);
            }
        }

        HighlightController.Instance.HighlightTiles(availableMoves);
    }



    public void MoveToTile(Tile targetTile)
{
    if (targetTile == null || !targetTile.IsHighlighted) return;

    // Освобождаем текущую клетку
    if (currentTile != null)
    {
        currentTile.SetBlackTeamAffiliation(-1);
    }

    // Если на новой клетке вражеская фигура, уничтожаем её
    if (targetTile.TeamAffiliation != -1 && targetTile.TeamAffiliation != teamAffiliation)
    {
        Figure enemyFigure = targetTile.GetComponentInChildren<Figure>();
        if (enemyFigure != null)
        {
            Destroy(enemyFigure.gameObject);
        }
    }

    // Перемещаем фигуру
    transform.position = targetTile.transform.position;
    currentTile = targetTile;
    currentTile.SetBlackTeamAffiliation(teamAffiliation); // Обновляем информацию о занятости

    // Очищаем подсветку
    HighlightController.Instance.ClearHighlights();
    GameManager.Instance.SelectedFigure = null;
}

}