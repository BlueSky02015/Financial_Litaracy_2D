// CompoundGrowthAlgorithm.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Trading/Algorithms/Compound Growth")]
public class CompoundGrowthAlgorithm : PriceAlgorithm
{
    [SerializeField] private float dailyGrowthRate = 0.0005f;
    
    public override float CalculateNextPrice(float currentPrice, float longTermAverage, float timeFactor)
    {
        float baseChange = base.CalculateNextPrice(currentPrice, longTermAverage, timeFactor);
        return baseChange * (1 + dailyGrowthRate);
    }
}