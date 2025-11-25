using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;

    public float autoAdvanceDelay = 2f; 
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSO dialogue;

    [Header("Trigger Conditions")]
    public bool triggerOnStart = true;
    public bool triggerOnInteract = false; 

    void Start()
    {
        if (triggerOnStart)
        {
            TriggerDialogue();
        }
    }

    // Call this from BedInteraction, Laptop, etc.
    public void TriggerDialogue()
    {
        if (DialogManager.Instance != null && dialogue != null)
        {
            DialogManager.Instance.StartDialogue(dialogue);
        }
    }

    // Optional: Reset tutorial (for testing)
    void OnValidate()
    {
        if (Application.isPlaying && dialogue != null)
        {
            // Auto-reset in editor for testing
        }
    }
}