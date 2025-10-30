// InterestAlgorithm.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Trading/Algorithms/Interest")]
public class InterestAlgorithm : PriceAlgorithm
{
    [SerializeField] private float dailyInterestRate = 0.0002f;
    [SerializeField] private float maxVolatility = 0.5f;
    
    public override float CalculateNextPrice(float currentPrice, float _, float __)
    {
        return currentPrice * (1 + dailyInterestRate) 
               + GaussianRandom() * maxVolatility;
    }
}