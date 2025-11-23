// DebtSystem.cs
using UnityEngine;

public class DebtSystem : MonoBehaviour
{
    public static DebtSystem instance;

    [Header("Debt Settings")]
    [SerializeField] private float startingDebt = 500f;
    [SerializeField] private float monthlyInterestRate = 0.01f; // 1%
    [SerializeField] private string debtQuestDescription = "Pay off your debt.";

    private float currentDebt;
    private bool isQuestCompleted = false;

    [Header("Time Tracking")]
    private float lastInterestTime = -1f;
    private const float MONTH_SECONDS = 30 * 24 * 3600f; // 30 days

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load from save later — for now, start fresh
        currentDebt = startingDebt;
        lastInterestTime = Clock.instance?.elapsedTime ?? 0f;
        isQuestCompleted = currentDebt <= 0;
    }

    void Update()
    {
        if (Clock.instance == null) return;
        ApplyMonthlyInterest();
    }

    void ApplyMonthlyInterest()
    {
        if (lastInterestTime == -1 || currentDebt <= 0) return;

        float currentTime = Clock.instance.elapsedTime;
        if (currentTime - lastInterestTime >= MONTH_SECONDS)
        {
            // Calculate full months passed
            float monthsPassed = (currentTime - lastInterestTime) / MONTH_SECONDS;
            int fullMonths = Mathf.FloorToInt(monthsPassed);

            if (fullMonths > 0)
            {
                currentDebt *= Mathf.Pow(1f + monthlyInterestRate, fullMonths);
                lastInterestTime += fullMonths * MONTH_SECONDS;
                Debug.Log($"[Debt] Interest applied. New debt: ${currentDebt:0.00}");
            }
        }
    }

    // --- Public API ---
    public bool PayDebt(float amount)
    {
        if (amount <= 0 || PlayerStats.instance.GetStat(StatType.Money) < amount)
            return false;

        // Deduct money
        PlayerStats.instance.ModifyStat(StatType.Money, -amount);

        // Reduce debt
        currentDebt = Mathf.Max(0, currentDebt - amount);
        isQuestCompleted = currentDebt <= 0;

        if (isQuestCompleted)
        {
            OnDebtQuestCompleted?.Invoke();
        }

        OnDebtChanged?.Invoke();
        return true;
    }

    public void ResetDebt()
    {
        currentDebt = startingDebt;
        lastInterestTime = Clock.instance?.elapsedTime ?? 0f;
        Debug.Log($"💳 Debt reset to: ${currentDebt}");
    }


    public float GetCurrentDebt() => currentDebt;
    public bool IsQuestCompleted() => isQuestCompleted;
    public string GetQuestDescription() => debtQuestDescription;

    // --- Events ---
    public delegate void DebtChangedHandler();
    public static event DebtChangedHandler OnDebtChanged;

    public delegate void DebtQuestCompletedHandler();
    public static event DebtQuestCompletedHandler OnDebtQuestCompleted;
}