using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public static FogOfWarManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateFogOfWar()
    {
        foreach (var tile in TilesRepository.Instance.GetTiles().Values)
        {
            tile.SetHiddenByFog(true);
        }

        List<Figure> figuresToReveal = new List<Figure>();

        // Если режим игры против AI, то туман снимается только для фигур игрока.
        if (GameStateManager.Instance.CurrentGameMode == GameStateManager.GameMode.VsAiEnemy)
        {
            figuresToReveal = GameStateManager.Instance.humanPlaysWhite
                ? FiguresRepository.Instance.GetWhiteFigures()
                : FiguresRepository.Instance.GetBlackFigures();
        }
        else // Локальный мультиплеер: туман снимается для фигур текущей команды.
        {
            bool isWhiteTurn = GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitesPlaying;
            figuresToReveal = isWhiteTurn
                ? FiguresRepository.Instance.GetWhiteFigures()
                : FiguresRepository.Instance.GetBlackFigures();
        }

        // Для выбранных фигур снимаем туман с их клеток и соседних.
        foreach (var fig in figuresToReveal)
        {
            if (fig == null) continue;
            if (fig.CurrentTile == null || fig.fogNeighborTilesSelectionSettings == null)
                continue;

            fig.CurrentTile.SetHiddenByFog(false);
            List<Tile> fogTiles = fig.CurrentTile.GetNeighbors(fig.fogNeighborTilesSelectionSettings);
            foreach (var tile in fogTiles)
            {
                tile.SetHiddenByFog(false);
            }
        }
    }
}