using System.IO;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager instance;

    // private const string SAVE_FILE_NAME = "game_save.json";
    // private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // GameSaveManager.cs
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

        // Save tutorial progress
        saveData.currentTutorialStep = TutorialManager.instance?.currentStep.ToString() ?? "None";

        // Save time
        saveData.elapsedTime = Clock.instance.elapsedTime;

        // Save portfolio
        saveData.stockHoldings.Clear();
        if (PortfolioManager.instance != null)
        {
            foreach (var holding in PortfolioManager.instance.holdings)
            {
                saveData.stockHoldings.Add(new StockHoldingSaveData
                {
                    stockSymbol = holding.stock.stockSymbol,
                    sharesOwned = holding.sharesOwned,
                    totalInvested = holding.totalInvested
                });
            }
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
        Clock.instance.SetElapsedTime(saveData.elapsedTime); Clock.instance.InitializeClockUI(); // refresh UI

        // Load tutorial progress
        if (!string.IsNullOrEmpty(saveData.currentTutorialStep))
        {
            if (System.Enum.TryParse<TutorialStep>(saveData.currentTutorialStep, out TutorialStep loadedStep))
            {
                TutorialManager.instance.currentStep = loadedStep;
            }
        }

        // Load portfolio
        if (PortfolioManager.instance != null)
        {
            PortfolioManager.instance.ClearHoldings();
            foreach (var holdingData in saveData.stockHoldings)
            {
                StockData stock = FindStockBySymbol(holdingData.stockSymbol);
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

        Debug.Log("Game loaded!");
        return true;
    }

    private StockData FindStockBySymbol(string symbol)
    {
        // Option 1: Keep a reference to all stocks in a manager
        // Option 2: Search in Resources (if you store stocks in Resources folder)

        // For now, assume you have a StockManager
        if (StockManager.instance != null)
        {
            return StockManager.instance.GetStockBySymbol(symbol);
        }

        Debug.LogWarning($"Stock '{symbol}' not found during load!");
        return null;
    }
}