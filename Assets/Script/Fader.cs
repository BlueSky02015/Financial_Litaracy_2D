using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public FadeType fadeType;
    private Material _material;
    private Image _image;
    private int _fadeAmount = Shader.PropertyToID("_FadeAmount");

    private int _fadeNormal = Shader.PropertyToID("_FadeNormal");
    private int _fadeSleep = Shader.PropertyToID("_FadeSleep");
    private int _fadeGoOut = Shader.PropertyToID("_FadeGoOut");

    private int? _lastFadeEffect;
    public enum FadeType
    {
        _FadeNormal,
        _FadeSleep,
        _FadeGoOut
    }

    void Awake()
    {
        _image = GetComponent<Image>();

        Material Mat = _image.material;
        _image.material = new Material(Mat);
        _material = _image.material;

        _lastFadeEffect = _fadeNormal;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Fadein(fadeType);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Fadeout(fadeType);
        }
    }

    public void Fadein(FadeType fadeType)
    {
        Changefadeeffect(fadeType);
        StartFadeIn();
    }

    public void Fadeout(FadeType fadeType)
    {
        Changefadeeffect(fadeType);
        StartFadeOut();
    }

    void Changefadeeffect(FadeType fadeType)
    {
        if (_lastFadeEffect.HasValue)
        {
            _material.SetFloat((int)_lastFadeEffect, 0);
        }

        switch (fadeType)
        {
            case FadeType._FadeNormal:
                SwitchFadeEffect(_fadeNormal);
                break;
            case FadeType._FadeSleep:
                SwitchFadeEffect(_fadeSleep);
                break;
            case FadeType._FadeGoOut:
                SwitchFadeEffect(_fadeGoOut);
                break;
        }
    }

    void SwitchFadeEffect(int fadeEffect)
    {
        _material.SetFloat(fadeEffect, 1);
        _lastFadeEffect = fadeEffect;
    }

    void StartFadeIn()
    {
        _material.SetFloat(_fadeAmount, 1f);
        StartCoroutine(HandleFade(0f,1f));
    }

    void StartFadeOut()
    {
        _material.SetFloat(_fadeAmount, 0f);
        StartCoroutine(HandleFade(1f,0f));
    }
    
    private IEnumerator HandleFade(float StartAmount, float TargetAmount)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeAmount = Mathf.Lerp(StartAmount, TargetAmount, (elapsedTime / fadeDuration));
            _material.SetFloat(_fadeAmount, fadeAmount);

            yield return null;
        }

        _material.SetFloat(_fadeAmount, TargetAmount);
    }
}
