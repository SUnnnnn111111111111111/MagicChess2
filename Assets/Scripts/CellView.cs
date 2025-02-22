using UnityEngine;

public class CellView : MonoBehaviour
{
    public CellData CellData { get; private set; }
    private Renderer cellRenderer;

    void Start()
    {
        cellRenderer = GetComponent<Renderer>();
        UpdateVisuals();
    }

    public void Initialize(CellData cellData)
    {
        CellData = cellData;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (CellData != null && cellRenderer != null)
        {
            cellRenderer.material.color = CellData.IsOccupied ? Color.red : Color.white;
        }
    }

    public void Highlight(bool enable)
    {
        if (cellRenderer != null)
        {
            cellRenderer.material.color = enable ? Color.green : (CellData.IsOccupied ? Color.red : Color.white);
        }
    }
}
