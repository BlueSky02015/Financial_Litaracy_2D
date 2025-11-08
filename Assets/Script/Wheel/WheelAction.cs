using UnityEngine;

[CreateAssetMenu(fileName = "New Wheel Action", menuName = "Game/Wheel Action")]
public class WheelAction : ScriptableObject
{
    [Header("Action Info")]
    public string actionName = "Unnamed";
    [TextArea] public string description = "No description";

    [Header("Stat Changes")]
    public int moneyChange = 0;
    public int hungerChange = 0;
    public int staminaChange = 0;
    public int moodChange = 0;
}