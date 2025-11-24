using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager instance;
    private string SaveFolderPath => Path.Combine(Application.dataPath, "GameSaveFolder");
    private string SaveFilePath => Path.Combine(SaveFolderPath, "game_save.json");

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameSaveManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData();

        // Save stats
        var stats = saveData.stats;
        stats.health = PlayerStats.instance.GetStat(StatType.Health);
        stats.hunger = PlayerStats.instance.GetStat(StatType.Hunger);
        stats.stamina = PlayerStats.instance.GetStat(StatType.Stamina);
        stats.mood = PlayerStats.instance.GetStat(StatType.Mood);
        stats.money = PlayerStats.instance.GetStat(StatType.Money);
        stats.knowledge = PlayerStats.instance.GetStat(StatType.Knowledge);


        // Save time
        saveData.elapsedTime = Clock.instance.elapsedTime;
        saveData.currentDay = Clock.instance.GetCurrentDay();

        // Save portfolio
        if (PortfolioManager.instance != null)
        {
            foreach (var holding in PortfolioManager.instance.holdings)
            {
                saveData.stockHoldings.Add(new StockHoldingSaveData
                {
                    stockName = holding.stock.stockName,
                    sharesOwned = holding.sharesOwned,
                    totalInvested = holding.totalInvested
                });
            }
        }

        // SAVE INVESTMENTS
        if (InvestmentManager.instance != null)
        {
            foreach (var investment in InvestmentManager.instance.Investments)
            {
                saveData.investments.Add(new InvestmentSaveData
                {
                    assetName = investment.asset.assetName,
                    ownedCount = investment.ownedCount,
                    isRented = investment.isRented
                });
            }
        }

        // SAVE DEBT
        if (DebtSystem.instance != null)
        {
            saveData.currentDebt = DebtSystem.instance.GetCurrentDebt();
            // You'll need to add GetCurrentDebt() method to DebtSystem
        }

        // Save tutorial
        if (TutorialManager.instance != null)
        {
            saveData.completedAppTutorials = new List<string>(
                TutorialManager.instance.GetCompletedTutorials()
            );
        }

        // Ensure save folder exists
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
        }

        // Convert to JSON
        string json = JsonUtility.ToJson(saveData, prettyPrint: true);

        // Write to file
        File.WriteAllText(SaveFilePath, json);
        Debug.Log($"Game saved to: {SaveFilePath}");
    }

    public bool LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("No save file found. Starting new game.");
            return false;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        // Load stats
        var stats = saveData.stats;
        PlayerStats.instance.SetStat(StatType.Health, stats.health);
        PlayerStats.instance.SetStat(StatType.Hunger, stats.hunger);
        PlayerStats.instance.SetStat(StatType.Stamina, stats.stamina);
        PlayerStats.instance.SetStat(StatType.Mood, stats.mood);
        PlayerStats.instance.SetStat(StatType.Money, stats.money);
        PlayerStats.instance.SetStat(StatType.Knowledge, stats.knowledge);

        // Load time
        Clock.instance.SetElapsedTime(saveData.elapsedTime);
        Clock.instance.SetCurrentDay(saveData.currentDay);

        if (StatTimeUpdater.instance != null)
        {
            StatTimeUpdater.instance.OnGameLoaded();
        }

        // Load portfolio
        if (PortfolioManager.instance != null)
        {
            PortfolioManager.instance.ClearHoldings();
            foreach (var holdingData in saveData.stockHoldings)
            {
                StockData stock = FindStockByName(holdingData.stockName);
                if (stock != null)
                {
                    PortfolioManager.instance.AddHolding(
                        stock,
                        holdingData.sharesOwned,
                        holdingData.totalInvested
                    );
                }
            }
            PortfolioManager.NotifyPortfolioUpdated();
        }

        // LOAD INVESTMENTS
        if (InvestmentManager.instance != null)
        {
            InvestmentManager.instance.ClearAllHoldings();
            foreach (var invData in saveData.investments)
            {
                InvestmentAsset asset = InvestmentManager.instance?.FindAssetByName(invData.assetName);
                if (asset != null)
                {
                    InvestmentManager.instance.AddInvestment(
                        asset,
                        invData.ownedCount,
                        invData.isRented
                    );
                }
            }
            InvestmentManager.NotifyInvestmentsChanged();
        }

        // LOAD DEBT
        if (DebtSystem.instance != null)
        {
            DebtSystem.instance.SetCurrentDebt(saveData.currentDebt);
        }

        // Load tutorial progress
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.SetCompletedTutorials(
                new HashSet<string>(saveData.completedAppTutorials)
            );
        }

        Debug.Log("Game loaded!");
        return true;
    }

    private StockData FindStockByName(string name)
    {
        if (StockManager.instance != null)
        {
            return StockManager.instance.GetStockByName(name);
        }
        return null;
    }
}