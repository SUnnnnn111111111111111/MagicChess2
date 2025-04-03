using System.Collections.Generic;
using UnityEngine;

public static class EnemyFiguresUtility
{
    /// <summary>
    /// Возвращает список позиций (Vector2Int) в tile-координатах,
    /// на которых находятся вражеские фигуры команды isWhiteTeam.
    /// </summary>
    public static List<Vector2Int> GetEnemyFigurePositions(bool isWhiteTeam)
    {
        var enemyFigures = FiguresRepository.Instance.GetFiguresByTeam(!isWhiteTeam);

        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (var fig in enemyFigures)
        {
            // Предполагаем, что CurrentPosition = tile-координаты
            if (fig.CurrentTile != null) // Чтобы не брать фигуры, которые уже убиты или убраны
            {
                positions.Add(fig.CurrentPosition);
            }
        }
        return positions;
    }

    /// <summary>
    /// Возвращает список tile-объектов, на которых стоят вражеские фигуры
    /// команды isWhiteTeam (параметр указывает цвет ИИ или игрока).
    /// </summary>
    public static List<Tile> GetEnemyFigureTiles(bool isWhiteTeam)
    {
        var result = new List<Tile>();
        var enemyPositions = GetEnemyFigurePositions(isWhiteTeam);

        foreach (var pos in enemyPositions)
        {
            var tile = TilesRepository.Instance.GetTileAt(pos);
            if (tile != null)
            {
                result.Add(tile);
            }
        }
        return result;
    }

    /// <summary>
    /// Если нужно получить мировые координаты (Vector3) вражеских фигур
    /// (например, для какого-то эффекта) - можно сделать так:
    /// </summary>
    public static List<Vector3> GetEnemyFigureWorldPositions(bool isWhiteTeam, float tileStep)
    {
        var result = new List<Vector3>();
        var enemyPositions = GetEnemyFigurePositions(isWhiteTeam);

        foreach (var tilePos in enemyPositions)
        {
            // Переводим tile-координату в мир (если нужно).
            // Предполагаем (0,0) tile => (0,0,0) world, шаг = tileStep
            Vector3 worldPos = new Vector3(tilePos.x * tileStep, 0f, tilePos.y * tileStep);
            result.Add(worldPos);
        }
        return result;
    }
}
