using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public StatSaveData stats = new StatSaveData();
    public float elapsedTime;
    public int currentDay;

    // Portfolio (Stock Holdings)
    public List<StockHoldingSaveData> stockHoldings = new List<StockHoldingSaveData>();

    //Tutorial Progress
    public List<string> completedAppTutorials = new List<string>();
    
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
    public string stockName; 
    public int sharesOwned;
    public float totalInvested;
}