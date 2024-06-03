using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class RB_UXRewindManager : MonoBehaviour
{
    public static RB_UXRewindManager Instance;

    [SerializeField] private Volume _rewindVolume;
    [SerializeField] private float _durationToSwitch = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    public void StartRewindTransition(bool fullRewind = false)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRewindEffect(_durationToSwitch * 0.5f));
        if (!fullRewind)
        {
            RB_UxHourglass.Instance.StartUseHourglassUx();
        }
        else
        {

        }
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