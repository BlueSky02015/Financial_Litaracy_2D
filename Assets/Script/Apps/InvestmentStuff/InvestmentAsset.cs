using UnityEngine;

[CreateAssetMenu(fileName = "New Investment", menuName = "Investment/Investment Asset")]
public class InvestmentAsset : ScriptableObject
{
    [Header("Basic Info")]
    public string assetName = "Apartment";
    public Sprite icon;
    [TextArea] public string description = "A place to live or rent out.";

    [Header("Economy")]
    public int purchasePrice = 1000;
    public int sellPrice = 1200; // usually less than purchase
    public int weeklyRentalIncome = 50; // passive income if rented

    [Header("Visuals")]
    public Color rentedColor = new Color(0.2f, 0.8f, 0.4f); // green tint when rented
}