using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_WhiteSpriteUpdate : MonoBehaviour
{
    private SpriteRenderer _parentSpriteRenderer;
    private SpriteRenderer _selfSpriteRenderer;

    [SerializeField] private float _blinkDuration = 1;
    [SerializeField] private float _blinkMaxAlpha = 1;
    [SerializeField] private AnimationCurve _blinkCurve;

    private bool _isBlinking = false;
    private float _startBlinkingTime;
    private Color _baseColor;

    private void Start()
    {
        SpriteRenderer[] sprites;
        sprites = GetComponentsInParent<SpriteRenderer>();
        _parentSpriteRenderer = sprites[1];
        _selfSpriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _selfSpriteRenderer.color;
        if (RB_Tools.TryGetComponentInParent<RB_Health>(gameObject, out RB_Health health))
        {
            health.EventTakeDamage.AddListener(Blink);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _selfSpriteRenderer.sprite = _parentSpriteRenderer.sprite;

        if (_isBlinking)
        {
            float progression = (Time.time - _startBlinkingTime) / _blinkDuration;
            if (progression >= 1)
            {
                progression = 1;
                _isBlinking = false;
            }
            float alpha = _blinkCurve.Evaluate(progression) * _blinkMaxAlpha;
            _selfSpriteRenderer.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);
        }
    }

    public void Blink()
    {
        _startBlinkingTime = Time.time;
        _isBlinking = true;
    }
}
