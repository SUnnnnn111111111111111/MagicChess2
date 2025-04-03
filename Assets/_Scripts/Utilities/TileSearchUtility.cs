using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TileSearchUtility
{
    /// <summary>
    /// Возвращает все клетки, у которых МИРОВОЙ X (tileCoord.x * tileStep)
    /// примерно совпадает с заданным targetX (с точностью до epsilon).
    /// </summary>
    public static List<Tile> GetTilesWithWorldX(float targetX, float tileStep, float epsilon = 0.01f)
    {
        var tilesDict = TilesRepository.Instance.GetTiles();
        var result = new List<Tile>();

        foreach (var kvp in tilesDict)
        {
            Vector2Int tileCoord = kvp.Key; // tileCoord = (x,y)
            Tile tile = kvp.Value;

            float worldX = tileCoord.x * tileStep;
            if (Mathf.Abs(worldX - targetX) <= epsilon)
            {
                result.Add(tile);
            }
        }
        return result;
    }

    /// <summary>
    /// Возвращает все клетки, у которых МИРОВОЙ Z (tileCoord.y * tileStep)
    /// примерно совпадает с заданным targetZ (с точностью до epsilon).
    /// </summary>
    public static List<Tile> GetTilesWithWorldZ(float targetZ, float tileStep, float epsilon = 0.01f)
    {
        var tilesDict = TilesRepository.Instance.GetTiles();
        var result = new List<Tile>();

        foreach (var kvp in tilesDict)
        {
            Vector2Int tileCoord = kvp.Key;
            Tile tile = kvp.Value;

            float worldZ = tileCoord.y * tileStep;
            if (Mathf.Abs(worldZ - targetZ) <= epsilon)
            {
                result.Add(tile);
            }
        }
        return result;
    }

    /// <summary>
    /// Возвращает все клетки, находящиеся в пределах радиуса (radius)
    /// от центра (centerWorld.x, centerWorld.y) в МИРОВЫХ координатах.
    /// Для упрощения используем 2D-расстояние (X,Z).
    /// </summary>
    /// <param name="centerWorld">Вектор (x,z) центра в МИРОВЫХ координатах.</param>
    /// <param name="radius">Радиус, в тех же единицах, что и tileStep.</param>
    /// <param name="tileStep">Шаг между клетками.</param>
    public static List<Tile> GetTilesInWorldRadius(Vector2 centerWorld, float radius, float tileStep)
    {
        var tilesDict = TilesRepository.Instance.GetTiles();
        var result = new List<Tile>();

        foreach (var kvp in tilesDict)
        {
            Vector2Int tileCoord = kvp.Key;
            Tile tile = kvp.Value;

            // Переводим tileCoord в мировые coords
            float worldX = tileCoord.x * tileStep;
            float worldZ = tileCoord.y * tileStep;

            // Евклидово расстояние 2D
            float dist = Vector2.Distance(
                new Vector2(worldX, worldZ),
                centerWorld
            );

            if (dist <= radius)
            {
                result.Add(tile);
            }
        }
        return result;
    }
    
    /// <summary>
    /// Вычисляет 2D-координаты центра доски (в tile-координатах),
    /// используя bounding-box всех тайлов.
    /// </summary>
    private static Vector2 GetBoardCenterTileCoords()
    {
        var tilesDict = TilesRepository.Instance.GetTiles();
        if (tilesDict.Count == 0)
            return Vector2.zero;

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var kvp in tilesDict)
        {
            Vector2Int pos = kvp.Key;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        float cx = (minX + maxX) / 2f;
        float cy = (minY + maxY) / 2f;

        return new Vector2(cx, cy);
    }
    
    /// <summary>
    /// Возвращает мировые координаты (Vector2),
    /// соответствующие центру доски.
    /// Параметр tileStep отвечает за шаг между клетками.
    /// </summary>
    public static Vector2 GetBoardCenterWorldPos(float tileStep)
    {
        // Берём центр в tile-координатах:
        Vector2 tileCenter = GetBoardCenterTileCoords();
        // Переводим в мировые, умножая на tileStep:
        return tileCenter * tileStep;
    }
    
    /// <summary>
    /// Возвращает список из count клеток, которые ближе всего
    /// к центру доски в МИРОВЫХ координатах (с учётом tileStep).
    /// </summary>
    /// <param name="count">Сколько ближайших к центру клеток вернуть.</param>
    /// <param name="tileStep">Шаг между клетками в мировых координатах.</param>
    public static List<Tile> GetClosestTilesToCenter(int count, float tileStep)
    {
        var tilesDict = TilesRepository.Instance.GetTiles();
        if (tilesDict.Count == 0)
            return new List<Tile>();

        // Центр доски в tile-координатах:
        Vector2 tileCenter = GetBoardCenterTileCoords();

        // Центр доски в мировых координатах:
        Vector2 centerWorld = tileCenter * tileStep;

        // Сортируем все тайлы по возрастанию distance до centerWorld
        var ordered = tilesDict.Values
            .OrderBy(tile =>
            {
                // Позиция тайла в tile-координатах
                Vector2 tilePos2D = new Vector2(tile.Position.x, tile.Position.y);
                // Переводим в мировые
                Vector2 tileWorldPos = tilePos2D * tileStep;

                // Distance
                return Vector2.Distance(tileWorldPos, centerWorld);
            })
            .Take(count);

        return ordered.ToList();
    }
}
