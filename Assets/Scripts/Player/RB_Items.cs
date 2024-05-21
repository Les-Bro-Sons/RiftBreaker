using System.Collections;
using UnityEngine;

public class RB_Items : MonoBehaviour
{
    //Attack
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _chargedAttackDamage;
    [SerializeField] private float _specialAttackDamage;
    private float _lastUsedAttackTime;
    private float _currentDamage;
    public float CurrentAttackCombo;
    public float ChargeTime;

    //Components
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private RB_CollisionDetection _collisionDetection; 
    RB_PlayerAction _playerAction;
    public Sprite HudSprite;

    private void Awake()
    {
        _playerAction = GetComponentInParent<RB_PlayerAction>();
    }

    private void Start()
    {
        _collisionDetection.EventOnObjectEntered.AddListener(DealDamage);
    }

    public virtual void ResetAttack()
    {
        //Turning off all attack animations
        _playerAnimator.SetBool("Attacking", false);
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
        _lastUsedAttackTime = Time.time;
        //Starting and resetting the attack animation
        _playerAnimator.SetBool("Attacking", true);
        StartCoroutine(WaitToResetAttacks());

        //Degats
        //KBs
    }

    public virtual void DealDamage()
    {
        foreach (GameObject detectedObject in _collisionDetection.DetectedObjects)
        {
            if (detectedObject.transform.root.TryGetComponent<RB_Health>(out RB_Health _enemyHealth))
            {
                _enemyHealth.TakeDamage(_currentDamage);
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
        _playerAnimator.SetBool("ChargeAttack", true);
        StartCoroutine(WaitToResetAttacks());
        //A COMPLETER
    }

    public virtual void SpecialAttack()
    {
        //Starting special attack
        _currentDamage = _specialAttackDamage;
        _playerAnimator.SetBool("SpecialAttack", true);
        StartCoroutine(WaitToResetAttacks());
        //A COMPLETER
    }


}
