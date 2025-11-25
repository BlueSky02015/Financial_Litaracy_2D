// PredictionTradingUI.cs (direct reference version)
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictionTradingUI : MonoBehaviour
{
    [Header("Market Info")]
    [SerializeField] private TMP_Text marketNameText;
    [SerializeField] private TMP_Text currentPriceText;
    [SerializeField] private TMP_Text timeLeftText;

    [Header("Trade Controls")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TMP_InputField lotAmountInput;

    [Header("Profit Calculator")]
    [SerializeField] private TMP_Text profitCalculatorText;

    [Header("References")]
    [SerializeField] private StockData stockData; 

    private float currentMarketPrice = 0f;

    void Start()
    {
        if (stockData == null)
        {
            Debug.LogError("StockData not assigned in PredictionTradingUI!", this);
            return;
        }

        SetupButtonListeners();
        RefreshUI();
    }

    void Update()
    {
        UpdateCurrentPrice();
        UpdateTimeDisplay();
    }

    void SetupButtonListeners()
    {
        buyButton.onClick.AddListener(() => PlaceTrade(true));
        sellButton.onClick.AddListener(() => PlaceTrade(false));
    }

    void UpdateCurrentPrice()
    {
        if (stockData == null) return;

        if (stockData.currentPrice > 0)
        {
            currentMarketPrice = stockData.currentPrice;
            if (currentPriceText != null)
            {
                currentPriceText.text = $"Current: ${currentMarketPrice:F2}";
            }
        }
    }

    void PlaceTrade(bool isLong)
    {
        if (!int.TryParse(lotAmountInput.text, out int lots) || lots <= 0)
        {
            Debug.Log("Invalid lot amount!");
            return;
        }

        if (currentMarketPrice <= 0 || stockData == null)
        {
            Debug.Log("No current price available!");
            return;
        }

        // Check if already have an active trade
        if (PredictionTradingManager.instance.HasActiveTrade(stockData.stockName))
        {
            Debug.Log($"Already have an active trade for {stockData.stockName}!");
            return;
        }

        if (PredictionTradingManager.instance.PlaceTrade(stockData, isLong, lots))
        {
            lotAmountInput.text = "";
            RefreshUI();
        }
    }

    void UpdateTimeDisplay()
    {
        if (stockData == null) return;

        var activeTrade = PredictionTradingManager.instance.GetActiveTrade(stockData.stockName);
        if (activeTrade != null && !activeTrade.isResolved)
        {
            float timePassed = Clock.instance.elapsedTime - activeTrade.entryTime;
            float timeLeft = activeTrade.tradeDuration - timePassed;
            
            if (timeLeft > 0)
            {
                int minutesLeft = Mathf.CeilToInt(timeLeft / 60f);
                timeLeftText.text = $"Time left: {minutesLeft}m";
            }
            else
            {
                timeLeftText.text = "Resolving...";
            }
        }
        else
        {
            timeLeftText.text = "Ready to trade!";
        }
    }

    public void RefreshUI()
    {
        if (stockData == null) return;

        // Update market info
        marketNameText.text = stockData.stockName;

        // Check for active trade and update button states
        bool hasActiveTrade = PredictionTradingManager.instance.HasActiveTrade(stockData.stockName);
        
        if (hasActiveTrade)
        {
            // Disable both buttons while trade is active
            buyButton.interactable = false;
            sellButton.interactable = false;
        }
        else
        {
            // Enable both buttons when no active trade
            buyButton.interactable = true;
            sellButton.interactable = true;
        }

        // Update profit calculator
        UpdateProfitCalculator();
    }

    void UpdateProfitCalculator()
    {
        if (!int.TryParse(lotAmountInput.text, out int lots) || lots <= 0 || currentMarketPrice <= 0)
        {
            profitCalculatorText.text = "Enter lots to calculate profit";
            return;
        }

        float totalInvestment = lots * 10f;

        // Calculate potential profit/loss for different exit prices
        float profitAtPlus5 = (currentMarketPrice + 5f - currentMarketPrice) * lots * 10f;
        float profitAtMinus5 = (currentMarketPrice - 5f - currentMarketPrice) * lots * 10f;

        profitCalculatorText.text = $"Investment: ${totalInvestment:F2}\n" +
                                  $"+5: ${profitAtPlus5:F2} profit\n" +
                                  $"-5: ${profitAtMinus5:F2} loss";
    }

    // Remove the old FindStockBySymbol method - no longer needed
}