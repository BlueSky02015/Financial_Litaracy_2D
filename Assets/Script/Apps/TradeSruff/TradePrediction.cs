// TradePrediction.cs (independent version)
using System;

[Serializable]
public class TradePrediction
{
    public string marketSymbol;     // e.g., "GAME", "GOLD", "BTC"
    public float entryPrice;        // Price when trade opened
    public int lots;               // Number of lots (1 lot = $10)
    public float lotValue = 10f;   // Value per lot
    public bool isLong;            // True = BUY (bet UP), False = SELL (bet DOWN)
    public float entryTime;        // Clock.elapsedTime when trade opened
    public float tradeDuration = 3600f; // 1 hour in seconds
    public bool isResolved = false;
    public float exitPrice;        // Price when trade closed
    public float profitLoss;       // Final profit/loss amount

    public StockData stockReference;
}