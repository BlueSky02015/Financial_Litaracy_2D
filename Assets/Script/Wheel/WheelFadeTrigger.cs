using System.Collections;
using UnityEngine;
public class WheelFadeTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip fadeOutClip;
    [SerializeField] private AnimationClip fadeInClip;

    public void FadeOut()
    {
        animator.SetTrigger("PlayFadeOut");
    }

    public void FadeIn()    
    {
        animator.SetTrigger("PlayFadeIn");
    }

    public IEnumerator WaitForFadeOut()
    {
        FadeOut();
        yield return new WaitForSeconds(fadeOutClip.length);
        Debug.Log(">>> FadeAnimatorTrigger.PlayFadeOut() called!");
    }

    public IEnumerator WaitForFadeIn()
    {
        FadeIn();
        yield return new WaitForSeconds(fadeInClip.length);
        Debug.Log(">>> FadeAnimatorTrigger.PlayFadeIn() called!");
    }
}