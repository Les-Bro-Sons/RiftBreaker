using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Cinemachine.PostFX;

public class RB_UXRewindManager : MonoBehaviour
{
    public static RB_UXRewindManager Instance;

    [SerializeField] private Volume _rewindVolume;

    [SerializeField] private float _durationToSwitch = 1f;

    private void Awake()
    {
        Instance = this;
    }
    public void StartRewindTransition()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRewindEffect(_durationToSwitch * 0.5f));
    }

    public void StopRewindTransition()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRewindEffect(_durationToSwitch * 0.5f));
    }


    private IEnumerator FadeInRewindEffect(float duration)
    {
        float elapsedTime = 0f;
        float startWeight = _rewindVolume.weight;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _rewindVolume.weight = Mathf.Lerp(startWeight, 1, t);

            yield return null;
        }

        _rewindVolume.weight = 1;
    }

    private IEnumerator FadeOutRewindEffect(float duration)
    {
        float elapsedTime = 0f;
        float startWeight = _rewindVolume.weight;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _rewindVolume.weight = Mathf.Lerp(startWeight, 0, t);

            yield return null;
        }

        _rewindVolume.weight = 0;
    }
}