using UnityEngine;

public class InvestmentPanel : MonoBehaviour
{
    [SerializeField] private InvestmentAsset[] availableAssets;  
    [SerializeField] private InvestmentItemUI itemTemplate;
    [SerializeField] private Transform contentPanel;

    void Start()
    {
        CreateInvestmentItems();
        InvestmentManager.OnInvestmentsChanged += CreateInvestmentItems;
    }

    void CreateInvestmentItems()
    {
        // Clear existing
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        // Create new
        foreach (var asset in availableAssets)
        {
            var item = Instantiate(itemTemplate, contentPanel);
            item.Initialize(asset);
        }
    }

    void OnDestroy()
    {
        InvestmentManager.OnInvestmentsChanged -= CreateInvestmentItems;
    }
}