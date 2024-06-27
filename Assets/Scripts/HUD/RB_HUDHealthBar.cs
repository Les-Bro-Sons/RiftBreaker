using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_HUDHealthBar : MonoBehaviour
{
    // Enumeration for the health bar modes
    enum MODE
    {
        Player,
        Boss,
        Other
    }

    [SerializeField] private MODE _mode = MODE.Other; // Mode of the health bar (Player, Boss, Other)

    [SerializeField] private TextMeshProUGUI _hpText; // Reference to the health text display
    public RB_Health Rb_health; // Reference to the health component

    [SerializeField] private TextMeshProUGUI _bossName; // Reference to the boss name display

    [Header("UX")]
    [SerializeField] private float _chipSpeed = 2.0f; // Speed at which the health bar updates
    [SerializeField] private Image _frontHealthBar; // Front health bar image
    [SerializeField] private Image _backHealthBar; // Back health bar image
    [SerializeField] private TMP_Text _healthTextPlayer; // Player health text display

    private float _displayedHealth; // Displayed health value for smooth transitions

    /// <summary>
    /// Initializes the health bar and sets the boss name if applicable.
    /// </summary>
    private void Start()
    {
        // Set the boss name above the health bar if in Boss mode
        if (_mode == MODE.Boss && Rb_health.Name != null)
        {
            _bossName.text = Rb_health.Name;
        }
        else if (_bossName != null)
        {
            _bossName.text = "";
        }

        // Set the health component to the player's health if in Player mode
        if (_mode == MODE.Player)
        {
            Rb_health = RB_PlayerController.Instance.GetComponent<RB_Health>();
        }

        UxStart();
    }

    /// <summary>
    /// Updates the health bar every frame.
    /// </summary>
    private void Update()
    {
        UxUpdateXHealthBar();
    }

    /// <summary>
    /// Initializes the UX elements of the health bar.
    /// </summary>
    private void UxStart()
    {
        _displayedHealth = Rb_health.Hp;

        _frontHealthBar.fillAmount = Rb_health.Hp / Rb_health.HpMax;
        _backHealthBar.fillAmount = Rb_health.Hp / Rb_health.HpMax;
    }

    /// <summary>
    /// Updates the UX elements of the health bar.
    /// </summary>
    private void UxUpdateXHealthBar()
    {
        UxUpdateCheck();
        UxRefreshText();
    }

    /// <summary>
    /// Updates the fill amounts of the front and back health bars based on the current health.
    /// </summary>
    private void UxUpdateCheck()
    {
        float fillF = _frontHealthBar.fillAmount;
        float fillB = _backHealthBar.fillAmount;
        float hFraction = Rb_health.Hp / Rb_health.HpMax; // Decimal representation of health (0 to 1)

        // Update back health bar (damage taken)
        if (fillB > hFraction)
        {
            _frontHealthBar.fillAmount = hFraction;
            _backHealthBar.color = Color.red;
            Rb_health.LerpTimer += Time.deltaTime;

            float percentComplete = Rb_health.LerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;

            _backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }

        // Update front health bar (healing)
        if (fillF < hFraction)
        {
            _backHealthBar.color = Color.green;
            _backHealthBar.fillAmount = hFraction;
            Rb_health.LerpTimer += Time.deltaTime;

            float percentComplete = Rb_health.LerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;

            _frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    /// <summary>
    /// Refreshes the displayed health text.
    /// </summary>
    private void UxRefreshText()
    {
        Rb_health.LerpTimer += Time.deltaTime;

        float percentComplete = Rb_health.LerpTimer / _chipSpeed;
        percentComplete = percentComplete * percentComplete;

        _displayedHealth = Mathf.Lerp(_displayedHealth, Rb_health.Hp, percentComplete);
        //_healthTextPlayer.text = $"{Mathf.RoundToInt(_displayedHealth)} / {Rb_health.HpMax} ♥";
    }
}
