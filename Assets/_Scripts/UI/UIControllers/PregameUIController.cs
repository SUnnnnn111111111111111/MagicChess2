using TMPro;
using UnityEngine;

public class PregameUIController : MonoBehaviour
{
    [SerializeField] private GameObject choiceAIText;
    [SerializeField] private GameObject settingsUI;
    
    private void Awake()
    {
        if (choiceAIText != null)
            choiceAIText.gameObject.SetActive(false);
        if (settingsUI != null)
            settingsUI.gameObject.SetActive(false);
    }

    public void ShowChoiceAI(bool show)
    {
        if (choiceAIText == null) return;
        choiceAIText.gameObject.SetActive(show);
    }

    public void ShowSettings(bool show)
    {
        if (settingsUI == null) return;
        settingsUI.gameObject.SetActive(show);
    }
}
