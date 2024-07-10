using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_TransitionRift : RB_Transition
{
    [Header("Parameters")]
    [SerializeField] private Material _material;

    [Header("Shader mask")]

    [SerializeField] private float _startAmount = -.1f;
    [SerializeField] private float _endAmount = 0.7f;

    void Start()
    {
        StartCoroutine(Rift(NextSceneID, Duration, SpeedType));
    }

    public IEnumerator Rift(int sceneId, float duration, SPEEDTYPES speedType = SPEEDTYPES.Linear)
    {
        float baseValue = FadeIn ? _startAmount : _endAmount;
        _material.SetFloat("_MaskAmount", baseValue);
        float targetValue = FadeIn ? _endAmount : _startAmount;

        yield return StartCoroutine(FadeMaterial(_material, targetValue, duration, speedType)); // Fade in for half the duration.

        FinishedTransition = true;
    }

    private IEnumerator FadeMaterial(Material material, float targetValue, float duration, SPEEDTYPES speedType)
    {
        
        float maskAmount = material.GetFloat("_MaskAmount");
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