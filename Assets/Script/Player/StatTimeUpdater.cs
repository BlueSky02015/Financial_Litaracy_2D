using UnityEngine;
public class StatTimeUpdater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clock clock;
    [SerializeField] private PlayerStats playerStats;

    [Header("Settings")]
    [Tooltip("How often to update stats (in in-game hours)")]
    [SerializeField] private float updateIntervalHours = 1f;

    private float lastUpdateTime = -1f;
    private float IntervalSeconds => updateIntervalHours * 3600f; // 1 hour = 3600 seconds

    void Start()
    {
        if (clock == null || playerStats == null)
        {
            Debug.LogError("StatTimeUpdater: Missing Clock or PlayerStats reference!", this);
            enabled = false;
            return;
        }

        // Initialize to current game time
        lastUpdateTime = clock.elapsedTime;
    }

    void Update()
    {
        if (clock == null || playerStats == null) return;

        float currentTime = clock.elapsedTime;

        // Skip if time hasn't advanced
        if (currentTime <= lastUpdateTime)
            return;

        float timePassed = currentTime - lastUpdateTime;
        if (timePassed < IntervalSeconds)
            return;

        // Calculate how many full intervals have passed
        float hoursPassed = timePassed / 3600f;
        playerStats.UpdateStatsPerHour(hoursPassed);

        // Snap to last full interval to avoid drift
        lastUpdateTime = currentTime - (timePassed % IntervalSeconds);
    }
}