using System.Collections.Generic;
using UnityEngine;

public class RB_FadeEffect
{
    private CanvasGroup _objectToFade;
    private float _changeSpeed;
    private FadeType _fadeType;
    public List<RB_FadeEffect> FadeEffects;
    public bool IsComplete { get; private set; }

    public RB_FadeEffect(CanvasGroup objectToFade, float changeSpeed, FadeType fadeType) //Create the fading effect for the object
    {
        this._objectToFade = objectToFade;
        this._changeSpeed = changeSpeed;
        this._fadeType = fadeType;
        IsComplete = false;

        if (fadeType == FadeType.Out)
        {
            this._objectToFade.alpha = 1;
        }
        else if (fadeType == FadeType.In)
        {
            this._objectToFade.alpha = 0;
        }
    }

    public void UpdateFade() //Updating constantly the fade effect
    {
        if (_fadeType == FadeType.Out)
        {
            _objectToFade.alpha -= Time.unscaledDeltaTime * _changeSpeed;
            if (_objectToFade.alpha <= 0)
            {
                _objectToFade.alpha = 0;
                IsComplete = true;
            }
        }
        else if (_fadeType == FadeType.In)
        {
            _objectToFade.alpha += Time.unscaledDeltaTime * _changeSpeed;
            if (_objectToFade.alpha >= 1)
            {
                _objectToFade.alpha = 1;
                IsComplete = true;
            }
        }
    }
}
