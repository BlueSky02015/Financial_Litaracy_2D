using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BedInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private FadeAnimatorTrigger fadeTrigger;
    [SerializeField] private float fadeDuration = 2f;

    public void OnClicked()
    {
        Debug.Log("Bed was clicked!");

        // Prevent multiple triggers (optional)
        if (fadeTrigger == null) return;

        fadeTrigger.PlayFade();
        StartCoroutine(DoAfterFade());
    }

    private System.Collections.IEnumerator DoAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);
        DoBedAction();
    }

    private void DoBedAction()
    {
        if (Clock.instance != null)
        {
            Clock.instance.JumpToMorning();
            GameSaveManager.instance?.SaveGame();
        }
    }
}