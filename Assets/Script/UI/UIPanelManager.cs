using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    public static UIPanelManager instance;

    [System.Serializable]
    public class PanelConfig
    {
        public string panelName;
        public GameObject panel;
        public float fadeDuration = 0.3f;
    }

    [Header("Panel Settings")]
    public List<PanelConfig> panels = new List<PanelConfig>();
    private Dictionary<string, PanelConfig> panelMap = new Dictionary<string, PanelConfig>();

    [Header("Settings")]
    public bool closeOtherPanels = true; // auto-close others when opening one

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        // Build lookup map
        panelMap.Clear();
        foreach (var config in panels)
        {
            if (config.panel == null) continue;

            // Ensure CanvasGroup exists
            if (config.panel.GetComponent<CanvasGroup>() == null)
            {
                config.panel.AddComponent<CanvasGroup>();
            }

            // Start hidden
            SetPanelVisible(config.panel, false, 0f);

            panelMap[config.panelName] = config;
        }
    }

    // --- Public API ---
    public void OpenPanel(string panelName)
    {
        if (!panelMap.ContainsKey(panelName))
        {
            Debug.LogWarning($"Panel not found: {panelName}");
            return;
        }

        var config = panelMap[panelName];

        // Close others if needed
        if (closeOtherPanels)
        {
            CloseAllPanelsExcept(panelName);
        }

        // Open with fade
        StartCoroutine(FadePanel(config.panel, true, config.fadeDuration));
    }

    public void ClosePanel(string panelName)
    {
        if (panelMap.ContainsKey(panelName))
        {
            var config = panelMap[panelName];
            StartCoroutine(FadePanel(config.panel, false, config.fadeDuration));
        }
    }

    public void CloseAllPanels()
    {
        foreach (var config in panels)
        {
            if (config.panel != null)
            {
                SetPanelVisible(config.panel, false, 0f);
            }
        }
    }

    // --- Private Helpers ---
    void CloseAllPanelsExcept(string panelToKeep)
    {
        foreach (var config in panels)
        {
            if (config.panelName == panelToKeep) continue;
            if (config.panel != null)
            {
                SetPanelVisible(config.panel, false, 0f);
            }
        }
    }

    void SetPanelVisible(GameObject panel, bool visible, float alpha)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = alpha;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
        else
        {
            panel.SetActive(visible);
        }
    }

    IEnumerator FadePanel(GameObject panel, bool show, float duration)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) yield break;

        float startAlpha = cg.alpha;
        float endAlpha = show ? 1f : 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // works when paused
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
        cg.interactable = show;
        cg.blocksRaycasts = show;
    }
}