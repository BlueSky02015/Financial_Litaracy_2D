using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Wheel Action", menuName = "Game/Wheel Action")]
public class WheelAction : ScriptableObject
{
    [Header("Action Name")]
    public string actionName = "Unnamed";

    [Header("Possible Outcomes")]
    public List<WheelOutcome> outcomes = new List<WheelOutcome>
    {
        new WheelOutcome
        {
            description = "Default outcome.",
            HealthChange = 0,
            hungerChange = 0,
            staminaChange = 0,
            moodChange = 0,
            knowledgeChange = 0,
            moneyChange = 0,
            knowledgeReq = 0,
        }
    };

    public WheelOutcome GetRandomWeightedOutcome()
    {
        if (outcomes == null || outcomes.Count == 0)
            return new WheelOutcome { description = "No outcomes available." };

        // Define weights for each tier
        float GetWeight(RarityTier tier)
        {
            return tier switch
            {
                RarityTier.Common => 60f,
                RarityTier.Uncommon => 25f,
                RarityTier.Rare => 10f,
                RarityTier.Legendary => 1f,
                _ => 0f
            };
        }

        // Build cumulative weight list
        List<float> weights = new List<float>();
        float totalWeight = 0f;

        foreach (var outcome in outcomes)
        {
            float weight = GetWeight(outcome.rarity);
            totalWeight += weight;
            weights.Add(totalWeight);
        }

        // Pick a random point
        float randomPoint = Random.Range(0f, totalWeight);

        // Find which outcome it lands on
        for (int i = 0; i < weights.Count; i++)
        {
            if (randomPoint <= weights[i])
            {
                return outcomes[i];
            }
        }

        // Fallback
        return outcomes[0];
    }
}

[System.Serializable]
public class WheelOutcome
{
    [TextArea(2, 4)]
    public string description = "Something happened.";
    public RarityTier rarity = RarityTier.Common;
    public int HealthChange = 0;
    public int hungerChange = 0;
    public int staminaChange = 0;
    public int moodChange = 0;
    public int knowledgeChange = 0;
    public float timeCostHours = 1f; // dont forget (0.5 = 30 minutes, 2 = 2 hours)
    public int moneyChange = 0;
    public int knowledgeReq = 0;
}