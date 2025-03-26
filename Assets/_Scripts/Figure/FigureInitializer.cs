using UnityEngine;

[RequireComponent(typeof(Figure))]
public class FigureInitializer : MonoBehaviour
{
    private Figure figure;

    private void Awake()
    {
        figure = GetComponent<Figure>();
    }

    private void Start()
    {
        InitializeFigurePosition();
        RegisterFigure();
        SpawnUI();
        CloneMovementSettings();
        FogOfWarManager.Instance.UpdateFogOfWar();
    }
    
    private void InitializeFigurePosition()
    {
        figure.CurrentPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );

        figure.CurrentTile = TilesRepository.Instance.GetTileAt(figure.CurrentPosition);
        if (figure.CurrentTile != null)
        {
            figure.CurrentTile.SetOccupyingFigure(figure);
        }
        else
        {
            Debug.LogWarning($"[FigureInitializer] Фигура {gameObject.name} не нашла свою клетку.");
        }
    }

    private void RegisterFigure()
    {
        if (figure.CurrentTile != null)
        {
            FiguresRepository.Instance.RegisterFigure(figure, figure.CurrentPosition);
        }
    }

    private void CloneMovementSettings()
    {
        if (figure.neighborTilesSelectionSettings != null)
        {
            figure.neighborTilesSelectionSettings = Instantiate(figure.neighborTilesSelectionSettings);
        }
    }

    private void SpawnUI()
    {
        if (figure.uiPrefab != null)
        {
            figure.uiController = Instantiate(figure.uiPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}