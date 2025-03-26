using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.75f;

    public void SetProgress(float progress)
    {
        progressBar.value = progress;
        progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    public IEnumerator FadeOutAndDestroy()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}