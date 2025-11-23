using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DeathAnimatorTrigger : MonoBehaviour
{
    private Animator animator;
    public AnimationClip deathAnimationClip;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator missing!");
        }
    }

    public void PlayDeath()
    {
        if (animator != null)
        {
            animator.Play(deathAnimationClip.name);
        }
        
    }
}