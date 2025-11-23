// SceneLoader.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [Header("Scene Names")]
    public string gameCoreScene = "MainScene";
    public string mainMenuScene = "MainMenu";


    public void LoadGameScene()
    {

        StartCoroutine(UnloadSceneAsync(mainMenuScene));
        UnloadGameScenes();

        // Load core systems FIRST
        StartCoroutine(LoadSceneAsync(gameCoreScene, OnCoreLoaded));
    }

    void OnCoreLoaded()
    {
        Debug.Log("✅ GameCore loaded. Initializing systems...");

        // Optional: Load player data
        if (GameSaveManager.instance != null)
        {
            GameSaveManager.instance.LoadGame();
        }
    }

    void OnUILoaded()
    {
        Debug.Log("✅ GameUI loaded. Ready to play!");
        UIManager.NotifyGameReady();
    }

    IEnumerator LoadSceneAsync(string sceneName, System.Action onLoaded)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading {sceneName}: {asyncLoad.progress:P0}");
            yield return null;
        }

        onLoaded?.Invoke();
    }

    public void UnloadGameScenes()
    {
        if (SceneManager.GetSceneByName(gameCoreScene).isLoaded)
            SceneManager.UnloadSceneAsync(gameCoreScene);
    }

    IEnumerator UnloadSceneAsync(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"Unloading {sceneName}...");
            yield return SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}