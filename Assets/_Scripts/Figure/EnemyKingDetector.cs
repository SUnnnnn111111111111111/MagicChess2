using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Figure))]
public class EnemyKingDetector : MonoBehaviour
{
    private Figure figure;

    public bool kingUnderAttack { get; private set; }
    public List<Tile> blockableTiles { get; private set; } = new();
    public List<Figure> coveringPieces { get; private set; } = new();

    private void Awake()
    {
        figure = GetComponent<Figure>();
    }

    private void OnDisable()
    {
        figure.alertUIController?.HideAll();
    }

    public bool isKingIsUnderAttack()
    {
        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing && f.CurrentTile != null);

        if (king == null) return false;

        var enemies = FiguresRepository.Instance.GetFiguresByTeam(!figure.whiteTeamAffiliation);
        var allies = FiguresRepository.Instance.GetFiguresByTeam(figure.whiteTeamAffiliation).Where(f => !f.isKing).ToList();

        var result = KingThreatAnalyzer.Analyze(king, enemies, allies);

        kingUnderAttack = result.isUnderAttack;
        blockableTiles = result.blockableTiles;
        coveringPieces = result.coveringPieces;

        EnemyKingDetectorUIController.UpdateAlerts(result, king, enemies, allies);

        // Проверка мата
        if (kingUnderAttack)
        {
            List<Tile> kingMoves = FigureMoveService.GetAvailableToMoveTiles(king)
                .Where(t => !TileThreatAnalyzer.IsTileUnderThreat(t, king.whiteTeamAffiliation))
                .ToList();

            if (kingMoves.Count == 0)
            {
                var state = figure.whiteTeamAffiliation
                    ? GameStateManager.GameState.WhitesLost
                    : GameStateManager.GameState.BlacksLost;

                GameStateManager.Instance?.SetGameState(state);
            }
        }

        return kingUnderAttack;
    }
}