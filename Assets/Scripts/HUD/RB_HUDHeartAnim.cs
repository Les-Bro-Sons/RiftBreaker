using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RB_HUDHeartAnim : MonoBehaviour {
    [SerializeField] List<Sprite> _sprites1;
    [SerializeField] List<Sprite> _sprites2;
    [SerializeField] List<Sprite> _sprites3;
    [SerializeField] List<Sprite> _sprites4;

    [SerializeField] float _waitForNextSprite = 0.5f;
    int _currentSpriteID = 0;

    Image _image;
    RB_Health _rb_Health;

    float _hpPercent;

    void Start() {
        _rb_Health = RB_PlayerController.Instance.GetComponent<RB_Health>();
        _image = GetComponent<Image>();
    }

    float elapsedTime = 0f;

    private void FixedUpdate() {
        _hpPercent = ((_rb_Health.Hp / _rb_Health.HpMax) * 100);
            elapsedTime += Time.fixedDeltaTime;
            if (elapsedTime >= _waitForNextSprite){
                if (!RB_TimeManager.Instance.IsRewinding) { 
                    if(_hpPercent > 66) {
                        if (_sprites1.Count-1 >= _currentSpriteID) {
                            _image.sprite = _sprites1[_currentSpriteID];
                            _currentSpriteID++;
                        }
                        else {
                            _currentSpriteID = 0;
                        }
                    }
                    else if(_hpPercent > 33) {
                        if (_sprites2.Count-1 >= _currentSpriteID) {
                            _image.sprite = _sprites2[_currentSpriteID];
                            _currentSpriteID++;
                        }
                        else {
                            _currentSpriteID = 0;
                        }
                    }
                    else if (_hpPercent > 0) {
                        if (_sprites3.Count - 1 >= _currentSpriteID) {
                            _image.sprite = _sprites3[_currentSpriteID];
                            _currentSpriteID++;
                        }
                        else {
                            _currentSpriteID = 0;
                        }
                    }
                    else {
                        if (_sprites4.Count - 1 >= _currentSpriteID) {
                            _image.sprite = _sprites4[_currentSpriteID];
                            _currentSpriteID++;
                        }
                        else {
                            _currentSpriteID = 0;
                        }
                    }
                }
                else {
                    if (_hpPercent > 66){
                        if (_currentSpriteID >= 0){
                            _image.sprite = _sprites1[_currentSpriteID];
                            _currentSpriteID--;
                        }
                        else{
                            _currentSpriteID = _sprites1.Count - 1;
                        }
                    }
                else if (_hpPercent > 33){
                    if (_currentSpriteID >= 0) {
                        _image.sprite = _sprites2[_currentSpriteID];
                        _currentSpriteID--;
                    }
                    else{
                        _currentSpriteID = _sprites2.Count - 1;
                    }
                }
                else if (_hpPercent > 0){
                    if (_currentSpriteID >= 0) {
                        _image.sprite = _sprites3[_currentSpriteID];
                        _currentSpriteID--;
                    }
                    else{
                        _currentSpriteID = _sprites3.Count - 1;
                    }
                }
                else {
                    if (_currentSpriteID >= 0) {
                        _image.sprite = _sprites4[_currentSpriteID];
                        _currentSpriteID--;
                    }
                    else {
                        _currentSpriteID = _sprites4.Count - 1;
                    }

                }
            }

            elapsedTime = 0.0f;
        }
    }

}
