// TradingManager.cs (cleaned up version)
using System.Collections.Generic;
using UnityEngine;

public class TradingManager : MonoBehaviour
{
    [Header("Graph")]
    [SerializeField] private WindowGraph graph;
    [SerializeField] private int dataPoints = 30;

    [Header("Stock Settings")]
    public StockData currentStock; // ← Only this reference to StockData

    private List<int> currentPriceData;

    void Start()
    {
        GenerateNewData();
    }

    public void UpdatePrice()
    {
        if (currentStock == null) return;

        if (currentPriceData == null || currentPriceData.Count == 0)
        {
            GenerateNewData();
            return;
        }

        // Generate new price using StockData's settings
        float lastPrice = currentPriceData[currentPriceData.Count - 1];
        System.Random random = new System.Random();
        float randomMove = (float)(random.NextDouble() * 2 - 1);
        float priceChange = lastPrice * currentStock.volatility * randomMove;

        // 3% chance of big event
        if (random.NextDouble() < currentStock.jumpProbability)
        {
            float eventMove = (float)(random.NextDouble() * 0.25f + 0.05f);
            if (random.NextDouble() < 0.5) eventMove *= -1;
            priceChange = lastPrice * eventMove;
        }

        float newPrice = lastPrice + priceChange;
        newPrice = Mathf.Max(newPrice, 1f);

        // Shift data
        if (currentPriceData.Count >= dataPoints)
            currentPriceData.RemoveAt(0);
        currentPriceData.Add(Mathf.RoundToInt(newPrice));

        // ✅ UPDATE GRAPH AND STOCKDATA SIMULTANEOUSLY
        if (graph != null)
            graph.UpdateGraphData(currentPriceData);

        // ✅ UPDATE STOCKDATA IMMEDIATELY
        currentStock.UpdateCurrentPrice(Mathf.RoundToInt(newPrice));
    }

    public void GenerateNewData()
    {
        if (currentStock == null)
        {
            Debug.LogError("CurrentStock is null!");
            return;
        }

        currentPriceData = TradingDataGenerator.GeneratePriceData(
            dataPoints,
            currentStock.basePrice,
            currentStock.volatility,
            currentStock.jumpProbability,
            currentStock.minJumpSize,
            currentStock.maxJumpSize
        );

        if (currentPriceData != null && currentPriceData.Count > 0)
        {

            foreach (int price in currentPriceData)
            {
                if (price <= 1)
                {
                    Debug.LogWarning($"⚠️ Price is {price} - might be too low!");
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("❌ currentPriceData is null or empty!");
            return;
        }

        // Update StockData
        int currentPrice = currentPriceData[currentPriceData.Count - 1];
        currentStock.UpdateCurrentPrice(currentPrice);

        if (graph != null)
        {
            graph.UpdateGraphData(currentPriceData);
            Debug.Log("📈 Graph updated");
        }
    }

    public int GetCurrentPrice()
    {
        if (currentPriceData == null || currentPriceData.Count == 0)
            return Mathf.RoundToInt(currentStock?.basePrice ?? 100f);
        return currentPriceData[currentPriceData.Count - 1];
    }
    public void UpdateCurrentStockPrice()
    {
        if (currentPriceData != null && currentPriceData.Count > 0)
        {
            // Get the current price from your price data
            int currentPrice = currentPriceData[currentPriceData.Count - 1];

            // Update the corresponding StockData
            if (currentStock != null)
            {
                currentStock.UpdateCurrentPrice(currentPrice);
            }
        }
    }

}