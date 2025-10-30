using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine autoAdvanceCoroutine;


    float typingSpeed = 0.1f;
    public Animator animator;



    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
        isDialogueActive = false;
    }

    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned in DialogManager!");
        }
    }

    void Update()
    {
        if (isDialogueActive && !isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextDialogueLine();
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        animator.SetBool("IsDialog", true);
        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        // Debug.Log("Lines remaining: " + lines.Count);
        if (lines.Count == 0)
        {
            Debug.Log("No more lines. Ending dialogue.");
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        isTyping = true;
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        // Start the auto-advance coroutine
        autoAdvanceCoroutine = StartCoroutine(AutoAdvanceDialogue());
    }

    IEnumerator AutoAdvanceDialogue()
    {
        yield return new WaitForSeconds(2f);
        DisplayNextDialogueLine();
    }


    void EndDialogue()
    {
        isDialogueActive = false;
        animator.SetBool("IsDialog", false);
    }


}