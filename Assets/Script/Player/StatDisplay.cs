using TMPro;
using UnityEngine;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] private StatType statType;
    [SerializeField] private string format = "{0}: {1:0}"; // "Hunger: 75"
    
    private TMP_Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.LogError("StatDisplay requires TMP_Text!", this);
            enabled = false;
            return;
        }

        // Register for updates
        PlayerStats.OnStatChanged += OnStatChanged;
        
        // Initial update
        RefreshText();
    }

    void OnStatChanged(StatType stat, float current, float max)
    {
        if (stat == statType)
            RefreshText();
    }

    void RefreshText()
    {
        float current = PlayerStats.instance.GetStat(statType);
        string displayName = statType.ToString();
        textComponent.text = string.Format(format, displayName, current);
    }

    void OnDestroy()
    {
        PlayerStats.OnStatChanged -= OnStatChanged;
    }
}