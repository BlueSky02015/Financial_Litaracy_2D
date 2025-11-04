using UnityEngine;

/// <summary>
/// Attach this to each app button (e.g., EmailButton, BrowserButton).
/// When clicked, it tells UIManager to open the associated app.
/// </summary>
public class AppOpener : MonoBehaviour
{
    [Tooltip("The app UI canvas to open")]
    public GameObject appCanvas;

    void Start()
    {

        var button = GetComponent<UnityEngine.UI.Button>();
        if (button != null && button.onClick.GetPersistentEventCount() == 0)
        {
            button.onClick.AddListener(OnAppButtonClick);
        }
    }

    // Call this from UI Button's OnClick()
    public void OnAppButtonClick()
    {
        if (appCanvas == null)
        {
            Debug.LogError("AppCanvas not assigned on " + name, this);
            return;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.OpenApp(appCanvas);
            Debug.Log("Opening app: " + appCanvas.name);
        }
        else
        {
            Debug.LogError("UIManager not found!");
        }
    }
}