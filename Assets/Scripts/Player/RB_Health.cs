using UnityEngine;
using UnityEngine.Events;


public class RB_Health : MonoBehaviour {
    [SerializeField] float _hp; public float Hp { get { return _hp; } }
    [SerializeField] float _hpMax; public float HpMax { get { return _hpMax; } }        
    public float Armor;

    public bool Dead = false;

    //Nom de l'entité qui possède le script
    string _name; public string Name { get { return _name; } }
    
    [HideInInspector] public UnityEvent EventDeath;
    [HideInInspector] public UnityEvent EventTakeDamage;
    [HideInInspector] public UnityEvent EventHeal;

    
    // ~~~~~~~~~~ UX ~~~~~~~~~~
    public float LerpTimer; //A METTRE DANS LE HUD

    [SerializeField] private GameObject _particleDamage;
    [SerializeField] private GameObject _particleDeath;
    [SerializeField] private GameObject _particleHeal;



    void Awake() {
        _name = gameObject.name;
        _hp = _hpMax;
    }

    //Fonction de prise de dégâts
    public void TakeDamage(float amount) {
        _hp = Mathf.Clamp(_hp - amount, 0, _hpMax);
        LerpTimer = 0.0f;
        EventTakeDamage.Invoke();
        if (_hp <= 0 && !Dead)
        {
            Dead = true;
            if (_particleDeath)
                Instantiate(_particleDeath, transform.position, Quaternion.identity);
            EventDeath.Invoke();
        }
        if (_particleDamage)
            Instantiate(_particleDamage, transform.position, Quaternion.identity);
    }

    //Fonction de soin
    public void Heal(float amount) {
        _hp = Mathf.Clamp(_hp + amount, 0, _hpMax);
        LerpTimer = 0.0f;
        EventHeal.Invoke();
        Instantiate(_particleHeal, transform.position, Quaternion.identity);
    }

    //Fonction de soin Maximum
    public void Heal() {
        Heal(_hpMax);
    }
}