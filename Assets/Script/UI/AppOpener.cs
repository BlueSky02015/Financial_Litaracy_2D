using UnityEngine;

public class AppOpener : MonoBehaviour
{
    [Tooltip("The app UI canvas to open")]
    public GameObject appCanvas;
    public string appTag;

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
            return;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.OpenApp(appCanvas);
            TutorialManager.instance?.OnAppClicked(appTag);
            UIPanelManager.instance?.OpenPanel(appTag);
        }
        else
        {
            Debug.LogError("UIManager not found!");
        }
    }
}