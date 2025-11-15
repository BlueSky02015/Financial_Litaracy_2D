using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [Header("Dialogue Lines")]
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    [Header("Tutorial Settings")]
    public string targetInteractableTag = ""; 
     public bool isIntroDialogue = false;
    public bool showOnlyOnce = true; 
    
}