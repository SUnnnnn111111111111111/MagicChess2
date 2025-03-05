using UnityEngine;
using System;

public class FigureInitialization : MonoBehaviour
{
    public event Action<Tile> OnInitializationCompleted; // Событие завершения инициализации

    private void Start()
    {
        // Даем время BoardManager зарегистрировать все клетки перед поиском
        Invoke(nameof(LateStart), 0.1f);
    }

    private void LateStart()
    {
        Tile tile = BoardManager.Instance.GetTileAt(new Vector2Int((int)transform.position.x, (int)transform.position.z));

        if (tile != null)
        {
            tile.SetOccupied(true);
            OnInitializationCompleted?.Invoke(tile);
        }
        else
        {
            Debug.LogWarning($"⚠️ Фигура {gameObject.name} не нашла свою текущую клетку!");
        }
    }
}