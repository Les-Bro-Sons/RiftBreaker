using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_TransitionFade : RB_Transition
{
    [Header("Parameters")]
    [SerializeField] private Image _fadeImage;

    void Start()
    {
        StartCoroutine(Fade(NextSceneID, Duration));
    }

    public IEnumerator Fade(int nameScene, float duration, SPEEDTYPES speedType = SPEEDTYPES.Linear)
    {
        float baseValue = FadeIn ? 0 : 1;
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, baseValue);

        yield return StartCoroutine(FadeImage(_fadeImage, FadeIn, duration * 0.5f, speedType)); // Fade in for half the duration.

        FinishedTransition = true;
    }

    public IEnumerator FadeImage(Image image, bool fadeIn, float duration, SPEEDTYPES speedType)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = image.color.a;
        float startTime = Time.unscaledTime;

        while (Mathf.Abs(image.color.a - targetAlpha) > 0.01f)
        {
            float elapsedTime = (Time.unscaledTime - startTime) / duration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, RB_SceneTransitionManager.Instance.SpeedCurves[speedType].Evaluate(elapsedTime));
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
    }
}