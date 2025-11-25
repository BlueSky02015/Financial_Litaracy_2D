using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("UI References")]
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public Animator animator;

    [Header("Settings")]
    public float defaultAutoAdvanceDelay = 2f;
    public float typingSpeed = 0.1f;

    private Queue<DialogueLine> lines;
    private DialogueSO currentDialogue;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine autoAdvanceCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        lines = new Queue<DialogueLine>();
    }

    void Update()
    {
        if (isDialogueActive && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogueLine();
        }
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        // ✅ No need to check — TutorialManager decides when to call this
        currentDialogue = dialogue;
        isDialogueActive = true;
        TutorialManager.instance.SetTutorialActive(true);
        animator.SetBool("IsDialog", true);
        lines.Clear();

        foreach (var line in dialogue.dialogueLines)
        {
            lines.Enqueue(line);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        var currentLine = lines.Dequeue();
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine line)
    {
        isTyping = true;
        dialogueArea.text = "";
        foreach (char letter in line.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        float delay = line.autoAdvanceDelay > 0 ? line.autoAdvanceDelay : defaultAutoAdvanceDelay;
        autoAdvanceCoroutine = StartCoroutine(AutoAdvance(delay));
    }

    IEnumerator AutoAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayNextDialogueLine();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        TutorialManager.instance.SetTutorialActive(false);
        animator.SetBool("IsDialog", false);
        
        currentDialogue = null;
    }
}