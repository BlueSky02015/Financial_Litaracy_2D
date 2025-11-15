// TutorialManager.cs (mouse-only version)
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public TutorialStep currentStep = TutorialStep.None;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        currentStep = TutorialStep.IntroStory;
        SaveProgress();
        ShowCurrentStepDialogue();
    }

    // === CLICK-BASED TRIGGERS ===
    public void OnLaptopClicked()
    {
        if (currentStep == TutorialStep.ChooseInteraction)
        {
            Debug.Log("✅ Advancing to UseLaptop");
            AdvanceTo(TutorialStep.UseLaptop); // ← This must run
        }
    }

    public void OnDoorClicked()
    {
        if (currentStep == TutorialStep.ChooseInteraction)
        {
            AdvanceTo(TutorialStep.UseDoor);
        }
    }

    public void OnEmailAppOpened() => CompleteTutorial();
    public void OnWentOutside() => CompleteTutorial();

    // === CORE ===
    private void AdvanceTo(TutorialStep step)
    {
        currentStep = step;
        SaveProgress();
        ShowCurrentStepDialogue();
    }

    private void CompleteTutorial()
    {
        AdvanceTo(TutorialStep.Completed);
    }

    private void ShowCurrentStepDialogue()
    {
        DialogueSO dialogue = GetDialogueForStep(currentStep);
        if (dialogue != null)
        {
            DialogManager.Instance.StartDialogue(dialogue);
        }
    }

    private DialogueSO GetDialogueForStep(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.IntroStory: return introStoryDialogue;
            case TutorialStep.ChooseInteraction: return chooseInteractionDialogue;
            case TutorialStep.UseLaptop: return useLaptopDialogue;
            case TutorialStep.CheckEmail: return checkEmailDialogue;
            case TutorialStep.UseDoor: return useDoorDialogue;
            case TutorialStep.GoOutside: return goOutsideDialogue;
            default: return null;
        }
    }
    public void AdvanceToChoosePath()
    {
        AdvanceTo(TutorialStep.ChooseInteraction);
    }

    // === SAVE/LOAD ===
    private const string TUTORIAL_KEY = "TutorialStep";

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(TUTORIAL_KEY, (int)currentStep);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey(TUTORIAL_KEY))
        {
            currentStep = (TutorialStep)PlayerPrefs.GetInt(TUTORIAL_KEY);
        }
    }

    // === PUBLIC API ===
    public bool IsTutorialActive() =>
        currentStep != TutorialStep.Completed && currentStep != TutorialStep.None;

    // Assign in Inspector
    public DialogueSO introStoryDialogue;
    public DialogueSO chooseInteractionDialogue;
    public DialogueSO useLaptopDialogue;
    public DialogueSO checkEmailDialogue;
    public DialogueSO useDoorDialogue;
    public DialogueSO goOutsideDialogue;
}