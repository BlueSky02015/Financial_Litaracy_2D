using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FadeAnimatorTrigger : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator missing!");
        }
    }

    public void PlayFade()
    {
        if (animator != null)
        {
            animator.SetTrigger("PlayFade");
        }
        
    }
}