using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeathManager : MonoBehaviour
{
    public static DeathManager Instance;

    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TMP_Text deathTitleText;
    [SerializeField] private TMP_Text debtStatusText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private DeathAnimatorTrigger deathAnimator;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDeathScreen()
    {
        // Get debt information
        float currentDebt = 0f;
        if (DebtSystem.instance != null)
        {
            currentDebt = DebtSystem.instance.GetCurrentDebt();
        }

        // Show death screen with fade
        StartCoroutine(ShowDeathScreenSequence(currentDebt));
    }

    System.Collections.IEnumerator ShowDeathScreenSequence(float debtAmount)
    {
        // Play fade in
        deathAnimator.PlayDeath();
        yield return new WaitForSeconds(fadeDuration * 0.5f); 

        // Show death screen
        deathScreen.SetActive(true);
        
        // Set up text
        deathTitleText.text = "YOU DIED";
        
        // Debt status
        if (debtAmount > 0)
        {
            debtStatusText.text = $"<color=red>DEBT REMAINING: ${debtAmount:F0}</color>\n\n" +
                                "You failed to pay off your debt before your health ran out.";
        }
        else
        {
            debtStatusText.text = "<color=green>DEBT PAID OFF: ✓</color>\n\n" +
                                "You lived a debt-free life, but your health failed you.";
        }

        instructionText.text = "Press SPACE to return to title screen";

        // Wait for fade to complete
        yield return new WaitForSeconds(fadeDuration * 0.5f);
    }

    void Update()
    {
        if (deathScreen != null && deathScreen.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ReturnToTitle();
            }
        }
    }

    void ReturnToTitle()
    {
        // Save game first (optional)
        if (GameSaveManager.instance != null)
        {
            GameSaveManager.instance.SaveGame();
        }

        // Reset all systems
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.SetCompletedTutorials(new System.Collections.Generic.HashSet<string>());
        }

        // close death screen
        deathScreen.SetActive(false);

        // Load title scene
        GameFlowManager.instance.EnterTitleScreen();
    }
}