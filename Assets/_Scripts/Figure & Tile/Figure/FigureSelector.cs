using System.Linq;

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
            !GameStateManager.Instance.IsPlayersTurn())
            return false;

        bool isWhiteTurn = gameState == GameStateManager.GameState.WhitesPlaying;
        if (isWhiteTurn != figure.whiteTeamAffiliation)
            return false;

        if (!figure.GetComponentInChildren<FigureClickHandler>().isActive)
            return false;

        // Проверка шаха
        var kingDetector = FiguresRepository.Instance
            .GetFiguresByTeam(figure.whiteTeamAffiliation)
            .FirstOrDefault(f => f.isKing)
            ?.GetComponent<EnemyKingDetector>();

        if (kingDetector != null && kingDetector.isKingIsUnderAttack())
        {
            bool isKing = figure.isKing;
            bool isCovering = kingDetector.coveringPieces.Contains(figure);
            return isKing || isCovering;
        }

        return true;
    }
}