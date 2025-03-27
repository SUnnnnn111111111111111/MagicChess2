using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameStateUIController : MonoBehaviour
{
    public TMP_Text animatedGameStateText;
    public TMP_Text persistentGameStateText;

    public CanvasGroup animatedCanvasGroup;

    public float fadeInDuration = 0.5f;
    public float visibleDuration = 2.0f;
    public float fadeOutDuration = 0.5f;

    private void Start()
    {
        if (animatedCanvasGroup == null)
        {
            animatedCanvasGroup = GetComponent<CanvasGroup>();
            if (animatedCanvasGroup == null)
            {
                animatedCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        animatedCanvasGroup.alpha = 0;
    }

    public void UpdateGameState(GameStateManager.GameState newState)
    {
        string displayText = "";
        switch (newState)
        {
            case GameStateManager.GameState.WhitesPlaying:
                displayText = "Ход белых";
                break;
            case GameStateManager.GameState.BlacksPlaying:
                displayText = "Ход черных";
                break;
            case GameStateManager.GameState.Paused:
                displayText = "Пауза";
                break;
            case GameStateManager.GameState.WhitesLost:
                displayText = "Белые проиграли";
                break;
            case GameStateManager.GameState.BlacksLost:
                displayText = "Чёрные проиграли";
                break;
            default:
                displayText = "";
                break;
        }

        if (animatedGameStateText != null)
        {
            animatedGameStateText.text = displayText;
        }
        if (persistentGameStateText != null)
        {
            persistentGameStateText.text = displayText;
        }

        animatedCanvasGroup.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(animatedCanvasGroup.DOFade(1f, fadeInDuration).SetUpdate(true));
        seq.AppendInterval(visibleDuration).SetUpdate(true);
        seq.Append(animatedCanvasGroup.DOFade(0f, fadeOutDuration).SetUpdate(true));
    }
}
