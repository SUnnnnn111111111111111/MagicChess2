using UnityEngine;

public class FigureInitializer : MonoBehaviour
{
    private Figure figure;

    private void Awake()
    {
        figure = GetComponent<Figure>();
    }

    private void Start()
    {
        figure.CurrentPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );

        Tile tile = TilesRepository.Instance.GetTileAt(figure.CurrentPosition);
        figure.CurrentTile = tile;

        if (tile != null)
        {
            FiguresRepository.Instance.RegisterFigure(figure, figure.CurrentPosition);
            tile.SetOccupyingFigure(figure);
        }
        else
        {
            Debug.LogWarning($"[FigureInitializer] {gameObject.name} не нашёл свою клетку.");
        }

        if (figure.neighborTilesSelectionSettings != null)
        {
            figure.neighborTilesSelectionSettings = Instantiate(figure.neighborTilesSelectionSettings);
        }

        if (figure.uiPrefab != null)
        {
            figure.uiController = Instantiate(figure.uiPrefab, transform.position, Quaternion.identity, transform);
        }

        FogOfWarManager.Instance.UpdateFogOfWar();
    }

    private void OnDestroy()
    {
        if (FiguresRepository.Instance != null)
            FiguresRepository.Instance.UnregisterFigure(figure);

        if (FogOfWarManager.Instance != null)
            FogOfWarManager.Instance.UpdateFogOfWar();
    }
}