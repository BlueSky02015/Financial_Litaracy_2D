using UnityEngine;

public class WeeklyIncomeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clock clock;
    [SerializeField] private InvestmentManager investmentManager;

    [Header("Settings")]
    [SerializeField] private float incomeIntervalDays = 7f;

    private float lastIncomeTime = -1f;
    private float IntervalSeconds => incomeIntervalDays * 24f * 3600f; // days → seconds

    void Start()
    {
        if (clock == null || investmentManager == null)
        {
            Debug.LogError("WeeklyIncomeManager: Missing Clock or InvestmentManager!", this);
            enabled = false;
            return;
        }

        lastIncomeTime = clock.elapsedTime;
    }

    void Update()
    {
        if (clock == null) return;

        float currentTime = clock.elapsedTime;

        // Skip if time hasn't advanced enough
        if (currentTime < lastIncomeTime + IntervalSeconds)
            return;

        // Calculate how many full intervals have passed
        float timePassed = currentTime - lastIncomeTime;
        int intervalsPassed = Mathf.FloorToInt(timePassed / IntervalSeconds);

        // Cap to avoid lag after long sleep
        intervalsPassed = Mathf.Min(intervalsPassed, 10);

        // Collect income for each interval
        for (int i = 0; i < intervalsPassed; i++)
        {
            investmentManager.CollectWeeklyIncome();
            lastIncomeTime += IntervalSeconds;
        }
    }
}