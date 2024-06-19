using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RB_MainMenuClock : MonoBehaviour{
    [Header("Minute")]
    [SerializeField] Image _minuteDisplay;
    [SerializeField] float _minuteWait;
    [SerializeField] List<Sprite> _minuteSprites = new List<Sprite>();
    int _currentMinuteSpriteID = 0;

    [Header("Hour")]
    [SerializeField] Image _hourDisplay;
    [SerializeField] List<Sprite> _hourSprites = new List<Sprite>();
    int _currentHourSpriteID = 0;

    float _elapsedTime = 0f;

    private void FixedUpdate() {
        _elapsedTime += Time.fixedDeltaTime;
        if (_elapsedTime >= _minuteWait){
            if (_minuteSprites.Count - 1 >= _currentMinuteSpriteID && _currentMinuteSpriteID >= 0) {
                _currentMinuteSpriteID++;
                _minuteDisplay.sprite = _minuteSprites[_currentMinuteSpriteID];
            }
            else {
                _currentMinuteSpriteID = 0;
                _minuteDisplay.sprite = _minuteSprites[_currentMinuteSpriteID];
                if (_hourSprites.Count - 1 >= _currentHourSpriteID && _currentHourSpriteID >= 0) {
                    _hourDisplay.sprite = _hourSprites[_currentHourSpriteID];
                    _currentHourSpriteID++;
                }
                else{
                    _currentHourSpriteID = 0;
                }
            }
            _elapsedTime = 0f;
        }
    }
}
