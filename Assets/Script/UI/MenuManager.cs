using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [Header("Scene Settings")]

    [Header("References")]
    [SerializeField] private GameObject settingsPanel;

    public void OnNewGameButton()
    {
        Debug.Log("Starting new game...");
        // Optional: delete save file for fresh start
        // System.IO.File.Delete(System.IO.Path.Combine(Application.persistentDataPath, "game_save.json"));

        GameFlowManager.instance.StartNewGame();
    }

    public void OnLoadGameButton()
    {
        GameFlowManager.instance.LoadGame();
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
