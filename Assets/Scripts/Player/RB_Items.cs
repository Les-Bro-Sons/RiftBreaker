using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RB_Items : MonoBehaviour
{
    //Attack
    private float _lastUsedAttackTime;
    private float _currentDamage;
    private float _currentKnockbackForce;
    protected float _currentHitScreenshakeForce;
    [HideInInspector] public float CurrentAttackCombo;
    public float ChargeTime;

    [SerializeField] private float _chargeZoom = 0.85f;

    [Header("Cooldowns")]
    [SerializeField] private float _attackCooldown; [HideInInspector] public float AttackCooldown {  get { return _attackCooldown; } }

    [Header("Damages")]
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _chargedAttackDamage;
    [SerializeField] private float _specialAttackDamage;

    [Header("Knockback")]
    [SerializeField] private float _normalKnockbackForce;
    [SerializeField] private float _chargeAttackKnockbackForce;
    [SerializeField] private float _specialAttackKnockbackForce;

    [Header("Screenshake")]
    [SerializeField] protected float _normalAttackScreenshakeForce;
    [SerializeField] protected float _normalHitScreenshakeForce;
    [SerializeField] protected float _chargedAttackScreenshakeForce;
    [SerializeField] protected float _chargedHitScreenshakeForce;
    [SerializeField] protected float _specialAttackScreenshakeForce;
    [SerializeField] protected float _specialHitScreenshakeForce;
    

    //Components
    [Header("Components")]
    protected Animator _playerAnimator;
    protected Animator _colliderAnimator;
    private RB_CollisionDetection _collisionDetection;
    [SerializeField] private GameObject _objectToRemove;
    protected Transform _transform;
    RB_PlayerAction _playerAction;
    public Sprite HudSprite;
    protected CinemachineImpulseSource _impulseSource;

    protected virtual void Awake()
    {
        _transform = transform;
        
        if(_playerAction != null )
        {
            Bind();
        }
    }

    protected virtual void Start()
    {
        _playerAction = RB_PlayerAction.Instance;
        _playerAnimator = _playerAction.PlayerAnimator;
        _colliderAnimator = _playerAction.ColliderAnimator;
        _collisionDetection = _playerAction.CollisionDetection;
        if (RB_Tools.TryGetComponentInParent<CinemachineImpulseSource>(gameObject, out CinemachineImpulseSource impulseSource))
            _impulseSource = impulseSource;
    }

    public virtual void Bind()
    {
        _collisionDetection.EventOnEnemyEntered.RemoveAllListeners();
        _collisionDetection.EventOnEnemyEntered.AddListener(DealDamage);
        //Reset the current transform
        _transform = transform;
        //When the item is gathered get the playerAction
        _playerAction = GetComponentInParent<RB_PlayerAction>();
        //Remove the colliders and visuals of the weapon
        _objectToRemove.SetActive(false);
    }

    public virtual void Drop()
    {
        _objectToRemove.SetActive(true);
        _transform.parent = null;
    }

    public virtual void ResetAttack()
    {
        //Turning off all attack animations
        _playerAnimator.SetBool("ChargeAttack", false);
        _playerAnimator.SetBool("SpecialAttack", false);
        _playerAction.StopAttack();
        _playerAction.StopChargedAttack();
        _playerAction.StopSpecialAttack();
    }

    public virtual void Attack()
    {

        _currentDamage = _attackDamage;
        _currentKnockbackForce = _normalKnockbackForce;
        //Cooldown for attack
        _lastUsedAttackTime = Time.time;
        //Starting and resetting the attack animation
        _playerAnimator.SetTrigger("Attack");
        _colliderAnimator.SetTrigger("Attack");
        StartCoroutine(WaitToResetAttacks());

        /////UX/////
        if (_impulseSource)
            _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * Random.Range(_normalAttackScreenshakeForce * 0.9f, _normalAttackScreenshakeForce * 1.1f));
        _currentHitScreenshakeForce = _normalHitScreenshakeForce;
        //Degats
        //KBs
    }

    public virtual void DealDamage()
    {
        List<RB_Health> alreadyDamaged = new();
        foreach (GameObject detectedObject in _collisionDetection.GetDetectedObjects())
        {
            //If on the detected object, there's life script, it deals damage
            if(RB_Tools.TryGetComponentInParent<RB_Health>(detectedObject, out RB_Health _enemyHealth) && !alreadyDamaged.Contains(_enemyHealth))
            {
                alreadyDamaged.Add(_enemyHealth);
                _enemyHealth.TakeKnockback((_enemyHealth.transform.position - _playerAction.transform.position).normalized, _currentKnockbackForce);
                _enemyHealth.TakeDamage(_currentDamage);

                /////UX/////
                if (_impulseSource)
                    _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * Random.Range(_currentHitScreenshakeForce * 0.9f, _currentHitScreenshakeForce * 1.1f));
            }
        }
    }

    private IEnumerator WaitToResetAttacks()
    {
        //Wait for the end of the frame 
        yield return new WaitForEndOfFrame();
        //Reset attack
        Invoke("ResetAttack", _playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length );
    } 

    public virtual bool CanAttack()
    {
        //Cooldown dash
        return Time.time > (_lastUsedAttackTime + _attackCooldown);
    }

    public virtual void ChargedAttack()
    {
        if(_playerAnimator.GetFloat("WeaponID") <= .1f && _playerAnimator.GetFloat("WeaponID") >= 0f)
        {
            //Reset directions
            RB_PlayerMovement.Instance.ResetDirection();
        }

        //Starting charge attack animations
        _currentDamage = _chargedAttackDamage;
        _currentKnockbackForce = _chargeAttackKnockbackForce;
        _playerAnimator.SetTrigger("ChargeAttack");
        _colliderAnimator.SetTrigger("ChargeAttack");
        StartCoroutine(WaitToResetAttacks());

        /////UX/////
        if (_impulseSource)
            _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * Random.Range(_chargedAttackScreenshakeForce * 0.9f, _chargedAttackScreenshakeForce * 1.1f));
        RB_Camera.Instance.Zoom(1);
        _currentHitScreenshakeForce = _chargedHitScreenshakeForce;
        //A COMPLETER
    }

    public virtual void SpecialAttack()
    {
        //Starting special attack
        _currentDamage = _specialAttackDamage;
        _currentKnockbackForce = _specialAttackKnockbackForce;
        _playerAnimator.SetTrigger("SpecialAttack");
        _colliderAnimator.SetTrigger("SpecialAttack");
        StartCoroutine(WaitToResetAttacks());

        /////UX/////
        if (_impulseSource)
            _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * Random.Range(_specialAttackScreenshakeForce * 0.9f, _specialAttackScreenshakeForce * 1.1f));
        _currentHitScreenshakeForce = _specialHitScreenshakeForce;

        //A COMPLETER
    }

    public virtual void StartChargingAttack()
    {
        //Start the charge animation
        _playerAnimator.SetBool("ChargingAttack", true);

        /////UX/////
        RB_Camera.Instance.Zoom(_chargeZoom);
    }

    public virtual void StopChargingAttack()
    {
        //Stop the charge animation
        _playerAnimator.SetBool("ChargingAttack", false);

        //////UX/////
        RB_Camera.Instance.Zoom(1);
    }

    public virtual void FinishChargingAttack()
    {
        //start the finish charge animation
        _playerAnimator.SetTrigger("FinishChargingAttack");
    }


}
