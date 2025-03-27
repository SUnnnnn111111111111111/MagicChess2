using UnityEngine;
using TMPro;
using DG.Tweening;

public class FigureUIController : MonoBehaviour
{
    public TMP_Text movesText;
    public CanvasGroup canvasGroup;

    public bool isHiddenOnStart;
    public float fadeInDuration = 0.5f;
    public float visibleDuration = 1.0f;
    public float fadeOutDuration = 0.5f;

    private int lastCount = -1;

    private void Awake()
    {
        if (canvasGroup != null && isHiddenOnStart)
        {
            canvasGroup.alpha = 0;
        }
    }

    public void UpdateCount(int currentCount, int maxCount)
    {
        if (currentCount == lastCount)
            return;

        lastCount = currentCount;
        movesText.text = $"{currentCount}/{maxCount}";

        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true));
        seq.AppendInterval(visibleDuration).SetUpdate(true);
        seq.Append(canvasGroup.DOFade(0f, fadeOutDuration).SetUpdate(true));
    }
}