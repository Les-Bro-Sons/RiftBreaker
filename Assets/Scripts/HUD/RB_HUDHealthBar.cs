using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_HUDHealthBar : MonoBehaviour {
    enum MODE
    {
        Player,
        Boss,
        Other
    }

    [SerializeField] private MODE _mode = MODE.Other;

    [SerializeField] TextMeshProUGUI _hpText;
    public RB_Health Rb_health;

    [SerializeField] TextMeshProUGUI _bossName;

    private void Start() {
        //Pour avoir le nom du boss au dessus de sa barre de vie
        if (_mode == MODE.Boss && Rb_health.Name != null) { 
            _bossName.text = Rb_health.Name;
        }
        //Le joueur ne possède pas de système d'affichage de son nom
        else if( _bossName != null) {
            _bossName.text = "";
        }

        switch (_mode)
        {
            case MODE.Player:
                Rb_health = RB_PlayerController.Instance.GetComponent<RB_Health>();
                break;
        }

        UxStart();
    }

    private void Update()
    {
        UxUpdateXHealthBar();
    }

    // ~~~~~~~~~~ UX ~~~~~~~~~~

    [Header("UX")]
    [SerializeField] private float _chipSpeed = 2.0f;
    

    [SerializeField] private Image _frontHealthBar;
    [SerializeField] private Image _backHealthBar;
    [SerializeField] private TMP_Text _healthTextPlayer;
    private float _displayedHealth;


    private void UxStart()
    {
        _displayedHealth = Rb_health.Hp;

        _frontHealthBar.fillAmount = Rb_health.Hp / Rb_health.HpMax;
        _backHealthBar.fillAmount = Rb_health.Hp / Rb_health.HpMax;
    }

    private void UxUpdateXHealthBar()
    {
        UxUpdateCheck();
        UxRefreshText();
    }

    private void UxUpdateCheck()
    {
        float fillF = _frontHealthBar.fillAmount;
        float fillB = _backHealthBar.fillAmount;
        float hFraction = Rb_health.Hp / Rb_health.HpMax; // decimal 0 to 1

        if (fillB > hFraction) // background
        {
            _frontHealthBar.fillAmount = hFraction;
            _backHealthBar.color = Color.red;
            Rb_health.LerpTimer += Time.deltaTime;

            float percentComplete = Rb_health.LerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;

            _backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction) // front
        {
            _backHealthBar.color = Color.green;
            _backHealthBar.fillAmount = hFraction;
            Rb_health.LerpTimer += Time.deltaTime;

            float percentComplete = Rb_health.LerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;

            _frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    private void UxRefreshText()
    {
        Rb_health.LerpTimer += Time.deltaTime;

        float percentComplete = Rb_health.LerpTimer / _chipSpeed;
        percentComplete = percentComplete * percentComplete;

        _displayedHealth = Mathf.Lerp(_displayedHealth, Rb_health.Hp, percentComplete);
        //_healthTextPlayer.text = $"{Mathf.RoundToInt(_displayedHealth)} / {Rb_health.HpMax} ♥";
    }
}