using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class RB_SunManager : MonoBehaviour
{
    public static RB_SunManager Instance;

    [Header("Sun")]
    [SerializeField] private bool _destroyOtherDirectionnalLight = true;
    public AYellowpaper.SerializedCollections.SerializedDictionary<PHASES, float> SunIntensity;
    public AYellowpaper.SerializedCollections.SerializedDictionary<PHASES, Color> SunColor;

    private float _oldIntensity; //old values are used for interpolation
    private Color _oldColor;

    private Light _light;

    private float _timer = 0;
    [SerializeField] private float _timeToSwitch = 1;
    private bool _isSwitching = true;

    private PHASES _currentPhase;

    private void Awake()
    {
        Instance = this;
        _light = GetComponent<Light>();
        if (_destroyOtherDirectionnalLight)
        {
            List<Light> lights = GameObject.FindObjectsOfType<Light>().ToList();
            lights.RemoveAll(light => light.type != UnityEngine.LightType.Directional);
            foreach (Light light in lights)
            {
                if (light != _light)
                    Destroy(light);
            }
        }
    }

    private void Start()
    {
        RB_LevelManager.Instance.EventSwitchPhase.AddListener(OnSwitchPhase);
        OnSwitchPhase();
        _timer = 1;
    }

    private void Update()
    {
        if (_isSwitching)
        {
            _timer += Time.deltaTime;
            ApplyValues(_timer / _timeToSwitch);

            if (_timer >= _timeToSwitch)
            {
                _isSwitching = false;
                ApplyValues(1);
            }
        }
    }

    private void ApplyValues(float t)
    {
        _light.intensity = Mathf.Lerp(_oldIntensity, SunIntensity[_currentPhase], t);
        _light.color = Color.Lerp(_oldColor, SunColor[_currentPhase], t);
    }

    private void OnSwitchPhase()
    {
        _oldIntensity = _light.intensity;
        _oldColor = _light.color;

        _isSwitching = true;
        _timer = 0;
        _currentPhase = RB_LevelManager.Instance.CurrentPhase;
    }
}
