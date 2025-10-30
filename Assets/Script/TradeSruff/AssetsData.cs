using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "AssetData", menuName = "Trading/Asset Data")]
public class AssetsData : ScriptableObject
{
    public enum AlgorithmType
    {
        Standard,
        CompoundGrowth,
        InterestBearing
    }

    [Header("Basic Settings")]
    public string assetName;
    public float initialPrice = 5f;
    public int node = 15;
    public AlgorithmType algorithmType;

    [Header("Algorithm References")]
    [SerializeField] private StandardAlgorithm standardAlgorithm;
    [SerializeField] private CompoundGrowthAlgorithm compoundAlgorithm;
    [SerializeField] private InterestAlgorithm interestAlgorithm;

    [NonSerialized] public float currentMidPrice;
    [NonSerialized] public List<int> priceHistory = new List<int>();

    public void Initialize()
    {
        currentMidPrice = initialPrice;
        priceHistory.Clear();
        
        // Initialize history based on algorithm type
        for (int i = 0; i < node; i++)
        {
            priceHistory.Add(Mathf.RoundToInt(initialPrice * GetInitialHistoryMultiplier()));
        }
    }

    private float GetInitialHistoryMultiplier()
    {
        return algorithmType switch
        {
            AlgorithmType.CompoundGrowth => UnityEngine.Random.Range(0.8f, 1.2f),
            AlgorithmType.InterestBearing => UnityEngine.Random.Range(0.98f, 1.02f),
            _ => UnityEngine.Random.Range(0.9f, 1.1f) // Standard
        };
    }

    public void UpdatePrice(float timeFactor)
    {
        currentMidPrice = GetCurrentAlgorithm().CalculateNextPrice(
            currentMidPrice,
            GetLongTermAverage(),
            timeFactor
        );

        // Update history
        if (priceHistory.Count >= node) priceHistory.RemoveAt(0);
        priceHistory.Add(Mathf.RoundToInt(currentMidPrice));
    }

    public PriceAlgorithm GetCurrentAlgorithm()
    {
        return algorithmType switch
        {
            AlgorithmType.CompoundGrowth => compoundAlgorithm,
            AlgorithmType.InterestBearing => interestAlgorithm,
            _ => standardAlgorithm
        };
    }

    private float GetLongTermAverage()
    {
        return algorithmType switch
        {
            AlgorithmType.CompoundGrowth => currentMidPrice * 1.1f, // Target 10% above current
            AlgorithmType.InterestBearing => currentMidPrice * 1.0002f, // Tiny daily growth
            _ => initialPrice // Standard mean reversion
        };
    }
}