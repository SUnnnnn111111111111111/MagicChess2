using UnityEngine;
using UltEvents;
using System.Linq;

public class FigureClickHandler : MonoBehaviour
{
    public bool isActive;
    private Figure figure;

    private void Start()
    {
        figure = GetComponentInParent<Figure>();

        if (figure == null)
        {
            Debug.LogWarning($" {gameObject.name} (HandlerForClickingAMouseButtonOnAnObject) не нашёл Figure!");
        }
    }

    private void OnMouseDown()
    {
        if (!FigureSelector.CanSelect(figure)) return;

        figure.GetComponent<FigureLogic>().HighlightAvailableToMoveTiles(includeFog: false);

        if (figure != null)
        {
            if (SelectedFigureManager.Instance.SelectedFigure == figure)
            {
                SelectedFigureManager.Instance.ResetSelection();
            }
            else
            {
                SelectedFigureManager.Instance.SelectedFigure = figure;
            }
        }
    }
}