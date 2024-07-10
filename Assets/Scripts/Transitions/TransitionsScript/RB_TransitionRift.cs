using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_TransitionRift : RB_Transition
{
    [Header("Parameters")]
    [SerializeField] private Material _material;

    void Start()
    {
        StartCoroutine(Rift(NextSceneID, Duration));
    }

    public IEnumerator Rift(int sceneId, float duration, SPEEDTYPES speedType = SPEEDTYPES.Linear)
    {
        float baseValue = FadeIn ? -.1f : 0.7f;
        _material.SetFloat("_MaskAmount", baseValue);
        float targetValue = FadeIn ? 0.7f : -.1f;

        yield return StartCoroutine(FadeMaterial(_material, targetValue, duration * 0.5f, speedType)); // Fade in for half the duration.

        FinishedTransition = true;
    }

    private IEnumerator FadeMaterial(Material material, float targetValue, float duration, SPEEDTYPES speedType)
    {
        
        float maskAmount = material.GetFloat("_MaxAmount");
        float startValue = maskAmount;
        float startTime = Time.unscaledTime;

        while (Mathf.Abs(targetValue - maskAmount) > 0.01f)
        {
            float elapsedTime = (Time.unscaledTime - startTime) / duration;
            maskAmount = Mathf.Lerp(startValue, targetValue, RB_SceneTransitionManager.Instance.SpeedCurves[speedType].Evaluate(elapsedTime)); // avec le speed value
            material.SetFloat("_MaskAmount", maskAmount);
            yield return null;
        }

        material.SetFloat("_MaskAmount", targetValue);
    }
}