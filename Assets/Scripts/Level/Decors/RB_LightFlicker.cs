using UnityEngine;

public class RB_LightFlicker : MonoBehaviour
{
    private new Transform transform;

    private Light _light;
    private float _baseIntensity;
    private Color _baseColor;

    [Header("Flicker Settings")]
    public float _minIntensityVariation = -0.5f;
    public float _maxIntensityVariation = 1.5f;
    public float _flickerSpeed = 0.1f;
    public Color _flickerColor = Color.yellow;

    private Vector2 _seed;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        _light = GetComponent<Light>();
        if (_light == null)
        {
            enabled = false;
            return;
        }

        _seed = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
        _baseIntensity = _light.intensity;
        _minIntensityVariation += _baseIntensity;
        _maxIntensityVariation += _baseIntensity;
        _baseColor = _light.color;
    }

    private void Update()
    {
        if (RB_Tools.GetPlayerDistance(transform.position) < 20)
        {
            // Simulate flickering by varying the light intensity and color over time
            _light.intensity = Mathf.Lerp(_minIntensityVariation, _maxIntensityVariation, Mathf.PerlinNoise(Time.time * _flickerSpeed + _seed.x, _seed.y));
            _light.color = Color.Lerp(_baseColor, _flickerColor, Mathf.PerlinNoise(Time.time * _flickerSpeed + (_seed.x / 2f), (_seed.y / 2f)));
        }
    }
}
