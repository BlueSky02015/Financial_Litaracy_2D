using UnityEngine;
using TMPro;
using System.Collections;

public class WheelFeedback : MonoBehaviour
{
    [Header("Fade System")]
    [SerializeField] private WheelFadeTrigger fadeTrigger;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private WheelSelection wheelSelection;
    [SerializeField] private GameObject wheelUI;

    [Header("Text Display")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private float displayDuration = 3f;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    public void ShowFeedback(string description, int health, int hunger, int stamina, int mood, int knowledge, int money,  Color textColor)
    {
        if (fadeTrigger == null || descriptionText == null || statsText == null)
        {
            Debug.LogError("WheelFeedback: Missing references!", this);
            return;
        }

        // Play SFX
        if (audioManager != null)
            audioManager.playSFX(audioManager.Click_SFX, 1.5f);

        // Set text
        descriptionText.color = textColor;
        statsText.color = textColor;
        descriptionText.text = description;
        statsText.text = FormatStats(health, hunger, stamina, mood, knowledge, money);

        // Start fade + display sequence
        StartCoroutine(ShowFeedbackSequence());
    }

    private IEnumerator ShowFeedbackSequence()
    {
        if (wheelSelection != null)
            wheelSelection.SetInteractable(false);
            wheelUI.SetActive(false);

        // Fade to black
        yield return StartCoroutine(fadeTrigger.WaitForFadeIn());
        Debug.Log(">>> WheelFeedback: Fade to black complete.");

        // Show text
        descriptionText.gameObject.SetActive(true);
        statsText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        // Hide text
        descriptionText.gameObject.SetActive(false);
        statsText.gameObject.SetActive(false);

        // Fade back in
        yield return StartCoroutine(fadeTrigger.WaitForFadeOut());
        Debug.Log(">>> WheelFeedback: Fade back in complete.");

        if (wheelSelection != null)
            wheelSelection.SetInteractable(true);
    }

    private string FormatStats(int health, int hunger, int stamina, int mood,  int knowledge, int money)
    {
        string s = "";
        if (health != 0) s += $"Health {health:+0;-0} \n";
        if (hunger != 0) s += $"Hunger {hunger:+0;-0} \n";
        if (stamina != 0) s += $"Stamina {stamina:+0;-0} \n";
        if (mood != 0) s += $"Mood {mood:+0;-0} \n";
        if (knowledge != 0) s += $"Knowledge {knowledge:+0;-0} \n";
        if (money != 0) s += $"Money {money:+0;-0} \n";
        return s.Trim();
    }
}