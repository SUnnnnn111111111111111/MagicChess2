using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class FigureLogic : MonoBehaviour
{
    private Figure figure;

    private void Awake()
    {
        figure = GetComponent<Figure>();
    }

    public void HighlightAvailableToMoveTiles(bool includeFog)
    {
        if (figure.CurrentTile == null)
        {
            Debug.LogWarning($"[FigureLogic] Фигура {gameObject.name} не может найти текущую клетку!");
            return;
        }

        // 💡 Получаем возможные ходы из внешнего сервиса
        List<Tile> moves = FigureMoveService.GetAvailableToMoveTiles(figure);

        // 🛡️ Фильтрация по шаху
        if (!figure.isKing)
        {
            moves = MoveFilterService.FilterByCheck(figure, moves);
        }
        else
        {
            moves = moves
                .Where(tile => !TileThreatAnalyzer.IsTileUnderThreat(tile, figure.whiteTeamAffiliation))
                .ToList();
        }

        // 🌫️ Фильтрация по туману
        if (includeFog)
            moves = moves.Where(tile => !tile.HiddenByFog).ToList();

        // ✨ Подсвечиваем
        TileHighlightService.HighlightTiles(figure, moves);
    }
}