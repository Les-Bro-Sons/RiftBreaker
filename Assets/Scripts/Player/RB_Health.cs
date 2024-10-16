using MANAGERS;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
[CustomEditor(typeof(RB_Health))]
public class RB_HealthCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RB_Health health = (RB_Health)target;

        if (GUILayout.Button("Die"))
        {
            health.EventDeath.Invoke();
        }
    }
}
#endif


public class RB_Health : MonoBehaviour {
    [SerializeField] float _hp; public float Hp { get { return _hp; } set { _hp = value; } }
    public float HpMax = 150;    
    public float Armor;
    public TEAMS Team = TEAMS.Ai;

    public bool Dead = false;

    //Nom de l'entité qui possède le script
    string _name; public string Name { get { return _name; } }
    
    [HideInInspector] public UnityEvent EventDeath;
    [HideInInspector] public UnityEvent EventTakeDamage;
    [HideInInspector] public UnityEvent EventHeal;
    [HideInInspector] public UnityEvent EventResurect;

    
    // ~~~~~~~~~~ UX ~~~~~~~~~~
    public float LerpTimer; //A METTRE DANS LE HUD

    [SerializeField] private GameObject _particleDamage;
    [SerializeField] private GameObject _particleDeath;
    [SerializeField] private GameObject _particleHeal;
    [SerializeField] Animator _animPlayer;
    [SerializeField] Animator _animUX;
    [SerializeField] Animator _animUIPlayer;

    //Components
    Rigidbody _rb;

    void Awake() {
        _name = gameObject.name;
        _hp = HpMax;
        _rb = GetComponent<Rigidbody>();
    }

    //Fonction de prise de dégâts
    public void TakeDamage(float amount, bool ignoreParticle = false) {
        if (amount == 0 && Dead) return;

        _hp = Mathf.Clamp(_hp - amount, 0, HpMax);
        LerpTimer = 0.0f;
        EventTakeDamage.Invoke();
        if (_hp <= 0 && !Dead)
        {
            Dead = true;
            if (_particleDeath && !ignoreParticle)
                Instantiate(_particleDeath, transform.position, Quaternion.identity);
            EventDeath.Invoke();
        }
        if (_particleDamage)
            Instantiate(_particleDamage, transform.position, Quaternion.identity);
        //triggering the animation
        if (_animPlayer) _animPlayer.SetTrigger("isDamage");
        else Debug.LogWarning("No _animPlayer in RB_Health");

        if (_animUX) _animUX.SetTrigger("isDamage");
        else Debug.LogWarning("No _animUX in RB_Health");

        if (_animUIPlayer)
        {
            //Change the text of the UI Text with the amount of damage
            _animUIPlayer.gameObject.GetComponentInChildren<TMP_Text>().text = (-amount).ToString();
            //Trigger the last animation
            _animUIPlayer.SetTrigger("isDamage");
        }
        else Debug.LogWarning("No _animUIPlayer in RB_Health");
        RB_AudioManager.Instance.PlaySFX("DamageSound", transform.position, false, 0.25f, 0.5f);
        
    }

    public void TakeKnockback(Vector3 direction, float knockbackForce)
    {
        if (knockbackForce == 0) return;

        //push the enemy away when getting hit
        direction = RB_Tools.GetHorizontalDirection(direction);
        _rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
    }

    //Fonction de soin
    public void Heal(float amount, bool ignoreParticle = false) {
        _hp = Mathf.Clamp(_hp + amount, 0, HpMax);
        LerpTimer = 0.0f;
        EventHeal.Invoke();
        if (_particleHeal && !ignoreParticle)
            Instantiate(_particleHeal, transform.position, Quaternion.identity);
        if (Dead && _hp > 0)
        {
            Dead = false;
            if (TryGetComponent<RB_Enemy>(out RB_Enemy enemy))
            {
                enemy.UnTombstone();
                EventResurect?.Invoke();
            }
        }
    }

    //Fonction de soin Maximum
    public void Heal() {
        Heal(HpMax);
    }
}