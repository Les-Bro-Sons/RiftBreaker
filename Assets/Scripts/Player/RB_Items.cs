using System.Collections;
using UnityEngine;

public class RB_Items : MonoBehaviour
{
    //Attack
    private float _lastUsedAttackTime;
    private float _currentDamage;
    private float _currentKnockbackForce;
    [HideInInspector] public float CurrentAttackCombo;
    public float ChargeTime;
    
    [SerializeField] private float _attackCooldown;

    [Header("Damages")]
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _chargedAttackDamage;
    [SerializeField] private float _specialAttackDamage;

    [Header("Knockback")]
    [SerializeField] private float _normalKnockbackForce;
    [SerializeField] private float _chargeAttackKnockbackForce;
    [SerializeField] private float _specialAttackKnockbackForce;

    //Components
    [Header("Components")]
    [SerializeField] protected Animator _playerAnimator;
    [SerializeField] protected Animator _colliderAnimator;
    [SerializeField] private RB_CollisionDetection _collisionDetection;
    private Transform _transform;
    RB_PlayerAction _playerAction;
    public Sprite HudSprite;

    private void Awake()
    {
        _playerAction = GetComponentInParent<RB_PlayerAction>();
        _transform = transform;
    }

    protected virtual void Start()
    {
        _collisionDetection.EventOnEnemyEntered.AddListener(DealDamage);
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
        //Cooldown for attack
        _currentDamage = _attackDamage;
        _currentKnockbackForce = _normalKnockbackForce;
        _lastUsedAttackTime = Time.time;
        //Starting and resetting the attack animation
        _playerAnimator.SetTrigger("Attack");
        _colliderAnimator.SetTrigger("Attack");
        StartCoroutine(WaitToResetAttacks());

        //Degats
        //KBs
    }

    public virtual void DealDamage()
    {
        foreach (GameObject detectedObject in _collisionDetection.GetDetectedObjects())
        {
            //If on the detected object, there's life script, it deals damage
            if(RB_Tools.TryGetComponentInParent<RB_Health>(detectedObject, out RB_Health _enemyHealth))
            {
                _enemyHealth.TakeDamage(_currentDamage);
                _enemyHealth.TakeKnockback((_enemyHealth.transform.position - _transform.position).normalized, _currentKnockbackForce);
                print(detectedObject.name + "took damage");
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
        //Starting charge attack animations
        _currentDamage = _chargedAttackDamage;
        _currentKnockbackForce = _chargeAttackKnockbackForce;
        _playerAnimator.SetTrigger("ChargeAttack");
        _colliderAnimator.SetTrigger("ChargeAttack");
        StartCoroutine(WaitToResetAttacks());
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
        //A COMPLETER
    }

    public virtual void StartChargingAttack()
    {
        //Start the charge animation
        _playerAnimator.SetBool("ChargingAttack", true);
    }

    public virtual void StopChargingAttack()
    {
        //Stop the charge animation
        _playerAnimator.SetBool("ChargingAttack", false);
    }

    public virtual void FinishChargingAttack()
    {
        //start the finish charge animation
        _playerAnimator.SetTrigger("FinishChargingAttack");
    }


}
