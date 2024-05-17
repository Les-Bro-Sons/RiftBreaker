using System.Collections;
using UnityEngine;

public class RB_Items : MonoBehaviour
{
    //Attack
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _attackDamage;
    private float _lastUsedAttackTime;
    public float CurrentAttackCombo;

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
        _playerAnimator.SetBool("Attacking", false);
        _playerAction.StopAttack();
    }

    public virtual void Attack()
    {
        //Cooldown for attack
        _lastUsedAttackTime = Time.time;

        //Starting and resetting the attack animation
        _playerAnimator.SetBool("Attacking", true);
        StartCoroutine("WaitForEndOfFrame");

        //Degats
        //KBs
    }

    private IEnumerator WaitForEndOfFrame()
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
        //A COMPLETER
    }

    public virtual void SpecialAttack()
    {
        //A COMPLETER
    }


}
