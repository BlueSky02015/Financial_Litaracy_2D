using UnityEngine;

public class TradingUpdater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Clock clock;
    [SerializeField] private TradingManager tradingManager;

    [Header("Settings")]
    [SerializeField] private float updateIntervalMinutes = 30f;
    [SerializeField] private int maxUpdatesPerFrame = 20;

    private float lastUpdateTime = -1f;
    private float IntervalSeconds => updateIntervalMinutes * 60f;
    [SerializeField] private StockData[] allStocks;


    void Start()
    {
        if (clock == null || tradingManager == null)
        {
            Debug.LogError("TradingUpdater missing!", this);
            enabled = false;
            return;
        }

        // Initialize to current game time
        lastUpdateTime = clock.elapsedTime;
    }

    void Update()
    {
        float currentTime = clock.elapsedTime;

        // Skip if time hasn't moved (game paused)
        if (currentTime <= lastUpdateTime)
            return;

        float timeSinceLast = currentTime - lastUpdateTime;
        int updatesNeeded = Mathf.FloorToInt(timeSinceLast / IntervalSeconds);

        if (updatesNeeded <= 0)
            return;

        // Cap updates to avoid performance spikes
        updatesNeeded = Mathf.Min(updatesNeeded, maxUpdatesPerFrame);

        // Perform each update
        for (int i = 0; i < updatesNeeded; i++)
        {
            tradingManager.UpdatePrice();
            lastUpdateTime += IntervalSeconds;
        }

        for (int i = 0; i < updatesNeeded; i++)
        {
            foreach (var stock in allStocks)
            {
                stock.AddNewPrice();
            }
            lastUpdateTime += IntervalSeconds;
        }

       PortfolioManager.NotifyPortfolioUpdated();
    }
}