using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RB_MainMenuClock : MonoBehaviour
{
    [Header("Minute")]
    [SerializeField] Image _minuteDisplay; // Image component for displaying minute sprites
    [SerializeField] float _minuteWait; // Time interval between minute updates
    [SerializeField] List<Sprite> _minuteSprites = new List<Sprite>(); // List of sprites for minute display
    int _currentMinuteSpriteID = 0; // Index of the current minute sprite being displayed

    [Header("Hour")]
    [SerializeField] Image _hourDisplay; // Image component for displaying hour sprites
    [SerializeField] List<Sprite> _hourSprites = new List<Sprite>(); // List of sprites for hour display
    int _currentHourSpriteID = 0; // Index of the current hour sprite being displayed

    float _elapsedTime = 0f; // Time elapsed since last minute update

    private void FixedUpdate()
    {
        _elapsedTime += Time.fixedDeltaTime; // Accumulate fixed delta time
        if (_elapsedTime >= _minuteWait)
        { // Check if it's time to update the minute
            _elapsedTime = 0f; // Reset elapsed time
            UpdateMinute(); // Update the minute display
        }
    }

    private void UpdateMinute()
    {
        // Increment to the next minute sprite, or wrap around if at the end of the list
        if (_currentMinuteSpriteID < _minuteSprites.Count - 1)
        {
            _currentMinuteSpriteID++;
        }
        else
        {
            _currentMinuteSpriteID = 0;
            UpdateHour(); // Update the hour display when minutes wrap around
        }
        // Update the minute display image with the current sprite
        _minuteDisplay.sprite = _minuteSprites[_currentMinuteSpriteID];
    }

    private void UpdateHour()
    {
        // Increment to the next hour sprite, or wrap around if at the end of the list
        if (_currentHourSpriteID < _hourSprites.Count - 1)
        {
            _currentHourSpriteID++;
        }
        else
        {
            _currentHourSpriteID = 0; // Reset to the first hour sprite
        }
        // Update the hour display image with the current sprite
        _hourDisplay.sprite = _hourSprites[_currentHourSpriteID];
    }
}
