using UnityEngine;
using TMPro;
using DG.Tweening;

public class FigureUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text movesText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private bool isHiddenOnStart = true;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float visibleDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private int lastCount = -1;

    private void Awake()
    {
        if (canvasGroup != null && isHiddenOnStart)
        {
            canvasGroup.alpha = 0;
        }
    }
    
    public void UpdateMoveCount(int currentCount, int maxCount)
    {
        if (currentCount == lastCount)
            return;

        lastCount = currentCount;
        
        if(movesText != null)
            movesText.text = $"{currentCount}/{maxCount}";

        if(canvasGroup == null)
            return;

        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true));
        seq.AppendInterval(visibleDuration).SetUpdate(true);
        seq.Append(canvasGroup.DOFade(0f, fadeOutDuration).SetUpdate(true));
    }
}