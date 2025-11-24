using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInvestment
{
    public InvestmentAsset asset;
    public bool isRented;
    public int ownedCount; // number of this asset owned
}

public class InvestmentManager : MonoBehaviour
{
    public static InvestmentManager instance;

    public List<InvestmentAsset> allAssets = new List<InvestmentAsset>();
    [SerializeField] private List<PlayerInvestment> investments = new List<PlayerInvestment>();
    public List<PlayerInvestment> Investments => investments;

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

    // --- Buy ---
    public bool BuyAsset(InvestmentAsset asset)
    {
        if (PlayerStats.instance.GetStat(StatType.Money) < asset.purchasePrice)
            return false;

        PlayerStats.instance.ModifyStat(StatType.Money, -asset.purchasePrice);

        var existing = investments.Find(i => i.asset == asset);
        if (existing != null)
        {
            existing.ownedCount++;
        }
        else
        {
            investments.Add(new PlayerInvestment
            {
                asset = asset,
                isRented = false,
                ownedCount = 1
            });
        }

        OnInvestmentsChanged?.Invoke();
        return true;
    }

    // --- Sell ---
    public void SellAsset(InvestmentAsset asset, int count = 1)
    {
        var holding = investments.Find(i => i.asset == asset);
        if (holding == null || holding.ownedCount < count) return;

        holding.ownedCount -= count;
        if (holding.ownedCount <= 0)
        {
            investments.Remove(holding);
        }

        int income = asset.sellPrice * count;
        PlayerStats.instance.ModifyStat(StatType.Money, income);

        OnInvestmentsChanged?.Invoke();
    }

    // --- Rent/Unrent ---
    public void ToggleRent(InvestmentAsset asset)
    {
        var holding = investments.Find(i => i.asset == asset);
        if (holding == null) return;

        holding.isRented = !holding.isRented;
        OnInvestmentsChanged?.Invoke();
    }

    // --- Weekly Income ---
    public void CollectWeeklyIncome()
    {
        int totalIncome = 0;
        foreach (var inv in investments)
        {
            if (inv.isRented)
            {
                totalIncome += inv.asset.weeklyRentalIncome * inv.ownedCount;
            }
        }

        if (totalIncome > 0)
        {
            PlayerStats.instance.ModifyStat(StatType.Money, totalIncome);
            Debug.Log($"Collected weekly rental income: ${totalIncome}");
        }
    }
    public void ResetHoldings()
    {
        investments.Clear();
        Debug.Log("🏢 Investment holdings reset");
        OnInvestmentsChanged?.Invoke();
    }

    public void AddInvestment(InvestmentAsset asset, int count, bool isRented)
    {
        var existing = investments.Find(i => i.asset == asset);
        if (existing != null)
        {
            existing.ownedCount = count;
            existing.isRented = isRented;
        }
        else
        {
            investments.Add(new PlayerInvestment
            {
                asset = asset,
                ownedCount = count,
                isRented = isRented
            });
        }
    }

    public void ClearAllHoldings()
    {
        investments.Clear();
        OnInvestmentsChanged?.Invoke();
    }


    // --- Events ---
    public delegate void InvestmentsChangedHandler();
    public static event InvestmentsChangedHandler OnInvestmentsChanged;
    public static void NotifyInvestmentsChanged()
    {
        OnInvestmentsChanged?.Invoke();
    }
    public InvestmentAsset FindAssetByName(string assetName)
    {
        return allAssets.Find(asset => asset.assetName == assetName);
    }
}