using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_TransitionFade : RB_Transition
{
    [Header("Parameters")]
    [SerializeField] private Image _fadeImage;
    public float _duration = 5f;
    //private float fadeInDuration = FadeInTime > 0 ? FadeInTime : _duration;

    void Awake()
    {
        if (_fadeImage.color.a != 0)
        {
            Color newColor = _fadeImage.color;
            newColor.a = 0;
            _fadeImage.color = newColor;
        }
    }

    void Start()
    {
        StartCoroutine(Fade("SampleScene", _duration, speedType : RB_SceneTransitionManager.Instance.SpeedType));
    }

    public override IEnumerator Fade(string nameScene, float duration, SPEEDTYPES speedType)
    {
        return base.Fade(nameScene, duration, speedType);
    }

    public override IEnumerator FadeIn(float duration, SPEEDTYPES speedType)
    {
        float targetAlpha = 1f;
        float startAlpha = _fadeImage.color.a;
        float startTime = Time.time;

        while (_fadeImage.color.a < 1f) // Continue fading in until the alpha is 1.
        {
            float elapsedTime = (Time.time - startTime) / duration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, RB_SceneTransitionManager.Instance.SpeedCurves[speedType].Evaluate(elapsedTime));
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, newAlpha);
            yield return null;
        }
    }

    public override IEnumerator FadeOut(float duration, SPEEDTYPES speedType)
    {
        float targetAlpha = 0f;
        float startAlpha = _fadeImage.color.a;
        float startTime = Time.time;

        while (_fadeImage.color.a > 0f) // Continue fading out until the alpha is 0.
        {
            float elapsedTime = (Time.time - startTime) / duration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, RB_SceneTransitionManager.Instance.SpeedCurves[speedType].Evaluate(elapsedTime));
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, newAlpha);
            yield return null;
        }
    }
}