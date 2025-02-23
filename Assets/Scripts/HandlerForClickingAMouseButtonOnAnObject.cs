using UnityEngine;
using UltEvents;

public class HandlerForClickingAMouseButtonOnAnObject : MonoBehaviour
{
    public bool isActive;
    [SerializeField] private UltEvent WhenClickOnAnObject;
    private Figure figure;

    private void Start()
    {
        figure = GetComponentInParent<Figure>(); // 🟢 Ищем Figure у родителя

        if (figure == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} (HandlerForClickingAMouseButtonOnAnObject) не нашёл Figure!");
        }
    }

    private void OnMouseDown()
    {
        if (!isActive) return;

        WhenClickOnAnObject.Invoke();

        if (figure != null)
        {
            GameManager.Instance.SelectedFigure = figure;
            figure.HighlightAvailableMoves();
        }
        else
        {
            Debug.LogWarning("⚠️ Клик по объекту, но фигура не найдена!");
        }
    }
}
