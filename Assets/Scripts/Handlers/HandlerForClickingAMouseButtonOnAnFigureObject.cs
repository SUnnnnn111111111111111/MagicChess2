using UnityEngine;
using UltEvents;

public class HandlerForClickingAMouseButtonOnAnFigureObject : MonoBehaviour
{
    public bool isActive;
    [SerializeField] private UltEvent WhenClickOnAnObject;
    private Figure figure;

    private void Start()
    {
        figure = GetComponentInParent<Figure>();

        if (figure == null)
        {
            // Debug.LogWarning($" {gameObject.name} (HandlerForClickingAMouseButtonOnAnObject) не нашёл Figure!");
        }
    }

    private void OnMouseDown()
    {
        if (!isActive) return;

        WhenClickOnAnObject.Invoke();

        if (figure != null)
        {
            if (GameManager.Instance.SelectedFigure == figure)
            {
                GameManager.Instance.ResetSelection();
            }
            else
            {
                GameManager.Instance.SelectedFigure = figure;
            }
        }
    }
}