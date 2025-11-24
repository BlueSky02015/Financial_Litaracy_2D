using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public static StockManager instance;

    [SerializeField] private StockData[] allStocks;

    private Dictionary<string, StockData> stockMap = new Dictionary<string, StockData>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Build lookup map
        foreach (var stock in allStocks)
        {
            if (stock != null)
                stockMap[stock.stockName] = stock;
        }
    }

    public StockData GetStockByName(string Name)
    {
        return stockMap.ContainsKey(Name) ? stockMap[Name] : null;
    }
}