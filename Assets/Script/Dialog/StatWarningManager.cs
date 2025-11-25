// StatWarningManager.cs
using System.Collections.Generic;
using UnityEngine;

public class StatWarningManager : MonoBehaviour
{
    public static StatWarningManager instance;

    [System.Serializable]
    public class StatWarning
    {
        public StatType statType;
        public float threshold = 50f; // Below this = warning
        public DialogueSO warningDialogue;
        public bool hasWarned = false; // Don't warn twice
    }

    [Header("Warning Settings")]
    public List<StatWarning> statWarnings = new List<StatWarning>();

    [Header("Cooldown")]
    public float warningCooldown = 300f; // 5 minutes between warnings (in seconds)

    private float lastWarningTime = -1f;

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

    void Start()
    {
        lastWarningTime = Clock.instance?.elapsedTime ?? 0f;
    }

    void Update()
    {
        CheckStatWarnings();
    }

    void CheckStatWarnings()
    {
        if (Clock.instance == null) return;

        float currentTime = Clock.instance.elapsedTime;
        if (currentTime - lastWarningTime < warningCooldown) return;

        foreach (var warning in statWarnings)
        {
            if (warning.warningDialogue != null && !warning.hasWarned)
            {
                float currentStat = PlayerStats.instance.GetStat(warning.statType);
                
                if (currentStat < warning.threshold)
                {
                    ShowWarning(warning);
                    warning.hasWarned = true;
                    lastWarningTime = currentTime;
                    break; // Show one warning at a time
                }
            }
        }
    }

    void ShowWarning(StatWarning warning)
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialogue(warning.warningDialogue);
            Debug.Log($"⚠️ Warning shown for {warning.statType} (current: {PlayerStats.instance.GetStat(warning.statType):F0}, threshold: {warning.threshold:F0})");
        }
    }

    // Public method to reset warnings (e.g., when player improves stats)
    public void ResetWarning(StatType statType)
    {
        var warning = statWarnings.Find(w => w.statType == statType);
        if (warning != null)
        {
            warning.hasWarned = false;
            Debug.Log($"🔄 {statType} warning reset");
        }
    }

    // Reset all warnings (e.g., new game)
    public void ResetAllWarnings()
    {
        foreach (var warning in statWarnings)
        {
            warning.hasWarned = false;
        }
        Debug.Log("🔄 All stat warnings reset");
    }
}