using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RB_VibrationManager : MonoBehaviour {

    public static RB_VibrationManager Instance;

    Gamepad _pad;
    float _shakeDuration;
    float _elapsedTime;
    bool _isShaking;

    float _lowFrequency, _highFrequency;

    [SerializeField] private List<Vibration> _vibrations = new List<Vibration>();

    public string ActualName;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        SetVibration((int)PlayerPrefs.GetFloat("Vibration"));
    }

    public void GamepadShake(float lowFrequency, float highFrequency, float duration) {
        //Get reference of player's gamepad
        _pad = Gamepad.current;

        //If player have a current Gamepad
        if (_pad != null) {
            //Start vibration
            _pad.SetMotorSpeeds(lowFrequency, highFrequency);

            // Set shake duration and reset elapsed time
            _shakeDuration = duration;
            _elapsedTime = 0f;
            _isShaking = true;
        }
    }

    public void GamepadShake(float multiplier, float timeMultiplier) {
        
        //Get reference of player's gamepad
        _pad = Gamepad.current;

        //If player have a current Gamepad
        if (_pad != null) {
            //Start vibration
            _pad.SetMotorSpeeds(_lowFrequency*multiplier, _highFrequency * multiplier);

            // Set shake duration and reset elapsed time
            _shakeDuration *= timeMultiplier;
            _elapsedTime = 0f;
            _isShaking = true;
            Debug.Log($"{ActualName} : {_lowFrequency * multiplier} + {_highFrequency * multiplier} + {_shakeDuration} ");
        }
    }
    public void GamepadShake() {
        
        //Get reference of player's gamepad
        _pad = Gamepad.current;

        //If player have a current Gamepad
        if (_pad != null) {
            //Start vibration
            _pad.SetMotorSpeeds(_lowFrequency, _highFrequency);

            // Set shake duration and reset elapsed time
            _elapsedTime = 0f;
            _isShaking = true;
            Debug.Log($"{ActualName} : {_lowFrequency} + {_highFrequency} + {_shakeDuration} ");
        }
    }

    public void SetVibration(int state) {
        ActualName = _vibrations[state].Name;
        _lowFrequency = _vibrations[state].Low;
        _highFrequency = _vibrations[state].High;
        _shakeDuration = _vibrations[state].Duration;
        PlayerPrefs.SetFloat("Vibration", state);
        PlayerPrefs.Save();
        GamepadShake();
    }

    private void Update() {
        if (_isShaking) {
            // Update elapsed time
            _elapsedTime += Time.unscaledDeltaTime;

            // Check if the duration is over
            if (_elapsedTime >= _shakeDuration) {
                // Stop vibration
                _pad.SetMotorSpeeds(0f, 0f);
                _isShaking = false;
            }
        }
    }

    [System.Serializable]
    public struct Vibration {
        public string Name;
        public float Low;
        public float High;
        public float Duration;
    }
}
