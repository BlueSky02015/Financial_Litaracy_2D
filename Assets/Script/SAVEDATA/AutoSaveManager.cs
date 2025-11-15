using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    public static AutoSaveManager instance;

    [Header("Auto-Save Settings")]
    [Tooltip("Auto-save every X in-game minutes")]
    public float autoSaveIntervalMinutes = 5f;

    private float lastAutoSaveTime = -1f;
    private float intervalSeconds => autoSaveIntervalMinutes * 60f;

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
        // Initialize to current game time
        if (Clock.instance != null)
            lastAutoSaveTime = Clock.instance.elapsedTime;
    }

    void Update()
    {
        if (Clock.instance == null) return;

        float currentTime = Clock.instance.elapsedTime;
        if (currentTime < lastAutoSaveTime) return; // time went backward (rare)

        if (currentTime - lastAutoSaveTime >= intervalSeconds)
        {
            PerformAutoSave();
            lastAutoSaveTime = currentTime;
        }
    }

    public void PerformAutoSave()
    {
        Debug.Log("🔄 Auto-saving game...");
        GameSaveManager.instance?.SaveGame();
    }

    // Optional: Force auto-save (e.g., before quitting)
    public void ForceAutoSave()
    {
        PerformAutoSave();
    }
}