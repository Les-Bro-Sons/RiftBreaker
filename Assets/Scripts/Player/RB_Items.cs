using System.Collections;
using UnityEngine;

public class RB_Items : MonoBehaviour
{
    //Attack
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackDamage;
    private float _lastUsedAttackTime;
    public float CurrentAttackCombo;
    public float ChargeTime;

    //Components
    [SerializeField] private Animator _playerAnimator;
    RB_PlayerAction _playerAction;
    public Sprite HudSprite;

    private void Awake()
    {
        _playerAction = GetComponentInParent<RB_PlayerAction>();
    }

    public virtual void ResetAttack()
    {
        //Turning off all attack animations
        _playerAnimator.SetBool("Attacking", false);
        _playerAnimator.SetBool("ChargeAttack", false);
        _playerAction.StopAttack();
        _playerAction.StopChargedAttack();
    }

    public virtual void Attack()
    {
        //Cooldown for attack
        _lastUsedAttackTime = Time.time;

        //Starting and resetting the attack animation
        _playerAnimator.SetBool("Attacking", true);
        StartCoroutine(WaitToResetAttacks());

        //Degats
        //KBs
    }

    private IEnumerator WaitToResetAttacks()
    {
        //Wait for the end of the frame 
        yield return new WaitForEndOfFrame();
        //Reset attack
        Invoke("ResetAttack", _playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    } 

    public virtual bool CanAttack()
    {
        //Cooldown dash
        return Time.time > (_lastUsedAttackTime + _attackCooldown);
    }

    public virtual void ChargedAttack()
    {
        //Starting charge attack animations
        _playerAnimator.SetBool("ChargeAttack", true);
        StartCoroutine(WaitToResetAttacks());
        //A COMPLETER
    }

    public virtual void SpecialAttack()
    {
        //A COMPLETER
    }


}
