using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public static event System.Action OnGameReady;

    [Header("Panel Canvas")]
    public GameObject GamePauseScreen;
    public GameObject GameCreditScreen;
    public GameObject laptopDesktopCanvas;
    public GameObject appsRoot;

    [SerializeField] private AudioManager audioManager;

    private UIState currentState = UIState.Playing;

    public enum UIState
    {
        Playing,
        Paused,
        InCredits,
        InLaptopDesktop,
        InLaptopApp
    }

    private bool IsAnyAppOpen()
    {
        if (appsRoot == null)
            return false;

        foreach (Transform child in appsRoot.transform)
        {
            if (child.gameObject.activeInHierarchy)
                return true;
        }
        return false;
    }

    void Awake()
    {
        HandleSingleton();

        if (instance == this)
            InitializeGameState();
    }

    private void InitializeGameState()
    {
        GameCreditScreen.SetActive(false);
        GamePauseScreen.SetActive(false);
        if (laptopDesktopCanvas != null)
            laptopDesktopCanvas.SetActive(false);
    }

    private void HandleSingleton()
    {
        if (instance != null)
        {
            // Check if the old instance is still valid (not destroyed)
            if (instance != this && instance.gameObject != null)
            {
                // Destroy this new instance
                Destroy(gameObject);
                return;
            }
        }
        
        // Set this as the instance
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        HandleInput();
    }

    // auto-sync state
    private void UpdateUIStateFromCanvas()
    {
        if (laptopDesktopCanvas != null && laptopDesktopCanvas.activeInHierarchy)
        {
            currentState = IsAnyAppOpen() ? UIState.InLaptopApp : UIState.InLaptopDesktop;
        }
        else if (GameCreditScreen.activeInHierarchy)
        {
            currentState = UIState.InCredits;
        }
        else if (GamePauseScreen.activeInHierarchy)
        {
            currentState = UIState.Paused;
        }
        else
        {
            currentState = UIState.Playing;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            audioManager.playSFX(audioManager.Setting_SFX, 1.2f);
            HandleEscapeKey();
        }
    }

    private void HandleEscapeKey()
    {
        switch (currentState)
        {
            case UIState.Playing: PauseGame(); break;
            case UIState.Paused: ResumeGame(); break;
            case UIState.InCredits: HideCreditScreen(); break;
            case UIState.InLaptopDesktop: ExitLaptop(); break;
            case UIState.InLaptopApp: CloseCurrentApp(); break;
        }
    }

    //------------------------------- Laptop/App Management -------------------------------

    // Called when laptop desktop is opened (by InteractableHandler)
    public void OnLaptopDesktopOpened()
    {
        currentState = UIState.InLaptopDesktop;
    }

    // Called when an app is opened
    public void OnAppOpened()
    {
        currentState = UIState.InLaptopApp;
    }

    public void OpenApp(GameObject appCanvas)
    {
        if (appCanvas == null)
        {
            Debug.LogWarning("Tried to open a null app canvas!");
            return;
        }

        currentState = UIState.InLaptopApp;
    }


    // Called when closing app → back to desktop
    private void CloseCurrentApp()
    {
        currentState = UIState.InLaptopDesktop;
    }

    public void ExitLaptop()
    {
        laptopDesktopCanvas?.SetActive(false);

        // Close all apps
        if (appsRoot != null)
        {
            UIPanelManager.instance.CloseAllPanels();
        }

        currentState = UIState.Playing;
        Time.timeScale = 1;
    }

    //------------------------------- Pause/Credit Management -------------------------------
    private void PauseGame()
    {
        GamePauseScreen.SetActive(true);
        currentState = UIState.Paused;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        GamePauseScreen.SetActive(false);
        currentState = UIState.Playing;
        Time.timeScale = 1;
    }

    public void ShowCreditScreen()
    {
        GameCreditScreen.SetActive(true);
        GamePauseScreen.SetActive(false);
        currentState = UIState.InCredits;
    }

    private void HideCreditScreen()
    {
        GameCreditScreen.SetActive(false);
        GamePauseScreen.SetActive(true);
        currentState = UIState.Paused;
    }

    // Reset only tutorial data
    public void ResetTutorialProgress()
    {
        PlayerPrefs.DeleteKey("CompletedAppTutorials");
        Debug.Log("✅ Tutorial progress reset!");
    }

    
    public static void NotifyGameReady()
    {
        OnGameReady?.Invoke();
    }
}