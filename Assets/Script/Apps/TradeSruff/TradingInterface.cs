using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradingInterface : MonoBehaviour
{
    [Header("Trading Core")]
    [SerializeField] private TradingManager tradingManager; 

    [Header("Stock Settings")]
    [SerializeField] private string stockName = "GAME";

    [Header("UI - Buy/Sell")]
    [SerializeField] private TMP_InputField InputField;
    [SerializeField] private TMP_Text cashText;
    [SerializeField] private TMP_Text sharesText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;

    private int sharesOwned = 0;

    void Start()
    {
        if (tradingManager == null)
        {
            Debug.LogError("TradingInterface: TradingManager not assigned!", this);
            enabled = false;
            return;
        }

        RefreshUI();
        SetupEventListeners();
    }

    void SetupEventListeners()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        PlayerStats.OnStatChanged += OnStatChanged;
    }

    void OnStatChanged(StatType stat, float current, float max)
    {
        if (stat == StatType.Money)
        {
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        // Update cash
        float cash = PlayerStats.instance.GetStat(StatType.Money);
        if (cashText != null)
            cashText.text = $"Cash: ${Mathf.RoundToInt(cash)}";

        // Update shares
        if (sharesText != null)
            sharesText.text = $"Shares: {sharesOwned}";

        // Update button states
        int currentPrice = tradingManager.GetCurrentPrice();
        buyButton.interactable = CanBuy(1, currentPrice);
        sellButton.interactable = sharesOwned > 0;
    }

    public void OnBuyClicked()
    {
        if (!int.TryParse(InputField.text, out int amount) || amount <= 0)
        {
            Debug.Log("Invalid buy amount");
            return;
        }

        int price = tradingManager.GetCurrentPrice();
        if (!CanBuy(amount, price))
        {
            Debug.Log("Not enough money!");
            return;
        }

        // Deduct money
        PlayerStats.instance.ModifyStat(StatType.Money, -amount * price);
        sharesOwned += amount;

        InputField.text = "";
        Debug.Log($"Bought {amount} shares of {stockName} at ${price}");
        RefreshUI();
    }

    public void OnSellClicked()
    {
        if (!int.TryParse(InputField.text, out int amount) || amount <= 0)
        {
            Debug.Log("Invalid sell amount");
            return;
        }

        if (amount > sharesOwned)
        {
            Debug.Log("Not enough shares!");
            return;
        }

        int price = tradingManager.GetCurrentPrice();

        // Add money
        PlayerStats.instance.ModifyStat(StatType.Money, +amount * price);
        sharesOwned -= amount;

        InputField.text = "";
        Debug.Log($"Sold {amount} shares of {stockName} at ${price}");
        RefreshUI();
    }

    private bool CanBuy(int amount, int price)
    {
        float cash = PlayerStats.instance.GetStat(StatType.Money);
        return cash >= amount * price;
    }

    void OnDestroy()
    {
        PlayerStats.OnStatChanged -= OnStatChanged;
    }
}