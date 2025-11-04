// TradingManager.cs
using System.Collections.Generic;
using UnityEngine;

public class TradingManager : MonoBehaviour
{
    [Header("Graph")]
    [SerializeField] private WindowGraph graph;
    [SerializeField] private int dataPoints = 30; // e.g., 30 days

    [Header("Stock Settings")]
    [SerializeField] private string stockName = "GAME";
    [SerializeField] private float basePrice = 100f;
    [SerializeField] private float volatility = 0.02f;
    [SerializeField] private float trend = 0f; // -0.3 to 0.3

    private List<int> currentPriceData;

    void Start()
    {
        GenerateNewData();
    }

    public void GenerateNewData()
    {
        // Use the event-based generator for more realism
        currentPriceData = TradingDataGenerator.GeneratePriceDataWithEvents(dataPoints, basePrice);
        
        // Update graph
        if (graph != null)
            graph.UpdateGraphData(currentPriceData);
    }

    // Call this when player buys/sells to update live
    public void UpdatePrice()
    {
        if (currentPriceData == null || currentPriceData.Count == 0) return;

        // Simple: shift data left, add new price
        currentPriceData.RemoveAt(0);
        
        float lastPrice = currentPriceData[currentPriceData.Count - 1];
        float randomMove = Random.Range(-volatility, volatility);
        float newPrice = lastPrice * (1 + randomMove);
        newPrice = Mathf.Max(newPrice, 1f);
        
        currentPriceData.Add(Mathf.RoundToInt(newPrice));
        graph.UpdateGraphData(currentPriceData);
    }

    // Optional: refresh button
    public void OnRefreshButton()
    {
        GenerateNewData();
    }

    // Get current price (for buying/selling)
    public int GetCurrentPrice()
    {
        if (currentPriceData == null || currentPriceData.Count == 0)
            return Mathf.RoundToInt(basePrice);
        return currentPriceData[currentPriceData.Count - 1];
    }
}