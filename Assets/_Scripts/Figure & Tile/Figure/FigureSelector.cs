using System.Linq;
using UnityEngine;

public static class FigureSelector
{
    public static bool CanSelect(Figure figure)
    {
        if (figure == null || figure.CurrentTile == null)
            return false;

        var gameState = GameStateManager.Instance.CurrentState;
        if (gameState == GameStateManager.GameState.Paused ||
            gameState == GameStateManager.GameState.WhitesLost ||
            gameState == GameStateManager.GameState.BlacksLost)
            return false;

        if (GameStateManager.Instance.CurrentGameMode == GameStateManager.GameMode.VsAiEnemy &&
            GameStateManager.Instance.IsPlayersTurn() == false)
            return false;

        bool isWhiteTurn = gameState == GameStateManager.GameState.WhitesPlaying;
        if (isWhiteTurn != figure.WhiteTeamAffiliation)
            return false;

        // Проверка шаха через кэш
        var king = FiguresRepository.Instance
            .GetFiguresByTeam(figure.WhiteTeamAffiliation)
            .FirstOrDefault(f => f.IsKing);

        var result = KingThreatStateCache.Instance.GetThreatState(king);
        
        if (result != null && result.isUnderAttack)
        {
            if (result.isDoubleCheck)
            {
                return figure.IsKing;
            }
        
            return figure.IsKing || result.coveringPieces.Contains(figure);
        }
        
        return true;
    }
}