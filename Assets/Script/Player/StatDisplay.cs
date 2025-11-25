// StatDisplay.cs (fixed)
using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] private StatType statType;
    [SerializeField] private string format = "{0}: {1:0}";

    private TMP_Text textComponent;
    private bool isInitialized = false;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("StatDisplay requires TMP_Text!", this);
            enabled = false;
            return;
        }

        // Try to initialize now
        TryInitialize();
    }

    void OnEnable()
    {
        // Re-initialize if enabled after being disabled
        TryInitialize();
    }

    void TryInitialize()
    {
        if (isInitialized) return;

        if (PlayerStats.instance == null)
        {
            return;
        }

        PlayerStats.OnStatChanged += OnStatChanged;
        RefreshText();
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
        {
            TryInitialize();
        }
    }

    void OnStatChanged(StatType stat, float current, float max)
    {
        if (stat == statType)
            RefreshText();
    }

    void RefreshText()
    {
        if (PlayerStats.instance == null)
        {
            textComponent.text = $"{statType}: ???";
            return;
        }

        float current = PlayerStats.instance.GetStat(statType);
        string displayName = statType.ToString();
        textComponent.text = string.Format(format, displayName, current);
    }

    void OnDisable()
    {
        if (isInitialized)
        {
            PlayerStats.OnStatChanged -= OnStatChanged;
        }
    }

    void OnDestroy()
    {
        if (isInitialized)
        {
            PlayerStats.OnStatChanged -= OnStatChanged;
        }
    }
}