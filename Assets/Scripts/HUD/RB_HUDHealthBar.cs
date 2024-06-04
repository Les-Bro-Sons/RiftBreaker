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
    [SerializeField]  RB_Health _rb_Health;

    [SerializeField] TextMeshProUGUI _bossName;

    private void Start() {
        //Pour avoir le nom du boss au dessus de sa barre de vie
        if (_mode == MODE.Boss && _rb_Health.Name != null) { 
            _bossName.text = _rb_Health.Name;
        }
        //Le joueur ne possède pas de système d'affichage de son nom
        else if( _bossName != null) {
            _bossName.text = "";
        }

        /*      
        //Quand les Event sont Invoke on Mets à jour la barre de vie
        _rb_Health.EventTakeDamage.AddListener(RefreshHealth);
        _rb_Health.EventHeal.AddListener(RefreshHealth);
        */

        switch (_mode)
        {
            case MODE.Player:
                _rb_Health = RB_PlayerController.Instance.GetComponent<RB_Health>();
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
        _displayedHealth = _rb_Health.Hp;

        _frontHealthBar.fillAmount = _rb_Health.Hp / _rb_Health.HpMax;
        _backHealthBar.fillAmount = _rb_Health.Hp / _rb_Health.HpMax;
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
        float hFraction = _rb_Health.Hp / _rb_Health.HpMax; // decimal 0 to 1

        if (fillB > hFraction) // background
        {
            _frontHealthBar.fillAmount = hFraction;
            _backHealthBar.color = Color.red;
            _rb_Health.LerpTimer += Time.deltaTime;

            float percentComplete = _rb_Health.LerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;

            _backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction) // front
        {
            _backHealthBar.color = Color.green;
            _backHealthBar.fillAmount = hFraction;
            _rb_Health.LerpTimer += Time.deltaTime;

            float percentComplete = _rb_Health.LerpTimer / _chipSpeed;
            percentComplete = percentComplete * percentComplete;

            _frontHealthBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }

    private void UxRefreshText()
    {
        _rb_Health.LerpTimer += Time.deltaTime;

        float percentComplete = _rb_Health.LerpTimer / _chipSpeed;
        percentComplete = percentComplete * percentComplete;

        _displayedHealth = Mathf.Lerp(_displayedHealth, _rb_Health.Hp, percentComplete);
        _healthTextPlayer.text = $"{Mathf.RoundToInt(_displayedHealth)} / {_rb_Health.HpMax} ♥";
    }
}