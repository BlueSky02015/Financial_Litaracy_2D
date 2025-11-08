// InvestmentUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestmentUI : MonoBehaviour
{
    [Header("Stock Reference")]
    [SerializeField] private StockData stock;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text stockNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text sharesText;
    [SerializeField] private TMP_Text profitText;
    [SerializeField] private TMP_InputField buyAmountInput;
    [SerializeField] private TMP_InputField sellAmountInput;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;

    void Start()
    {
        if (stock == null)
        {
            Debug.LogError("InvestmentUI: StockData not assigned!", this);
            return;
        }

        RefreshUI();
        SetupListeners();
    }

    void SetupListeners()
    {
        buyButton.onClick.AddListener(() => BuyStock());
        sellButton.onClick.AddListener(() => SellStock());
        PortfolioManager.OnPortfolioUpdated += RefreshUI;
        PlayerStats.OnStatChanged += OnStatChanged;
    }

    void OnStatChanged(StatType stat, float current, float max)
    {
        if (stat == StatType.Money)
            RefreshUI();
    }

    void RefreshUI()
    {
        // Basic info
        if (stockNameText != null)
            stockNameText.text = stock.stockName;
        
        if (priceText != null)
            priceText.text = $"Price: ${stock.currentPrice}";

        // Holdings
        var holding = PortfolioManager.instance.GetHolding(stock);
        int shares = holding?.sharesOwned ?? 0;
        if (sharesText != null)
            sharesText.text = $"Shares: {shares}";

        // Profit
        float profitPct = PortfolioManager.instance.GetProfitPercentage(stock);
        string profitColor = profitPct >= 0 ? "#4CAF50" : "#F44336"; // green/red
        if (profitText != null)
            profitText.text = $"<color={profitColor}>{profitPct:+0.00;-0.00}%</color>";

        // Button states
        buyButton.interactable = CanBuy(1);
        sellButton.interactable = shares > 0;
    }

    void BuyStock()
    {
        if (!int.TryParse(buyAmountInput.text, out int amount) || amount <= 0) return;
        if (PortfolioManager.instance.BuyStock(stock, amount, stock.currentPrice))
        {
            buyAmountInput.text = "";
        }
    }

    void SellStock()
    {
        if (!int.TryParse(sellAmountInput.text, out int amount) || amount <= 0) return;
        if (PortfolioManager.instance.SellStock(stock, amount))
        {
            sellAmountInput.text = "";
        }
    }

    bool CanBuy(int amount)
    {
        float cost = amount * stock.currentPrice;
        return PlayerStats.instance.GetStat(StatType.Money) >= cost;
    }

    void OnDestroy()
    {
        PortfolioManager.OnPortfolioUpdated -= RefreshUI;
        PlayerStats.OnStatChanged -= OnStatChanged;
    }
}