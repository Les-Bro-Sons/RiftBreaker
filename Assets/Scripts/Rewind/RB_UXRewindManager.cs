using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class RB_UXRewindManager : MonoBehaviour
{
    private Volume _volume;
    private ColorAdjustments _colorAdjustments;
    private FilmGrain _filmGrain;
    private WhiteBalance _whiteBalance;

    [SerializeField] private float _durationToSwitch = 1f;
    [SerializeField] private float _contrastValue = 50f;
    [SerializeField] private float _saturationValue = -100f;
    [SerializeField] private float _grainIntensityValue = 1f;
    [SerializeField] private float _whiteBalanceTempValue = -20;

    // Start is called before the first frame update
    void Start()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet(out _colorAdjustments);
        _volume.profile.TryGet(out _filmGrain);
        _volume.profile.TryGet(out _whiteBalance);
    }

    public void StartRewindTransition()
    {
        StartCoroutine(FadeInRewindEffect(_durationToSwitch * 0.5f));
    }

    public void StopRewindTransition()
    {
        StartCoroutine(FadeOutRewindEffect(_durationToSwitch * 0.5f));
    }


    private IEnumerator FadeInRewindEffect(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _colorAdjustments.contrast.value = Mathf.Lerp(0, _contrastValue, t);
            _colorAdjustments.saturation.value = Mathf.Lerp(0, _saturationValue, t);
            _filmGrain.intensity.value = Mathf.Lerp(0, _grainIntensityValue, t);
            _whiteBalance.temperature.value = Mathf.Lerp(0, _whiteBalanceTempValue, t);

            yield return null;
        }

        _colorAdjustments.contrast.value = _contrastValue;
        _colorAdjustments.saturation.value = _saturationValue;
        _filmGrain.intensity.value = _grainIntensityValue;
        _whiteBalance.temperature.value = _whiteBalanceTempValue;
    }

    private IEnumerator FadeOutRewindEffect(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _colorAdjustments.contrast.value = Mathf.Lerp(_contrastValue, 0, t);
            _colorAdjustments.saturation.value = Mathf.Lerp(_saturationValue, 0, t);
            _filmGrain.intensity.value = Mathf.Lerp(_grainIntensityValue, 0, t);
            _whiteBalance.temperature.value = Mathf.Lerp(_whiteBalanceTempValue, 0, t);

            yield return null;
        }

        _colorAdjustments.contrast.value = 0;
        _colorAdjustments.saturation.value = 0;
        _filmGrain.intensity.value = 0;
        _whiteBalance.temperature.value = 0;
    }
}