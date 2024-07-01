using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RB_GamepadShakeManager : MonoBehaviour {

    public static RB_GamepadShakeManager Instance;

    Gamepad _pad;
    float _shakeDuration;
    float _elapsedTime;
    bool _isShaking;

    [SerializeField] Slider _shakeIntensity;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            DestroyImmediate(gameObject);
        }
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

    public void ChangeFrequency() { 
        
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
}
