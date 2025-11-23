// TradingDataGenerator.cs
using System.Collections.Generic;
using UnityEngine;

public static class TradingDataGenerator
{
    private static System.Random rng = new System.Random();
    public static List<int> GeneratePriceData(
        int pointCount, 
        float basePrice = 100f,
        float volatility = 0.02f,      // σ: 2% daily move
        float jumpProbability = 0.05f, // λ: 5% chance of jump
        float minJumpSize = 0.1f,      // ξ_min: 10%
        float maxJumpSize = 0.4f       // ξ_max: 40%
    )
    {
        List<int> prices = new List<int>();
        float currentPrice = basePrice;

        for (int i = 0; i < pointCount; i++)
        {
            // 1. BASE GBM MOVE: σ × Z_t (Z_t ~ N(0,1))
            float z = SampleNormal(0f, 1f); // Standard normal
            float priceChange = currentPrice * volatility * z;

            // 2. JUMP COMPONENT (with probability λ)
            if (rng.NextDouble() < jumpProbability)
            {
                float jumpSize = Random.Range(minJumpSize, maxJumpSize);
                if (rng.NextDouble() < 0.5f) jumpSize *= -1f; // Random sign
                priceChange = currentPrice * jumpSize; // Replace GBM move with jump
            }

            // 3. UPDATE PRICE
            currentPrice += priceChange;

            // 4. SAFETY: No negative/zero prices
            currentPrice = Mathf.Max(currentPrice, 1f);

            prices.Add(Mathf.RoundToInt(currentPrice));
        }

        return prices;
    }

    // Efficient Normal Distribution Sampling (Box-Muller)
    private static float SampleNormal(float mean, float stdDev)
    {
        float u1 = 1.0f - (float)rng.NextDouble(); // Uniform(0,1] 
        float u2 = 1.0f - (float)rng.NextDouble();
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }
}