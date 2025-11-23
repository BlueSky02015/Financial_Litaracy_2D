using UnityEngine;
using UnityEngine.UI;

public class WheelSelection : MonoBehaviour
{
    [Header("Buttons & Actions")]
    public Button[] buttons;
    public WheelAction[] wheelActions;

    [Header("Layout")]
    public float radius = 200f;
    public Vector2 centerOffset = Vector2.zero;
    [SerializeField] private WheelFeedback wheelFeedback;


    void Start()
    {
        ArrangeButtonsInCircle();
        SetupButtonActions();
    }

    void OnValidate()
    {
        ArrangeButtonsInCircle();
    }

    void ArrangeButtonsInCircle()
    {
        if (buttons == null || buttons.Length == 0)
            return;

        float angleStep = 360f / buttons.Length;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
                continue;

            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(radians) * radius;
            float y = Mathf.Cos(radians) * radius;

            RectTransform btnTransform = buttons[i].GetComponent<RectTransform>();
            if (btnTransform != null)
            {
                btnTransform.anchoredPosition = new Vector2(x, y) + centerOffset;
                btnTransform.localEulerAngles = new Vector3(0, 0, -angle);

                foreach (RectTransform child in btnTransform)
                {
                    child.localEulerAngles = new Vector3(0, 0, angle);
                }
            }
        }
    }

    void SetupButtonActions()
    {
        if (buttons == null || wheelActions == null)
            return;

        for (int i = 0; i < buttons.Length && i < wheelActions.Length; i++)
        {
            int index = i; // capture for closure
            buttons[i].onClick.AddListener(() => ExecuteAction(index));
        }
    }

    void ExecuteAction(int index)
    {
        if (index >= wheelActions.Length || wheelActions[index] == null)
            return;

        WheelAction action = wheelActions[index];

        // 🔒 CHECK REQUIREMENTS (in separate file!)
        if (!ActionRequirementsChecker.CanUseAction(action, showMessages: true))
        {
            return; // ⛔ Blocked!
        }

        WheelOutcome outcome = action.GetRandomWeightedOutcome();

        // ⏳ ADVANCE IN-GAME TIME
        if (Clock.instance != null)
        {
            Clock.instance.AddTimeHours(outcome.timeCostHours);
        }

        // Apply stats
        PlayerStats.instance.ModifyStat(StatType.Health, outcome.HealthChange);
        PlayerStats.instance.ModifyStat(StatType.Hunger, outcome.hungerChange);
        PlayerStats.instance.ModifyStat(StatType.Stamina, outcome.staminaChange);
        PlayerStats.instance.ModifyStat(StatType.Mood, outcome.moodChange);
        PlayerStats.instance.ModifyStat(StatType.Knowledge, outcome.knowledgeChange);
        PlayerStats.instance.ModifyStat(StatType.Money, outcome.moneyChange);

        // Determine color based on rarity
        Color rarityColor = outcome.rarity switch
        {
            RarityTier.Common => Color.white,
            RarityTier.Uncommon => new Color(0.3f, 0.7f, 1f),    // blue
            RarityTier.Rare => new Color(0.7f, 0.3f, 1f),        // purple
            RarityTier.Legendary => new Color(1f, 0.84f, 0f),    // gold
            _ => Color.white
        };

        if (wheelFeedback != null)
        {
            wheelFeedback.ShowFeedback(
                outcome.description,
                outcome.HealthChange,
                outcome.hungerChange,
                outcome.staminaChange,
                outcome.moodChange,
                outcome.knowledgeChange,
                outcome.moneyChange,
                rarityColor
            );
        }
    }

    public void SetInteractable(bool interactable)
    {
        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.interactable = interactable;
        }
    }

}