// GameFlowManager.cs (canvas-only version)
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager instance;

    [Header("UI Panels")]

    public GameObject titleScreen;      // Title screen canvas panel
    public GameObject disclaimerPanel;  // Disclaimer panel
    public GameObject inGameObjects;    // Player, interactables
    private bool isGameActive = false;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // START IN TITLE SCREEN MODE
        EnterTitleScreen();
    }
    public void EnterTitleScreen()
    {
        isGameActive = false;
        Time.timeScale = 0f;

        titleScreen.SetActive(true);
        inGameObjects.SetActive(false);
        if (disclaimerPanel != null) disclaimerPanel.SetActive(false);

        Debug.Log("🎬 Title screen active");
    }

    public void StartNewGame()
    {
        // ✅ RESET ALL SYSTEMS TO DEFAULTS
        PlayerStats.instance?.ResetToDefaults();
        Clock.instance?.MorningTime();
        InvestmentManager.instance?.ResetHoldings();
        DebtSystem.instance?.ResetDebt();
        StatTimeUpdater.instance?.OnNewGameStarted();
        StatWarningManager.instance?.ResetAllWarnings();

        ShowDisclaimerThenStart();
        Invoke(nameof(StartGameplay), 7f);

    }

    public void LoadGame()
    {
        bool hasSave = GameSaveManager.instance.LoadGame();
        if (!hasSave)
        {
            TutorialManager.instance.StartNewGame();
        }
        else
        {
            TutorialManager.instance.LoadTutorialProgress();
        }

        ShowDisclaimerThenStart();
    }

    void ShowDisclaimerThenStart()
    {
        // Hide title screen
        titleScreen.SetActive(false);

        // Show disclaimer panel if it exists
        if (disclaimerPanel != null)
        {
            disclaimerPanel.SetActive(true);
        }

        Time.timeScale = 1f; // Time runs during disclaimer

        // Start disclaimer manager to handle timing
        DisclaimerManager disclaimer = disclaimerPanel?.GetComponent<DisclaimerManager>();
        if (disclaimer != null)
        {
            disclaimer.StartCountdown(() => StartGameplay());
        }
        else
        {
            // Fallback: start gameplay after  seconds without manager
            Invoke(nameof(StartTutorial), 7f);
        }
    }

    void StartGameplay()
    {
        // Hide disclaimer
        if (disclaimerPanel != null)
        {
            disclaimerPanel.SetActive(false);
        }

        // Show game UI
        inGameObjects.SetActive(true);
        Time.timeScale = 1f;
        isGameActive = true;

        Debug.Log("🎮 Game started");
    }

    public void QuitToTitle()
    {
        GameSaveManager.instance.SaveGame();

        if (disclaimerPanel != null) disclaimerPanel.SetActive(false);
        inGameObjects.SetActive(false);

        isGameActive = false;
        Time.timeScale = 0f;

        titleScreen.SetActive(true);

        Debug.Log("🏠 Returned to title");
    }

    void StartTutorial()
    {
        TutorialManager.instance.StartNewGame();
    }


}