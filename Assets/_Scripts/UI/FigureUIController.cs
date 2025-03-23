using UnityEngine;
using TMPro;
using DG.Tweening;

public class FigureUIController : MonoBehaviour
{
    public TMP_Text movesText;          // Ссылка на текстовый компонент
    public CanvasGroup canvasGroup;     // Для управления прозрачностью

    public float fadeInDuration = 0.5f;
    public float visibleDuration = 1.0f;
    public float fadeOutDuration = 0.5f;

    private int lastCount = -1;
    
    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0; // UI изначально скрыт
    }
    
    /// <summary>
    /// Вызывайте этот метод при изменении countOfMovesIsOnEventTriggeringTile.
    /// </summary>
    public void UpdateCount(int currentCount, int maxCount)
    {
        // Если значение не изменилось, можно не обновлять UI
        if (currentCount == lastCount)
            return;
    
        lastCount = currentCount;
        movesText.text = $"{currentCount}/{maxCount}"; // формат "текущее значение/максимум"

        // Останавливаем любые запущенные анимации и запускаем новую последовательность
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true));
        seq.AppendInterval(visibleDuration).SetUpdate(true);
        seq.Append(canvasGroup.DOFade(0f, fadeOutDuration).SetUpdate(true));
    }
}