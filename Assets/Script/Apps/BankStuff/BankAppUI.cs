// BankAppUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankAppUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text cashText;
    public TMP_Text debtText;
    public TMP_InputField paymentInput;
    public Button payButton;
    public TMP_Text questText;

    void Start()
    {
        RefreshUI();
        SetupListeners();
    }

    void SetupListeners()
    {
        payButton.onClick.AddListener(OnPayButton);
        PlayerStats.OnStatChanged += OnStatChanged;
        DebtSystem.OnDebtChanged += RefreshUI;
        DebtSystem.OnDebtQuestCompleted += OnQuestCompleted;
    }

    void OnStatChanged(StatType stat, float current, float max)
    {
        if (stat == StatType.Money)
            RefreshUI();
    }

    void RefreshUI()
    {
        // Cash
        float cash = PlayerStats.instance.GetStat(StatType.Money);
        cashText.text = $"Cash: ${Mathf.RoundToInt(cash)}";

        // Debt
        float debt = DebtSystem.instance.GetCurrentDebt();
        debtText.text = $"Debt: ${debt:0.00}";
        payButton.interactable = debt > 0 && cash > 0;

        // Quest
        if (DebtSystem.instance.IsQuestCompleted())
        {
            questText.text = "<color=green>✅ Debt paid off!</color>";
        }
        else
        {
            questText.text = $"<b>MyDebt:</b> {DebtSystem.instance.GetQuestDescription()}";
        }
    }

    void OnPayButton()
    {
        if (!float.TryParse(paymentInput.text, out float amount) || amount <= 0)
        {
            Debug.Log("Invalid payment amount");
            return;
        }

        if (DebtSystem.instance.PayDebt(amount))
        {
            paymentInput.text = "";
            Debug.Log($"Paid ${amount} toward debt");
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    void OnQuestCompleted()
    {
        RefreshUI();
        // Optional: show celebration popup
    }

    void OnDestroy()
    {
        PlayerStats.OnStatChanged -= OnStatChanged;
        DebtSystem.OnDebtChanged -= RefreshUI;
        DebtSystem.OnDebtQuestCompleted -= OnQuestCompleted;
    }
}