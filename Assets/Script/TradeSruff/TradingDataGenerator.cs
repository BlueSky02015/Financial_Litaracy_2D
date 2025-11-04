// TradingDataGenerator.cs
using System.Collections.Generic;
using UnityEngine;

public static class TradingDataGenerator
{
    /// <summary>
    /// Generates a realistic-looking price series
    /// </summary>
    /// <param name="pointCount">Number of data points (e.g., 30 for 30 days)</param>
    /// <param name="basePrice">Starting price (e.g., 100)</param>
    /// <param name="volatility">How much prices jump (0.01 = 1% daily moves)</param>
    /// <param name="trend">Overall direction (-0.5 to 0.5)</param>
    /// <returns>List of prices</returns>
    public static List<int> GeneratePriceData(int pointCount, float basePrice = 100f, float volatility = 0.02f, float trend = 0f)
    {
        List<int> prices = new List<int>();
        float currentPrice = basePrice;

        System.Random random = new System.Random();

        for (int i = 0; i < pointCount; i++)
        {
            // Add small trend (drift)
            float drift = trend * volatility;

            // Generate random move (-1 to +1)
            float randomMove = (float)(random.NextDouble() * 2 - 1);

            // Calculate price change
            float priceChange = currentPrice * (drift + volatility * randomMove);

            // Apply mean reversion (pull toward basePrice over time)
            float meanReversion = (basePrice - currentPrice) * 0.01f;
            currentPrice += priceChange + meanReversion;

            // Ensure price stays positive
            currentPrice = Mathf.Max(currentPrice, 1f);

            prices.Add(Mathf.RoundToInt(currentPrice));
        }

        return prices;
    }

    /// <summary>
    /// Generate data with occasional big events (news, crashes, pumps)
    /// </summary>
    public static List<int> GeneratePriceDataWithEvents(int pointCount, float basePrice = 100f)
    {
        List<int> prices = new List<int>();
        float currentPrice = basePrice;
        System.Random random = new System.Random();

        for (int i = 0; i < pointCount; i++)
        {
            // Normal daily move
            float volatility = 0.015f;
            float randomMove = (float)(random.NextDouble() * 2 - 1);
            float priceChange = currentPrice * volatility * randomMove;

            // 5% chance of big event
            if (random.NextDouble() < 0.05)
            {
                float eventMagnitude = (float)(random.NextDouble() * 0.3f + 0.1f); // 10-40% move
                if (random.NextDouble() < 0.5)
                    eventMagnitude *= -1; // 50% chance crash

                priceChange = currentPrice * eventMagnitude;
            }

            currentPrice += priceChange;
            currentPrice = Mathf.Max(currentPrice, 1f);
            prices.Add(Mathf.RoundToInt(currentPrice));
        }

        return prices;
    }
}