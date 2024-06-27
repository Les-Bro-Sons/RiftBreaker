using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RB_HUDRecastTime : MonoBehaviour 
{
    public enum RECASTTYPE 
    {
        Dash,
        AttackBase,
        AttackCharged,
        AttackSpecial
    }

    public RECASTTYPE Type;

    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _displayCast;

    private float _remainTime;
    private bool _isTimerStarted;
    private float _multiplierFactor;

    public UnityEvent EventTimerEnd;

    /// <summary>
    /// Initializes the recast time system and sets up event listeners.
    /// </summary>
    private void Start() 
    {
        _multiplierFactor = 1f / _remainTime;
        _fillImage.fillAmount = 0;
        _timerText.text = "";

        switch (Type) 
        {
            case RECASTTYPE.Dash:
                RB_PlayerMovement.Instance.EventDash.AddListener(delegate { RecastTimerStart(RB_PlayerMovement.Instance.DashCooldown); });
                break;
            case RECASTTYPE.AttackBase:
                RB_PlayerAction.Instance.EventBasicAttack.AddListener(delegate { RecastTimerStart(RB_PlayerAction.Instance.CurrentItem.AttackCooldown()); });
                break;
            case RECASTTYPE.AttackCharged:
                RB_PlayerAction.Instance.EventStartChargingAttack.AddListener(delegate { RecastTimerStart(RB_PlayerAction.Instance.CurrentItem.ChargeTime - RB_PlayerAction.Instance.StartChargingDelay); });
                RB_PlayerAction.Instance.EventStopChargingAttack.AddListener(RecastTimerEnd);
                break;
            case RECASTTYPE.AttackSpecial:
                _multiplierFactor = 1f / 100f;
                break;
        }
    }

    /// <summary>
    /// Initializes the recast timer with a specified duration.
    /// </summary>
    /// <param name="timer">The duration of the recast timer in seconds.</param>
    public void RecastTimerStart(float timer) 
    {
        _multiplierFactor = 1f / timer;
        _remainTime = timer;
        _timerText.text = _remainTime.ToString();
        _isTimerStarted = true;

        switch (Type) 
        {
            default:
                _fillImage.fillAmount = _remainTime * _multiplierFactor;
                break;
            case RECASTTYPE.AttackCharged:
                _fillImage.fillAmount = 1;
                break;
        }
    }

    /// <summary>
    /// Updates the recast timer and handles special attack charge.
    /// </summary>
    private void Update() 
    {
        if (Type == RECASTTYPE.AttackSpecial) 
        {
            HandleSpecialAttackCharge();
        }

        if (!_isTimerStarted) return;

        if (_remainTime > 0f) 
        {
            UpdateRecastTimer();
        } 
        else 
        {
            RecastTimerEnd();
        }
    }

    /// <summary>
    /// Handles the special attack charge display.
    /// </summary>
    private void HandleSpecialAttackCharge() 
    {
        if (RB_PlayerAction.Instance.Item != null) 
        {
            float charge = RB_PlayerAction.Instance.Item.SpecialAttackCharge;

            if (charge < 100) 
            {
                _timerText.text = charge.ToString("0");
                _fillImage.fillAmount = Mathf.Abs((charge * _multiplierFactor) - 1);
            } 
            else 
            {
                _timerText.text = "";
                _fillImage.fillAmount = 0;
            }
        }
    }

    /// <summary>
    /// Updates the recast timer values and UI elements.
    /// </summary>
    private void UpdateRecastTimer() 
    {
        _remainTime -= Time.deltaTime;
        _timerText.text = _remainTime.ToString("0.0");

        switch (Type) 
        {
            case RECASTTYPE.AttackCharged:
                _fillImage.fillAmount = 1 - (_remainTime * _multiplierFactor);
                break;
            default:
                _fillImage.fillAmount = _remainTime * _multiplierFactor;
                break;
        }
    }

    /// <summary>
    /// Called when the recast timer ends.
    /// </summary>
    private void RecastTimerEnd() 
    {
        _timerText.text = "";
        _remainTime = 0;
        _fillImage.fillAmount = 0;
        EventTimerEnd?.Invoke();
    }
}
