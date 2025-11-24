using UnityEngine;

public class StatTimeUpdater : MonoBehaviour
{
    public static StatTimeUpdater instance;
    [Header("References")]
    [SerializeField] private Clock clock;
    [SerializeField] private PlayerStats playerStats;

    [Header("Settings")]
    [SerializeField] private float updateIntervalHours = 1f;
    private float lastUpdateTime = -1f;
    private float lastProcessedTime = -1f;
    private float IntervalSeconds => updateIntervalHours * 3600f;
    private bool isStatDecayPaused = false;
    private float timeWhenPaused = 0f;

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
        if (clock == null || playerStats == null)
        {
            Debug.LogError("StatTimeUpdater: Missing Clock or PlayerStats reference!", this);
            enabled = false;
            return;
        }

        lastUpdateTime = clock.elapsedTime;
        lastProcessedTime = clock.elapsedTime;
    }

    void Update()
    {
        if (clock == null || playerStats == null) return;

        float currentTime = clock.elapsedTime;
        if (currentTime <= lastUpdateTime) return;

        if (isStatDecayPaused)
        {
            lastUpdateTime = currentTime;
            return;
        }

        float timeSinceLastProcessed = currentTime - lastProcessedTime;
        if (timeSinceLastProcessed < IntervalSeconds) return;

        float hoursSinceLastProcessed = timeSinceLastProcessed / 3600f;

        // Update regular stats
        playerStats.UpdateStatsPerHour(hoursSinceLastProcessed);

        // Update special conditions
        UpdateSpecialConditions(hoursSinceLastProcessed);

        lastUpdateTime = currentTime;
        lastProcessedTime = currentTime;
    }

    void UpdateSpecialConditions(float hoursPassed)
    {
        // Get current stats
        float hunger = playerStats.GetStat(StatType.Hunger);

        // Check each condition separately (not in switch)
        CheckHealthDecay(hoursPassed, hunger);
        CheckKnowledgeDecay(hoursPassed);
        CheckStaminaRecovery(hoursPassed);
        CheckExtraHungerDecay(hoursPassed);
    }

    // HEALTH DECAY LOGIC
    void CheckHealthDecay(float hoursPassed, float currentHunger)
    {
        int totalDecay = 0;

        // Base decay: 1 per 5 hours
        int baseDecay = Mathf.FloorToInt(hoursPassed / 5f) * 1;

        // Extra decay when hunger <= 20: +5 per 30 minutes
        int extraDecay = 0;
        if (currentHunger <= 20f)
        {
            extraDecay = Mathf.FloorToInt(hoursPassed / 0.5f) * 5;
        }

        totalDecay = baseDecay + extraDecay;

        if (totalDecay > 0)
        {
            playerStats.ModifyStat(StatType.Health, -totalDecay);
            Debug.Log($"🩸 Health decay: -{totalDecay} (base: -{baseDecay}, hunger penalty: -{extraDecay}, hunger: {currentHunger:F0})");
        }
    }

    // KNOWLEDGE DECAY
    void CheckKnowledgeDecay(float hoursPassed)
    {
        float mood = playerStats.GetStat(StatType.Mood);
        if (mood <= 20f)
        {
            int decay = Mathf.FloorToInt(hoursPassed / 3f) * 1;
            if (decay > 0)
            {
                playerStats.ModifyStat(StatType.Knowledge, -decay);
                Debug.Log($"📚 Knowledge decay: -{decay} (mood low: {mood:F0})");
            }
        }
    }

    // STAMINA RECOVERY
    void CheckStaminaRecovery(float hoursPassed)
    {
        float mood = playerStats.GetStat(StatType.Mood);
        if (mood >= 80f)
        {
            int recovery = Mathf.FloorToInt(hoursPassed / 2f) * 1;
            if (recovery > 0)
            {
                playerStats.ModifyStat(StatType.Stamina, recovery);
                Debug.Log($"💪 Stamina recovery: +{recovery} (mood high: {mood:F0})");
            }
        }
    }

    // EXTRA HUNGER DRAIN
    void CheckExtraHungerDecay(float hoursPassed)
    {
        float health = playerStats.GetStat(StatType.Health);
        if (health <= 20f)
        {
            int extraDecay = Mathf.FloorToInt(hoursPassed / 1f) * 2;
            if (extraDecay > 0)
            {
                playerStats.ModifyStat(StatType.Hunger, -extraDecay);
                Debug.Log($"🍔 Extra hunger drain: -{extraDecay} (health low: {health:F0})");
            }
        }
    }

    public void PauseStatDecay()
    {
        isStatDecayPaused = true;
        timeWhenPaused = clock.elapsedTime;
    }

    public void ResumeStatDecay()
    {
        isStatDecayPaused = false;
        lastUpdateTime = clock.elapsedTime;
    }
    public bool IsStatDecayPaused() => isStatDecayPaused;

    public void OnNewGameStarted()
    {
        lastProcessedTime = clock.elapsedTime;
        lastUpdateTime = clock.elapsedTime;
    }

    public void OnGameLoaded()
    {
        lastProcessedTime = clock.elapsedTime;
        lastUpdateTime = clock.elapsedTime;
    }
}