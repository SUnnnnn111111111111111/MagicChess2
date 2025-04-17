using UnityEngine;
using System;

public class ClickHandler : MonoBehaviour
{
    public static event Action<Figure> FigureClicked;
    public static event Action<Tile> TileClicked;
    
    private void OnMouseDown()
    {
        Figure figure = GetComponentInParent<Figure>();
        if (figure != null)
        {
            FigureClicked?.Invoke(figure);
            return;
        }
        
        Tile tile = GetComponentInParent<Tile>();
        if (tile != null)
        {
            TileClicked?.Invoke(tile);
            return;
        }
    }
}