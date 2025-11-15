using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "MainScene";

    [Header("References")]
    [SerializeField] private GameObject settingsPanel;

    public void OnNewGameButton()
    {
        Debug.Log("Starting new game...");
        // Optional: delete save file for fresh start
        // System.IO.File.Delete(System.IO.Path.Combine(Application.persistentDataPath, "game_save.json"));

        SceneManager.LoadScene(gameSceneName);
        TutorialManager.instance.StartNewGame();
    }

    public void OnLoadGameButton()
    {
        // Safe call
        bool hasSave = GameSaveManager.instance?.LoadGame() ?? false;
        if (hasSave)
            SceneManager.LoadScene("MainScene");
        else
            OnNewGameButton(); // fallback to new game
    }

    public void OnSettingsButton()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("SettingsPanel not assigned!");
        }
    }
    public void OnCloseSettingsButton()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            UIManager.instance.ResumeGame();

        }
    }

    public void OnQuitButton()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
