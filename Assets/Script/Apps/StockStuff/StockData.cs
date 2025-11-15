using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stock", menuName = "Trading/Stock Data")]
public class StockData : ScriptableObject
{
    [Header("Basic Info")]
    public string stockSymbol = "STOCK";
    [TextArea] public string stockName = "Stock Company";

    [Header("Price Settings")]
    public float basePrice = 100f;
    public float volatility = 0.02f; // 2% daily move
    public float trend = 0f; // -0.3 to 0.3

    [Header("Runtime")]
    [HideInInspector] public List<int> priceHistory = new List<int>();
    [HideInInspector] public int currentPrice => 
        priceHistory.Count > 0 ? priceHistory[priceHistory.Count - 1] : Mathf.RoundToInt(basePrice);

    // Generate initial price data
    public void InitializePriceHistory(int pointCount)
    {
        priceHistory = TradingDataGenerator.GeneratePriceDataWithEvents(
            pointCount, basePrice);
    }

    // Update with new price
    public void AddNewPrice()
    {
        if (priceHistory.Count == 0)
        {
            InitializePriceHistory(1);
            return;
        }

        float lastPrice = priceHistory[priceHistory.Count - 1];
        System.Random random = new System.Random();
        float randomMove = (float)(random.NextDouble() * 2 - 1);
        float priceChange = lastPrice * volatility * randomMove;

        // 3% chance of big event
        if (random.NextDouble() < 0.03)
        {
            float eventMove = (float)(random.NextDouble() * 0.25f + 0.05f);
            if (random.NextDouble() < 0.5) eventMove *= -1;
            priceChange = lastPrice * eventMove;
        }

        float newPrice = lastPrice + priceChange;
        newPrice = Mathf.Max(newPrice, 1f);
        priceHistory.Add(Mathf.RoundToInt(newPrice));

        // Keep history size reasonable
        if (priceHistory.Count > 100)
            priceHistory.RemoveAt(0);
    }
}