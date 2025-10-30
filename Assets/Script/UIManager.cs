using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Panel")]
    public GameObject GamePauseScreen;
    public GameObject GameCreditScreen;
    public bool isPlaying, isPause, isInCredit;

    [SerializeField] private AudioManager audioManager;

    void Awake()
    {
        HandleSingleton();
        
        if (instance == this)
            InitializeGameState();
    }

    private void InitializeGameState()
    {
        isPlaying = true;
        isPause = false;
        isInCredit = false;
        GameCreditScreen.SetActive(false);
        GamePauseScreen.SetActive(false);
    }

    private void HandleSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
        HandleInput();
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
        switch (true)
        {
            case bool _ when isPlaying:
                PauseGame();
                break;

            case bool _ when isPause && !isInCredit:
                ResumeGame();
                break;

            case bool _ when isInCredit:
                HideCreditScreen();
                break;
        }
    }

    private void PauseGame()
    {
        GamePauseScreen.SetActive(true);
        isPause = true;
        isPlaying = false;
        Time.timeScale = 0;
    }

    private void ResumeGame()
    {
        GamePauseScreen.SetActive(false);
        isPause = false;
        isPlaying = true;
        Time.timeScale = 1;
    }

    public void ShowCreditScreen()
    {
        GameCreditScreen.SetActive(true);
        GamePauseScreen.SetActive(false);
        isInCredit = true;
    }

    private void HideCreditScreen()
    {
        GameCreditScreen.SetActive(false);
        GamePauseScreen.SetActive(true);
        isInCredit = false;
    }


    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }

    public void ExitGameFromPauseScreen()
    {
        ExitGame();
    }
}