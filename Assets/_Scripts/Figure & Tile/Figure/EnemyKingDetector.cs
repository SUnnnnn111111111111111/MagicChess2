using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyKingDetector
{
    private readonly Figure _owner;
    public bool KingUnderAttack { get; private set; }
    public List<Tile> BlockableTiles { get; private set; } = new List<Tile>();
    public List<Figure> CoveringPieces { get; private set; } = new List<Figure>();

    public EnemyKingDetector(Figure owner)
    {
        _owner = owner;
    }

    public bool IsKingUnderAttack()
    {
        var king = FiguresRepository.Instance
            .GetFiguresByTeam(_owner.WhiteTeamAffiliation)
            .FirstOrDefault(f => f.IsKing && f.CurrentTile != null);

        if (king == null)
            return false;
        
        var result = KingThreatStateCache.Instance.GetThreatState(king);
        if (result == null)
            return false;

        KingUnderAttack = result.isUnderAttack;
        BlockableTiles   = result.blockableTiles;
        CoveringPieces   = result.coveringPieces;

        // Обновляем UI
        EnemyKingDetectorUIController.UpdateAlerts(result, king,
            FiguresRepository.Instance.GetFiguresByTeam(_owner.WhiteTeamAffiliation == false).ToList(),
            FiguresRepository.Instance.GetFiguresByTeam(_owner.WhiteTeamAffiliation).Where(f => f.IsKing == false).ToList()
        );

        // Если король под атакой, проверяем условия пата/матa
        if (KingUnderAttack)
        {
            var kingMoves = TileThreatAnalyzer.FilterKingMoves(
                FigureMoveService.GetAvailableToMoveTiles(king), king);
            bool canEscape = kingMoves.Count > 0;
            bool canBlockOrCapture = false;

            if (result.isDoubleCheck == false)
            {
                foreach (var ally in CoveringPieces)
                {
                    var allyMoves = FigureMoveService.GetAvailableToMoveTiles(ally);
                    if (allyMoves.Any(t => BlockableTiles.Any(b => b.Position == t.Position)) ||
                        allyMoves.Any(t => result.attackers.Any(a => a.CurrentTile?.Position == t.Position)))
                    {
                        canBlockOrCapture = true;
                        break;
                    }
                }
            }

            if (canEscape == false && canBlockOrCapture == false)
            {
                king.AlertUIController?.HideAllAlerts();
                king.AlertUIController?.SetAlertVisibility(AlertType.KingUnderCheckmate, true);

                var state = _owner.WhiteTeamAffiliation
                    ? GameStateManager.GameState.WhitesLost
                    : GameStateManager.GameState.BlacksLost;

                GameStateManager.Instance?.SetGameState(state);
            }
        }

        return KingUnderAttack;
    }

    public void HideAlertsOnDisable()
    {
        _owner.AlertUIController?.HideAllAlerts();
    }
}
