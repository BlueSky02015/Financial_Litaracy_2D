using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatConfig
{
    public StatType statType;
    public float maxValue = 100f;
    public float startValue = 100f;
    public float decayPerHour = 0f;   // hunger decreases over time
    public float gainPerHour = 0f;    // stamina recovers while sleeping
}

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    [Header("Stats Configuration")]
    [SerializeField] private List<StatConfig> statConfigs = new List<StatConfig>();

    // Runtime stat values
    private Dictionary<StatType, float> currentValues = new Dictionary<StatType, float>();
    private Dictionary<StatType, StatConfig> configMap = new Dictionary<StatType, StatConfig>();

    // Event system
    public delegate void StatChangedHandler(StatType statType, float currentValue, float maxValue);
    public static event StatChangedHandler OnStatChanged;

    // Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeStats()
    {
        // Build config map
        foreach (var config in statConfigs)
        {
            configMap[config.statType] = config;
            currentValues[config.statType] = config.startValue;
        }
    }

    // --- Public API ---
    public float GetStat(StatType stat) =>
        currentValues.ContainsKey(stat) ? currentValues[stat] : 0f;

    public float GetMaxStat(StatType stat) =>
        configMap.ContainsKey(stat) ? configMap[stat].maxValue : 100f;

    public void ModifyStat(StatType stat, float delta)
    {
        if (!currentValues.ContainsKey(stat)) return;

        float max = configMap[stat].maxValue;
        currentValues[stat] = Mathf.Clamp(currentValues[stat] + delta, 0f, max);
        OnStatChanged?.Invoke(stat, currentValues[stat], max);
    }

    public void SetStat(StatType stat, float value)
    {
        if (!currentValues.ContainsKey(stat)) return;
        float max = configMap[stat].maxValue;
        currentValues[stat] = Mathf.Clamp(value, 0f, max);
        OnStatChanged?.Invoke(stat, currentValues[stat], max);
    }

    // --- Time-based updates (called by Clock) ---
    public void UpdateStatsPerHour(float hoursPassed)
    {
        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            if (!configMap.ContainsKey(stat)) continue;

            var config = configMap[stat];
            float change = (config.gainPerHour - config.decayPerHour) * hoursPassed;
            ModifyStat(stat, change);
        }
    }
}