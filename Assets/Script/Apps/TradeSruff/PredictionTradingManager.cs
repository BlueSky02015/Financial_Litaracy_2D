// PredictionTradingManager.cs (connected through StockData)
using System.Collections.Generic;
using UnityEngine;

public class PredictionTradingManager : MonoBehaviour
{
    public static PredictionTradingManager instance;

    private List<TradePrediction> activeTrades = new List<TradePrediction>();

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
    }

    /// <summary>
    /// Place a trade with lots (using StockData for current price)
    /// </summary>
    public bool PlaceTrade(StockData stock, bool isLong, int lots)
    {
        if (lots <= 0 || stock == null || stock.currentPrice <= 0)
        {
            Debug.Log("Invalid stock data or price!");
            return false;
        }

        return PlaceTradeInternal(stock, isLong, lots);
    }

    bool PlaceTradeInternal(StockData stock, bool isLong, int lots)
    {
        float currentPrice = stock.currentPrice; // ✅ Use the stock directly
        float totalCost = lots * 10f;

        if (PlayerStats.instance.GetStat(StatType.Money) < totalCost)
        {
            Debug.Log($"Not enough money! Need ${totalCost:F2}, have ${PlayerStats.instance.GetStat(StatType.Money):F2}");
            return false;
        }

        if (GetActiveTradeForMarket(stock.stockName) != null)
        {
            Debug.Log($"Already have an active trade for {stock.stockName}!");
            return false;
        }

        PlayerStats.instance.ModifyStat(StatType.Money, -totalCost);

        var trade = new TradePrediction
        {
            marketSymbol = stock.stockName,
            entryPrice = currentPrice,
            lots = lots,
            isLong = isLong,
            entryTime = Clock.instance.elapsedTime,
            isResolved = false,
            stockReference = stock // ✅ STORE THE REFERENCE
        };

        activeTrades.Add(trade);
        Debug.Log($"Trade placed: {lots} lots of {stock.stockName} at ${currentPrice:F2} (Total: ${totalCost:F2})");
        return true;
    }

    /// <summary>
    /// Check if any trades are ready to resolve
    /// </summary>
    void Update()
    {
        CheckTradesForResolution();
    }

    void CheckTradesForResolution()
    {
        for (int i = activeTrades.Count - 1; i >= 0; i--)
        {
            var trade = activeTrades[i];

            if (!trade.isResolved &&
                (Clock.instance.elapsedTime - trade.entryTime) >= trade.tradeDuration)
            {
                ResolveTrade(trade);
                activeTrades.RemoveAt(i);
            }
        }
    }

    void ResolveTrade(TradePrediction trade)
    {
        // ✅ GET FINAL PRICE FROM STORED STOCK REFERENCE
        float finalPrice = 0f;

        if (trade.stockReference != null)
        {
            finalPrice = trade.stockReference.currentPrice;
        }
        else
        {
            // Fallback to symbol search
            finalPrice = GetCurrentPriceFromName(trade.marketSymbol);
        }

        if (finalPrice <= 0)
        {
            Debug.LogWarning($"Could not get final price for {trade.marketSymbol}, using entry price");
            finalPrice = trade.entryPrice;
        }

        trade.exitPrice = finalPrice;
        // Calculate profit/loss: (exit - entry) * lots * 10
        float priceChange = trade.exitPrice - trade.entryPrice;
        float profitPerLot = priceChange * 10f;
        trade.profitLoss = profitPerLot * trade.lots;

        trade.isResolved = true;

        if (trade.isLong)
        {
            if (priceChange > 0)
            {
                PlayerStats.instance.ModifyStat(StatType.Money, trade.profitLoss);
                Debug.Log($"✅ LONG trade PROFIT! ${trade.profitLoss:F2}");
            }
            else if (priceChange < 0)
            {
                PlayerStats.instance.ModifyStat(StatType.Money, trade.profitLoss); // negative
                Debug.Log($"❌ LONG trade LOSS! ${trade.profitLoss:F2}");
            }
            else
            {
                Debug.Log($"➡️ LONG trade BREAK-EVEN");
            }
        }
        else
        {
            if (priceChange < 0)
            {
                PlayerStats.instance.ModifyStat(StatType.Money, -trade.profitLoss); // negative * negative = positive
                Debug.Log($"✅ SHORT trade PROFIT! ${-trade.profitLoss:F2}");
            }
            else if (priceChange > 0)
            {
                PlayerStats.instance.ModifyStat(StatType.Money, trade.profitLoss); // negative
                Debug.Log($"❌ SHORT trade LOSS! ${trade.profitLoss:F2}");
            }
            else
            {
                Debug.Log($"➡️ SHORT trade BREAK-EVEN");
            }
        }
    }

    public TradePrediction GetActiveTradeForMarket(string symbol)
    {
        return activeTrades.Find(trade => trade.marketSymbol == symbol && !trade.isResolved);
    }

    public List<TradePrediction> GetActiveTrades() => activeTrades;

    // Helper method
    StockData FindStockBySymbol(string symbol)
    {
        if (StockManager.instance != null)
        {
            return StockManager.instance.GetStockByName(symbol);
        }
        return null;
    }

    public bool HasActiveTrade(string symbol)
    {
        return GetActiveTradeForMarket(symbol) != null;
    }

    public TradePrediction GetActiveTrade(string symbol)
    {
        return GetActiveTradeForMarket(symbol);
    }

    float GetCurrentPriceFromName(string name)
{
    // Try StockManager first
    if (StockManager.instance != null)
    {
        StockData stock = StockManager.instance.GetStockByName(name);
        if (stock != null && stock.currentPrice > 0)
        {
            return stock.currentPrice;
        }
    }

    // If StockManager fails, try to find it in all loaded StockData
    // This might require you to have a list of all StockData assets
    // For now, return 0 to indicate failure
    Debug.LogWarning($"Stock {name} not found in StockManager!");
    return 0f;
}
}