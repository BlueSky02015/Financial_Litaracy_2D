// DisclaimerManager.cs (minimal version)
using UnityEngine;

public class DisclaimerManager : MonoBehaviour
{
    [Header("Settings")]
    public float displayTime = 3f;

    private System.Action onContinueCallback;
    private bool disclaimerActive = false;

    public void StartCountdown(System.Action onContinue)
    {
        onContinueCallback = onContinue;
        disclaimerActive = true;
        
        // Start timer to continue automatically
        Invoke(nameof(ContinueToNext), displayTime);
    }

    void ContinueToNext()
    {
        if (!disclaimerActive) return;

        disclaimerActive = false;
        onContinueCallback?.Invoke();
    }
}