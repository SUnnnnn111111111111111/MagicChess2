using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private Tile tile;

    private void Awake()
    {
        tile = GetComponentInParent<Tile>();
    }

    private void OnMouseDown()
    {
        if (tile.HiddenByFog)
            return;

        var selectedFigure = SelectedFigureManager.Instance.SelectedFigure;
        if (selectedFigure == null)
            return;

        var mover = selectedFigure.GetComponent<FigureMover>();
        if (mover != null)
        {
            mover.TryMoveToTile(tile);
        }
    }
}