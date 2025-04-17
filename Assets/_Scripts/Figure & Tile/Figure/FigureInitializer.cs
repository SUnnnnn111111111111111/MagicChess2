using UnityEngine;

[System.Serializable]
public class FigureInitializer
{
    private Figure figure;

    public FigureInitializer(Figure owner)
    {
        figure = owner;
    }
    
    public void Initialize()
    {
        InitializeFigurePosition();
        RegisterFigure();
        SpawnUI();
        CloneMovementSettings();
        
        figure.IsFirstMove = true;
        FogOfWarManager.Instance.UpdateFogOfWar();
    }

    private void InitializeFigurePosition()
    {
        figure.CurrentPosition = new Vector2Int(
            Mathf.RoundToInt(figure.transform.position.x),
            Mathf.RoundToInt(figure.transform.position.z)
        );

        figure.CurrentTile = TilesRepository.Instance.GetTileAt(figure.CurrentPosition);
        if (figure.CurrentTile != null)
        {
            figure.CurrentTile.SetOccupyingFigure(figure);
        }
        else
        {
            Debug.LogWarning($"[FigureInitializer] Фигура {figure.gameObject.name} не нашла свою клетку.");
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
        if (figure.TilesSelectionSettings != null)
        {
            figure.TilesSelectionSettings = Object.Instantiate(figure.TilesSelectionSettings);
        }
    }

    private void SpawnUI()
    {
        if (figure.UIPrefab  != null)
        {
            figure.UIController = Object.Instantiate(figure.UIPrefab , figure.transform.position, Quaternion.identity, figure.transform);
        }
    }
}