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

        List<Tile> moves = GetAvailableToMoveTiles();

        if (includeFog)
            moves = moves.Where(tile => !tile.HiddenByFog).ToList();

        List<Tile> emptyTiles = moves.Where(tile => tile.OccupyingFigure == null).ToList();
        List<Tile> enemyTiles = moves.Where(tile => tile.OccupyingFigure != null &&
                                                    tile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation).ToList();
        
        HighlightTilesManager.Instance.HighlightAvailableTiles(emptyTiles);
        HighlightTilesManager.Instance.HighlightEnemyTiles(enemyTiles);
    }

    public List<Tile> GetAvailableToMoveTiles()
    {
        if (figure.CurrentTile == null)
            return new List<Tile>();

        MoveCalculator calculator = MoveCalculatorFactory.Create(figure.neighborTilesSelectionSettings);
        List<Tile> moves = calculator.CalculateMoves(
            figure.CurrentTile,
            figure.neighborTilesSelectionSettings,
            figure.whiteTeamAffiliation
        ).Where(tile => !tile.isWall).ToList();

        return moves.Where(tile =>
            tile.OccupyingFigure == null ||
            tile.OccupyingFigure.whiteTeamAffiliation != figure.whiteTeamAffiliation).ToList();
    }
}