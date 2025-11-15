using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public StatSaveData stats = new StatSaveData();
    public float elapsedTime;

    // Portfolio (Stock Holdings)
    public List<StockHoldingSaveData> stockHoldings = new List<StockHoldingSaveData>();

    //Tutorial Progress
    public string currentTutorialStep = "None"; // store as string for safety
    
}

[System.Serializable]
public class StatSaveData
{
    public float health;
    public float hunger;
    public float stamina;
    public float mood;
    public float money;
    public float knowledge;
}

[System.Serializable]
public class StockHoldingSaveData
{
    public string stockSymbol; 
    public int sharesOwned;
    public float totalInvested;
}