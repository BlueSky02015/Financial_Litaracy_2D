using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Intro Dialogue")]
    public DialogueSO introDialogue;

    [System.Serializable]
    public class AppTutorial
    {
        public string appTag;
        public DialogueSO firstClickDialogue;
    }

    [Header("App Tutorials")]
    public List<AppTutorial> appTutorials = new List<AppTutorial>();

    private HashSet<string> completedTutorials = new HashSet<string>();
    private const string APP_TUTORIALS_KEY = "CompletedTutorials";
    private bool isTutorialActive = false;


    void Awake()
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

    public void StartNewGame()
    {
        Debug.Log("🚀 Starting new game tutorial");
        ResetAppTutorials();
        if (introDialogue != null && DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialogue(introDialogue);
        }
        else
        {
            Debug.Log("No intro dialogue assigned");
        }
    }

    public void OnAppClicked(string appTag)
    {
        // Skip if already shown
        if (completedTutorials.Contains(appTag))
            return;

        // Find dialogue for this app
        var tutorial = appTutorials.Find(t => t.appTag == appTag);
        if (tutorial?.firstClickDialogue != null)
        {
            DialogManager.Instance.StartDialogue(tutorial.firstClickDialogue);
            completedTutorials.Add(appTag);
            SaveTutorialProgress();
        }
    }

    public bool IsAppTutorialCompleted(string appTag)
    {
        return completedTutorials.Contains(appTag);
    }

    public void MarkAppTutorialCompleted(string appTag)
    {
        completedTutorials.Add(appTag);
        SaveTutorialProgress();
        Debug.Log($"✅ Marked app tutorial completed: {appTag}");
    }

    // --- Save/Load ---
    void SaveTutorialProgress()
    {
        // Convert to JSON for PlayerPrefs
        var wrapper = new StringListWrapper(new List<string>(completedTutorials));
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(APP_TUTORIALS_KEY, json);
        PlayerPrefs.Save();
    }

    public void LoadTutorialProgress()
    {
        if (PlayerPrefs.HasKey(APP_TUTORIALS_KEY))
        {
            string json = PlayerPrefs.GetString(APP_TUTORIALS_KEY);
            var wrapper = JsonUtility.FromJson<StringListWrapper>(json);
            completedTutorials = new HashSet<string>(wrapper.items);
            Debug.Log($"✅ Loaded {completedTutorials.Count} completed app tutorials");
        }
        else
        {
            Debug.Log("No saved tutorial progress found");
        }
    }

    private void ResetAppTutorials()
    {
        completedTutorials.Clear();
        PlayerPrefs.DeleteKey(APP_TUTORIALS_KEY);
        Debug.Log("🗑️ Cleared all app tutorial progress");
    }

    // Helper for JSON serialization
    [System.Serializable]
    private class StringListWrapper
    {
        public List<string> items;
        public StringListWrapper(List<string> list) => items = list;
        public StringListWrapper() => items = new List<string>();
    }

    // --- MISC METHODS ---
    public void SetTutorialActive(bool active)
    {
        isTutorialActive = active;
        Debug.Log($"Tutorial active: {active}");
    }

    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }

    public HashSet<string> GetCompletedTutorials()
    {
        return completedTutorials;
    }

    public void SetCompletedTutorials(HashSet<string> tutorials)
    {
        completedTutorials = tutorials ?? new HashSet<string>();
        Debug.Log($"Loaded {completedTutorials.Count} completed app tutorials");
    }

}