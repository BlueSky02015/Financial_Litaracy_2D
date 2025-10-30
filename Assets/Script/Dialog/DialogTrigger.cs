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
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogWarning("DialogManager instance is null. Cannot start dialogue.");
        }

    }

    void Start()
    {
        TriggerDialogue();
    }
}
