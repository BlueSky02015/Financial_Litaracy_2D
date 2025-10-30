using UnityEngine;
using TMPro;

public class StockTradingSystem : BaseTradingSystem 
{
    [Header("Market Hours")]
    [SerializeField] private float marketDurationHours = 6.5f;
    [SerializeField] private float closeDurationHours = 15.5f;
    
    private float marketOpenTime;

    protected override void Start()
    {
        base.Start();
        marketOpenTime = Time.time;
        
        InvokeRepeating(nameof(UpdatePrice), 1f, 1f);
        InvokeRepeating(nameof(CheckMarketHours), 0f, 60f);
    }

    protected override void UpdatePrice()
    {
        if (!marketIsOpen) return;
        
        float timeFactor = GetTimeFactor();
        assetData.UpdatePrice(timeFactor);
        UpdateGraph();
        UpdateUI();
    }

    private float GetTimeFactor()
    {
        float marketDuration = marketDurationHours * 3600;
        float elapsed = Time.time - marketOpenTime;
        float normalizedTime = elapsed / marketDuration;
        
        PriceAlgorithm currentAlgorithm = assetData.GetCurrentAlgorithm();
        
        float openFactor = Mathf.Clamp01(1 - normalizedTime * 5f) * currentAlgorithm.openingVolatility;
        float closeFactor = Mathf.Clamp01(normalizedTime * 5f - 4f) * currentAlgorithm.closingVolatility;
        
        return 1f + openFactor + closeFactor;
    }

    protected override void CheckMarketHours()
    {
        bool wasOpen = marketIsOpen;
        float marketDuration = marketDurationHours * 3600;
        
        if (Time.time - marketOpenTime > marketDuration)
        {
            marketIsOpen = false;
            marketOpenTime = Time.time + closeDurationHours * 3600;
        }
        else
        {
            marketIsOpen = true;
        }

        if (wasOpen && !marketIsOpen)
        {
            Debug.Log("Market Closed");
        }
        else if (!wasOpen && marketIsOpen)
        {
            Debug.Log("Market Opened");
            assetData.currentMidPrice *= Random.Range(0.98f, 1.02f);
            UpdateUI();
        }
    }
}