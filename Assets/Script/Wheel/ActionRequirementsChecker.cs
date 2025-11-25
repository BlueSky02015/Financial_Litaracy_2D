// ActionRequirementsChecker.cs
using UnityEngine;

public static class ActionRequirementsChecker
{
    public static bool CanUseAction(WheelAction action, bool showMessages = true)
    {
        if (action == null)
            return false;

        // 🔑 Knowledge requirement
        int requiredKnowledge = action.outcomes[0].knowledgeReq;
        int currentKnowledge = Mathf.RoundToInt(PlayerStats.instance.GetStat(StatType.Knowledge));

        if (currentKnowledge < requiredKnowledge)
        {
            if (showMessages)
            {
                Debug.Log($"[Action Locked] '{action.actionName}' requires {requiredKnowledge} Knowledge. You have {currentKnowledge}.");
            }
            return false;
        }

        // All req passed
        return true;
    }
}