// WheelSelection.cs (updated)
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log($"Executing: {action.actionName}");

        // Apply stat changes
        PlayerStats.instance.ModifyStat(StatType.Money, action.moneyChange);
        PlayerStats.instance.ModifyStat(StatType.Hunger, action.hungerChange);
        PlayerStats.instance.ModifyStat(StatType.Stamina, action.staminaChange);
        PlayerStats.instance.ModifyStat(StatType.Mood, action.moodChange);

        // Optional: show feedback (e.g., pop-up text)
        ShowFeedback(action);
    }

    void ShowFeedback(WheelAction action)
    {
        // You can add a TMP_Text or particle effect here later
        Debug.Log($"{action.actionName}: Money {action.moneyChange:+0;-0}, Hunger {action.hungerChange:+0;-0}, Stamina {action.staminaChange:+0;-0}, Mood {action.moodChange:+0;-0}");
    }
}