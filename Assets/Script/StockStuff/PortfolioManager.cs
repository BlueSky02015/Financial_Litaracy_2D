// PortfolioManager.cs
using System.Collections.Generic;
using UnityEngine;

public class PortfolioManager : MonoBehaviour
{
    public static PortfolioManager instance;

    [System.Serializable]
    public class Holding
    {
        public StockData stock;
        public int sharesOwned;
        public float totalInvested;
    }

    [SerializeField] private List<Holding> holdings = new List<Holding>();

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

    // --- Public API ---
    public bool BuyStock(StockData stock, int shares, int pricePerShare)
    {
        if (shares <= 0) return false;

        // Deduct money
        float totalCost = shares * pricePerShare;
        if (PlayerStats.instance.GetStat(StatType.Money) < totalCost)
            return false;

        PlayerStats.instance.ModifyStat(StatType.Money, -totalCost);

        // Find or create holding
        Holding holding = GetHolding(stock);
        if (holding == null)
        {
            holding = new Holding { stock = stock, sharesOwned = 0, totalInvested = 0 };
            holdings.Add(holding);
        }

        holding.sharesOwned += shares;
        holding.totalInvested += totalCost;

        Debug.Log($"Bought {shares} shares of {stock.stockSymbol} at ${pricePerShare}");
        OnPortfolioUpdated?.Invoke();
        return true;
    }

    public bool SellStock(StockData stock, int shares)
    {
        Holding holding = GetHolding(stock);
        if (holding == null || holding.sharesOwned < shares || shares <= 0)
            return false;

        int price = holding.stock.currentPrice;
        float revenue = shares * price;

        // Add money
        PlayerStats.instance.ModifyStat(StatType.Money, revenue);

        // Update holding
        holding.sharesOwned -= shares;
        if (holding.sharesOwned == 0)
        {
            holdings.Remove(holding);
        }

        Debug.Log($"Sold {shares} shares of {stock.stockSymbol} at ${price}");
        OnPortfolioUpdated?.Invoke();
        return true;
    }

    public Holding GetHolding(StockData stock)
    {
        return holdings.Find(h => h.stock == stock);
    }

    public float GetProfitPercentage(StockData stock)
    {
        Holding holding = GetHolding(stock);
        if (holding == null || holding.sharesOwned == 0 || holding.totalInvested == 0)
            return 0f;

        float currentValue = holding.sharesOwned * stock.currentPrice;
        float profit = currentValue - holding.totalInvested;
        return (profit / holding.totalInvested) * 100f;
    }

    public float GetTotalPortfolioValue()
    {
        float total = 0f;
        foreach (var holding in holdings)
        {
            total += holding.sharesOwned * holding.stock.currentPrice;
        }
        return total;
    }

    // --- Events ---
    public delegate void PortfolioUpdatedHandler();
    public static event PortfolioUpdatedHandler OnPortfolioUpdated;
    public static void NotifyPortfolioUpdated()
    {
        OnPortfolioUpdated?.Invoke();
    }
}