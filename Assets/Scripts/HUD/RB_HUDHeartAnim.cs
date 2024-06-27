using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RB_HUDHeartAnim : MonoBehaviour
{
    [SerializeField] private List<Sprite> _sprites1; // List of sprites for health percentage > 66
    [SerializeField] private List<Sprite> _sprites2; // List of sprites for health percentage > 33
    [SerializeField] private List<Sprite> _sprites3; // List of sprites for health percentage > 0
    [SerializeField] private List<Sprite> _sprites4; // List of sprites for health percentage == 0

    [SerializeField] private float _waitForNextSprite = 0.5f; // Time to wait before switching to the next sprite
    private int _currentSpriteID = 0; // Current sprite index

    private Image _image; // Reference to the Image component
    private RB_Health _rb_Health; // Reference to the player's health component

    private float _hpPercent; // Player's health percentage
    private float _elapsedTime = 0f; // Elapsed time for sprite switching

    /// <summary>
    /// Initializes the health component and Image component references.
    /// </summary>
    private void Start()
    {
        _rb_Health = RB_PlayerController.Instance.GetComponent<RB_Health>();
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// Updates the health percentage and handles sprite animation based on time.
    /// </summary>
    private void FixedUpdate()
    {
        _hpPercent = (_rb_Health.Hp / _rb_Health.HpMax) * 100;
        _elapsedTime += Time.fixedDeltaTime;

        if (_elapsedTime >= _waitForNextSprite)
        {
            UpdateSprite();
            _elapsedTime = 0.0f;
        }
    }

    /// <summary>
    /// Updates the sprite based on whether the game is in rewinding state or not.
    /// </summary>
    private void UpdateSprite()
    {
        if (!RB_TimeManager.Instance.IsRewinding)
        {
            UpdateSpriteForward();
        }
        else
        {
            UpdateSpriteBackward();
        }
    }

    /// <summary>
    /// Updates the sprite forward based on the health percentage.
    /// </summary>
    private void UpdateSpriteForward()
    {
        if (_hpPercent > 66)
        {
            UpdateSpriteList(_sprites1);
        }
        else if (_hpPercent > 33)
        {
            UpdateSpriteList(_sprites2);
        }
        else if (_hpPercent > 0)
        {
            UpdateSpriteList(_sprites3);
        }
        else
        {
            UpdateSpriteList(_sprites4);
        }
    }

    /// <summary>
    /// Updates the sprite backward based on the health percentage.
    /// </summary>
    private void UpdateSpriteBackward()
    {
        if (_hpPercent > 66)
        {
            UpdateSpriteListBackward(_sprites1);
        }
        else if (_hpPercent > 33)
        {
            UpdateSpriteListBackward(_sprites2);
        }
        else if (_hpPercent > 0)
        {
            UpdateSpriteListBackward(_sprites3);
        }
        else
        {
            UpdateSpriteListBackward(_sprites4);
        }
    }

    /// <summary>
    /// Updates the sprite list forward by incrementing the sprite index.
    /// </summary>
    /// <param name="sprites">List of sprites to animate through.</param>
    private void UpdateSpriteList(List<Sprite> sprites)
    {
        if (_currentSpriteID >= 0 && _currentSpriteID < sprites.Count)
        {
            _image.sprite = sprites[_currentSpriteID];
            _currentSpriteID++;
        }
        else
        {
            _currentSpriteID = 0;
        }
    }

    /// <summary>
    /// Updates the sprite list backward by decrementing the sprite index.
    /// </summary>
    /// <param name="sprites">List of sprites to animate through.</param>
    private void UpdateSpriteListBackward(List<Sprite> sprites)
    {
        if (_currentSpriteID >= 0 && _currentSpriteID < sprites.Count)
        {
            _image.sprite = sprites[_currentSpriteID];
            _currentSpriteID--;
        }
        else
        {
            _currentSpriteID = sprites.Count - 1;
        }
    }
}
