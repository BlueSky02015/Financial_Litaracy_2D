using UnityEngine;

public class PriceAlgorithm : ScriptableObject
{
    [Header("Base Parameters")]
    public float baseVolatility = 1.5f;
    public float drift = 0.02f;
    public float meanReversion = 0.1f;
    public float spread = 0.5f;

    [Header("Market Events")]
    public float eventProbability = 0.01f;
    public float eventMultiplier = 5f;

    [Header("Time Effects")]
    public float openingVolatility = 2f;
    public float closingVolatility = 2f;

    public virtual float CalculateNextPrice(float currentPrice, float longTermAverage, float timeFactor)
    {
        // Calculate time-based volatility
        float volatility = baseVolatility * timeFactor;

        // 1. Base random movement
        float randomChange = GaussianRandom() * volatility;

        // 2. Drift component
        float driftChange = drift * currentPrice;

        // 3. Mean reversion
        float reversionChange = (longTermAverage - currentPrice) * meanReversion;

        // 4. Market events
        if (Random.value < eventProbability)
        {
            randomChange *= eventMultiplier;
        }

        return currentPrice + randomChange + driftChange + reversionChange;
    }

    protected float GaussianRandom()
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        return Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
    }
}

