using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameStateUI : MonoBehaviour
{
    // Ссылка на UI-текст, который будет отображать состояние
    public TMP_Text gameStateText;
    // Компонент CanvasGroup для управления прозрачностью
    public CanvasGroup canvasGroup;

    // Параметры анимации
    public float fadeInDuration = 0.5f;
    public float visibleDuration = 2.0f;
    public float fadeOutDuration = 0.5f;

    private void Start()
    {
        // Если CanvasGroup не назначен, пытаемся получить его
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        // Сначала делаем UI невидимым
        canvasGroup.alpha = 0;

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged.AddListener(UpdateGameStateText);
            // Обновляем UI сразу при запуске
            UpdateGameStateText(GameStateManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogWarning("GameStateManager не найден!");
        }
    }

    private void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnGameStateChanged.RemoveListener(UpdateGameStateText);
        }
    }

    // Метод-обработчик, обновляющий текст и запускающий анимацию появления и исчезновения
    private void UpdateGameStateText(GameStateManager.GameState newState)
    {
        if (gameStateText != null)
        {
            gameStateText.text = newState.ToString();
        }
        // Останавливаем любые предыдущие анимации CanvasGroup
        canvasGroup.DOKill();

        // Создаем последовательность: fade in, задержка, fade out
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeInDuration));
        seq.AppendInterval(visibleDuration);
        seq.Append(canvasGroup.DOFade(0f, fadeOutDuration));
    }
}
