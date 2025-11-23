using System.Collections.Generic;
using UnityEngine;

public class TradingManager : MonoBehaviour
{
    [Header("Graph")]
    [SerializeField] private WindowGraph graph;
    [SerializeField] private int dataPoints = 30;


    [Header("Settings")]
    public float basePrice = 100f;
    public float volatility = 0.02f;
    public float jumpProbability = 0.05f;
    public float minJumpSize = 0.1f;
    public float maxJumpSize = 0.4f;

    private List<int> currentPriceData;

    void Start()
    {
        GenerateNewData();
    }

    public void GenerateNewData()
    {
        currentPriceData = TradingDataGenerator.GeneratePriceData(
            dataPoints, basePrice, volatility, jumpProbability, minJumpSize, maxJumpSize);
        if (graph != null)
            graph.UpdateGraphData(currentPriceData);
    }

    public void UpdatePrice()
    {
        if (currentPriceData == null || currentPriceData.Count == 0)
        {
            GenerateNewData();
            return;
        }

        float lastPrice = currentPriceData[currentPriceData.Count - 1];
        System.Random random = new System.Random();
        float randomMove = (float)(random.NextDouble() * 2 - 1);
        float priceChange = lastPrice * volatility * randomMove;

        if (random.NextDouble() < 0.03)
        {
            float eventMove = (float)(random.NextDouble() * 0.25f + 0.05f);
            if (random.NextDouble() < 0.5) eventMove *= -1;
            priceChange = lastPrice * eventMove;
        }

        float newPrice = lastPrice + priceChange;
        newPrice = Mathf.Max(newPrice, 1f);

        if (currentPriceData.Count >= dataPoints)
            currentPriceData.RemoveAt(0);
        currentPriceData.Add(Mathf.RoundToInt(newPrice));

        if (graph != null)
            graph.UpdateGraphData(currentPriceData);
    }

    public int GetCurrentPrice()
    {
        if (currentPriceData == null || currentPriceData.Count == 0)
            return Mathf.RoundToInt(basePrice);
        return currentPriceData[currentPriceData.Count - 1];
    }
}