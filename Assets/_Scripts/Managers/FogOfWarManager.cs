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
        // Сначала скрываем все плитки.
        foreach (var tile in TilesRepository.Instance.GetTiles().Values)
        {
            tile.SetHiddenByFog(true);
        }

        // Определяем, чей сейчас ход.
        bool isWhiteTurn = GameStateManager.Instance.CurrentState == GameStateManager.GameState.WhitesPlaying;

        // Получаем список фигур для текущей команды из репозитория.
        List<Figure> currentFigures = isWhiteTurn ? FiguresRepository.Instance.GetWhiteFigures() : FiguresRepository.Instance.GetBlackFigures();

        foreach (var fig in currentFigures)
        {
            if (fig == null) continue;
            if (fig.CurrentTile == null || fig.fogNeighborTilesSelectionSettings == null)
                continue;

            // Открываем плитку, на которой стоит фигура.
            fig.CurrentTile.SetHiddenByFog(false);

            // Получаем соседей для фигуры и убираем с них туман.
            List<Tile> fogTiles = fig.CurrentTile.GetNeighbors(fig.fogNeighborTilesSelectionSettings);
            foreach (var tile in fogTiles)
            {
                tile.SetHiddenByFog(false);
            }
        }
    }
}