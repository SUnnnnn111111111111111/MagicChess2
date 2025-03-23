using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{
    public bool isWall;
    public bool isSideEventTriggering;
    public bool isMiddleEventTriggering;
    public bool isAPawnMovementPromotion;
    public bool isAPawnMovementRandomPromotion;
    
    public Vector2Int Position { get; private set; }
    public List<Tile> Neighbors { get; private set; } = new List<Tile>();
    public Figure OccupyingFigure { get; private set; } 
    public bool IsHighlighted { get; private set; }
    public bool HiddenByFog { get; private set; }
    
    [SerializeField] private GameObject fogOfWarEffect;
    [SerializeField] private GameObject highlightEmptyTile;
    [SerializeField] private GameObject highlightEnemyTile;
    [SerializeField] private GameObject pawnMovementPromotionIcon;
    [SerializeField] private GameObject pawnMovementRandomPromotionIcon;
    
    

    private void Awake()
    {
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        TilesRepository.Instance.RegisterTile(this, Position);

        if (highlightEmptyTile != null)
        {
            highlightEmptyTile.SetActive(false);
        }
        SetHiddenByFog(true);
        UpdatePawnPromotionIcons();
    }

    public void SetNeighbors(List<Tile> neighbors)
    {
        Neighbors = neighbors;
    }

    public List<Tile> GetNeighbors(NeighborTilesSelectionSettings settings)
    {
        List<Tile> result = new List<Tile>();
        foreach (var offset in settings.GetOffsets())
        {
            Tile neighbor = TilesRepository.Instance.GetTileAt(Position + offset);
            if (neighbor != null)
                result.Add(neighbor);
        }
        return result;
    }

    public GameObject GetAvailableHighlightObject()
    {
        return highlightEmptyTile;
    }

    public GameObject GetEnemyHighlightObject()
    {
        return highlightEnemyTile;
    }

    public void SetHighlighted(bool state)
    {
        if (highlightEmptyTile != null)
        {
            highlightEmptyTile.SetActive(state);
            IsHighlighted = state;
        }
    }

    public void SetOccupyingFigure(Figure figure)
    {
        OccupyingFigure = figure;
    }
    
    public void SetHiddenByFog(bool value)
    {
        HiddenByFog = value;
        if (fogOfWarEffect != null)
            fogOfWarEffect.SetActive(value);
    }
    
    public void UpdatePawnPromotionIcons()
    {
        if (pawnMovementRandomPromotionIcon != null)
        {
            pawnMovementRandomPromotionIcon.SetActive(isAPawnMovementRandomPromotion);
        }
        if (pawnMovementPromotionIcon != null)
        {
            pawnMovementPromotionIcon.SetActive(isAPawnMovementPromotion);
        }
    }

    private void OnMouseDown()
    {
        if (HiddenByFog) return;
        
        if (SelectedFigureManager.Instance.SelectedFigure != null)
        {
            FigureMover mover = SelectedFigureManager.Instance.SelectedFigure.GetComponent<FigureMover>();
            if (mover != null)
            {
                mover.MoveToTile(this);
            }
        }
    }
}

