using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class BaseTradingSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] protected WindowGraph windowGraph;
    [SerializeField] protected Button buyButton;
    [SerializeField] protected Button sellButton;
    [SerializeField] protected TextMeshProUGUI moneyText;
    [SerializeField] protected TextMeshProUGUI sharesText;
    [SerializeField] protected TextMeshProUGUI buyPriceText;
    [SerializeField] protected TextMeshProUGUI sellPriceText;
    [SerializeField] protected TextMeshProUGUI portfolioValueText;
    [SerializeField] protected TextMeshProUGUI gainLossText;

    [Header("Trading Settings")]
    [SerializeField] protected float startingMoney = 1000f;
    [SerializeField] protected AssetsData assetData;

    protected float currentMoney;
    protected int sharesOwned = 0;
    protected float startingPortfolioValue;
    protected bool marketIsOpen = true;

    protected virtual void Start()
    {
        currentMoney = startingMoney;
        startingPortfolioValue = currentMoney;
        assetData.Initialize();
        
        UpdateUI();
        UpdateGraph();

        buyButton.onClick.AddListener(Buy);
        sellButton.onClick.AddListener(Sell);
    }

    protected abstract void UpdatePrice();
    protected abstract void CheckMarketHours();

    protected virtual void Buy()
    {
        if (currentMoney >= assetData.currentMidPrice)
        {
            currentMoney -= assetData.currentMidPrice;
            sharesOwned++;
            UpdateUI();
        }
    }

    protected virtual void Sell()
    {
        if (sharesOwned > 0)
        {
            currentMoney += assetData.currentMidPrice;
            sharesOwned--;
            UpdateUI();
        }
    }

    protected virtual void UpdateUI()
    {
        float portfolioValue = currentMoney + sharesOwned * assetData.currentMidPrice;
        float gainLoss = portfolioValue - startingPortfolioValue;
        float gainLossPercent = (gainLoss / startingPortfolioValue) * 100f;

        moneyText.text = $"Cash: ${currentMoney:F2}";
        sharesText.text = $"Shares: {sharesOwned}";
        buyPriceText.text = $"Buy: ${assetData.currentMidPrice:F2}";
        sellPriceText.text = $"Sell: ${assetData.currentMidPrice:F2}";
        portfolioValueText.text = $"Portfolio: ${portfolioValue:F2}";
        
        gainLossText.text = $"{gainLossPercent:F1}%";
        gainLossText.color = gainLoss >= 0 ? Color.green : Color.red;

        buyButton.interactable = currentMoney >= assetData.currentMidPrice && marketIsOpen;
        sellButton.interactable = sharesOwned > 0 && marketIsOpen;
    }

    protected virtual void UpdateGraph()
    {
        windowGraph.UpdateGraphData(assetData.priceHistory);
    }
}