﻿﻿using System.Collections.Generic;
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

        var result = KingThreatStateCache.Instance.GetThreatState(king);
        if (result == null) return false;

        kingUnderAttack = result.isUnderAttack;
        blockableTiles = result.blockableTiles;
        coveringPieces = result.coveringPieces;

        var enemies = FiguresRepository.Instance.GetFiguresByTeam(!figure.whiteTeamAffiliation);
        var allies = FiguresRepository.Instance.GetFiguresByTeam(figure.whiteTeamAffiliation).Where(f => !f.isKing).ToList();
        EnemyKingDetectorUIController.UpdateAlerts(result, king, enemies, allies);

        if (kingUnderAttack)
        {
            List<Tile> kingMoves = TileThreatAnalyzer.FilterKingMoves(
                FigureMoveService.GetAvailableToMoveTiles(king),
                king
            );

            bool kingCanEscape = kingMoves.Count > 0;
            bool someoneCanBlockOrCapture = false;

            if (!result.isDoubleCheck)
            {
                foreach (var ally in coveringPieces)
                {
                    var allyMoves = FigureMoveService.GetAvailableToMoveTiles(ally);

                    bool canBlock = allyMoves.Any(t =>
                        blockableTiles.Any(b => b.Position == t.Position)
                    );

                    bool canCapture = allyMoves.Any(t =>
                        result.attackers.Any(a => a.CurrentTile != null && a.CurrentTile.Position == t.Position)
                    );

                    if (canBlock || canCapture)
                    {
                        someoneCanBlockOrCapture = true;
                        break;
                    }
                }
            }

            if (!kingCanEscape && !someoneCanBlockOrCapture)
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