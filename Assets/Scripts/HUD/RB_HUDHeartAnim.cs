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

    float _elapsedTime = 0f;

    private void FixedUpdate() {
        _hpPercent = ((_rb_Health.Hp / _rb_Health.HpMax) * 100);
        _elapsedTime += Time.fixedDeltaTime;

        if (_elapsedTime >= _waitForNextSprite) {
            UpdateSprite();
            _elapsedTime = 0.0f;
        }
    }

    private void UpdateSprite(){
        if (!RB_TimeManager.Instance.IsRewinding) {
            UpdateSpriteForward();
        }
        else {
            UpdateSpriteBackward();
        }
    }

    private void UpdateSpriteForward() {
        if (_hpPercent > 66){
            UpdateSpriteList(_sprites1);
        }
        else if (_hpPercent > 33){
            UpdateSpriteList(_sprites2);
        }
        else if (_hpPercent > 0) {
            UpdateSpriteList(_sprites3);
        }
        else{
            UpdateSpriteList(_sprites4);
        }
    }

    private void UpdateSpriteBackward(){
        if (_hpPercent > 66){
            UpdateSpriteListBackward(_sprites1);
        }
        else if (_hpPercent > 33){
            UpdateSpriteListBackward(_sprites2);
        }
        else if (_hpPercent > 0) {
            UpdateSpriteListBackward(_sprites3);
        }
        else {
            UpdateSpriteListBackward(_sprites4);
        }
    }

    private void UpdateSpriteList(List<Sprite> sprites) {
        if (_currentSpriteID >= 0 && _currentSpriteID < sprites.Count) {
            _image.sprite = sprites[_currentSpriteID];
            _currentSpriteID++;
        }
        else {
            _currentSpriteID = 0;
        }
    }

    private void UpdateSpriteListBackward(List<Sprite> sprites){
        if (_currentSpriteID >= 0 && _currentSpriteID < sprites.Count) {
            _image.sprite = sprites[_currentSpriteID];
            _currentSpriteID--;
        }
        else  {
            _currentSpriteID = sprites.Count - 1;
        }
    }


}
