using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestmentItemUI : MonoBehaviour
{
    [Header("References")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    public TMP_Text ownedText;
    public Button buyButton;
    public Button sellButton;
    public Button rentButton;

    private InvestmentAsset asset;

    public void Initialize(InvestmentAsset asset)
    {
        this.asset = asset;
        icon.sprite = asset.icon;
        nameText.text = asset.assetName;
        descriptionText.text = asset.description;
        priceText.text = $"${asset.purchasePrice}";

        RefreshUI();
        SetupListeners();
    }

    void SetupListeners()
    {
        buyButton.onClick.AddListener(Buy);
        sellButton.onClick.AddListener(Sell);
        rentButton.onClick.AddListener(ToggleRent);
        InvestmentManager.OnInvestmentsChanged += RefreshUI;
    }

    void Buy()
    {
        if (InvestmentManager.instance.BuyAsset(asset))
        {
            RefreshUI();
        }
    }

    void Sell()
    {
        InvestmentManager.instance.SellAsset(asset);
    }

    void ToggleRent()
    {
        InvestmentManager.instance.ToggleRent(asset);
    }

    void RefreshUI()
    {
        var holding = InvestmentManager.instance.Investments.Find(i => i.asset == asset);
        int owned = holding?.ownedCount ?? 0;
        bool isRented = holding?.isRented ?? false;

        ownedText.text = $"Owned: {owned}";
        sellButton.interactable = owned > 0;
        rentButton.interactable = owned > 0;

        // Visual feedback for rented
        rentButton.GetComponent<Image>().color = isRented ? asset.rentedColor : Color.white;
        rentButton.transform.GetChild(0).GetComponent<TMP_Text>().text = isRented ? "Stop Rent" : "Rent";
    }

    void OnDestroy()
    {
        InvestmentManager.OnInvestmentsChanged -= RefreshUI;
    }
}