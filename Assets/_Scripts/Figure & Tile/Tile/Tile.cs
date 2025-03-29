using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(TileHoverHandler))]
[RequireComponent(typeof(TileHighlightVisuals))]
[RequireComponent(typeof(TileFogVisuals))]
[RequireComponent(typeof(TileIconController))]
public class Tile : MonoBehaviour
{
    [Header("Tile Properties")]
    public bool isWall;
    public bool isSideEventTriggering;
    public bool isMiddleEventTriggering;
    public bool isAPawnMovementPromotion;
    public bool isAPawnMovementRandomPromotion;

    [Header("State")]
    public Vector2Int Position { get; private set; }
    public List<Tile> Neighbors { get; private set; } = new();
    public Figure OccupyingFigure { get; private set; }
    public bool IsHighlighted { get; private set; }
    public bool HiddenByFog { get; private set; }
    

    private void Awake()
    {
        InitializePosition();
        TilesRepository.Instance.RegisterTile(this, Position);
        SetHiddenByFog(true);
    }

    private void InitializePosition()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            transform.position.y,
            Mathf.Round(transform.position.z)
        );

        Position = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)
        );
    }

    public void SetPosition(Vector3 newPosition)
    {
        newPosition.y = transform.position.y;
        newPosition.x = Mathf.Round(newPosition.x);
        newPosition.z = Mathf.Round(newPosition.z);
        transform.position = newPosition;
    }

    public void SetNeighbors(List<Tile> neighbors)
    {
        Neighbors = neighbors ?? new List<Tile>();
    }

    public List<Tile> GetNeighbors(NeighborTilesSelectionSettings settings)
    {
        if (settings == null) return new List<Tile>();

        var offsets = settings.GetOffsets();
        List<Tile> result = new List<Tile>();

        foreach (var offset in offsets)
        {
            Tile neighbor = TilesRepository.Instance.GetTileAt(Position + offset);
            if (neighbor != null)
                result.Add(neighbor);
        }

        return result;
    }

    public void SetOccupyingFigure(Figure figure)
    {
        OccupyingFigure = figure;
    }

    public void SetHiddenByFog(bool value)
    {
        HiddenByFog = value;

        var fogVisuals = GetComponent<TileFogVisuals>();
        if (fogVisuals != null)
        {
            fogVisuals.SetFog(value);
        }
    }

    public void SetHighlighted(bool state)
    {
        IsHighlighted = state;
    }

    public void Clear()
    {
        SetOccupyingFigure(null);
        SetHiddenByFog(true);
        SetHighlighted(false);
    }
}
