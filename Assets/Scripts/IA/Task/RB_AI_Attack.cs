using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_Attack : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private Transform _target;

    private float _attackCounter = 0f;
    private float _waitBeforeAttackCounter = 0f;

    private int _attackIndex = 0;

    //private Animator _animator;

    public RB_AI_Attack(RB_AI_BTTree BtParent, int attackIndex)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        _attackIndex = attackIndex;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        _target = (Transform)GetData("target");

        if (_target == null)
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _btParent.AttackSpeed)
        {
            _btParent.IsAttacking = true;

            switch (_btParent.AiType)
            {
                case ENEMYCLASS.Light:
                    switch (_attackIndex)
                    {
                        case -1: //infiltration attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash();
                                StopAttacking();
                            }
                            break;
                        case 0: //slash attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash();
                                StopAttacking();
                            }
                            break;
                        default:

                            break;
                    }
                    break;
                case ENEMYCLASS.Medium:
                    switch (_attackIndex)
                    {
                        case -1: //infiltration attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash();
                                StopAttacking();
                            }
                            break;
                    }
                    break;
                case ENEMYCLASS.Heavy:
                    switch (_attackIndex)
                    {
                        case -1: //infiltration attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash();
                                StopAttacking();
                            }
                            break;
                    }
                    break;
            }
        }

        _state = BTNodeState.RUNNING;
        return _state;
    }

    private bool WaitBeforeAttackCounter(float wait)
    {
        _waitBeforeAttackCounter += Time.deltaTime;
        _btParent.transform.forward = (_target.transform.position - _btParent.transform.position).normalized;
        return (_waitBeforeAttackCounter > wait);
    }

    private void StopAttacking()
    {
        _attackCounter = 0f;
        _waitBeforeAttackCounter = 0f;
        _btParent.IsAttacking = false;
    }

    public void Slash() //ATTACK 1
    {
        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapBox(_transform.position + (_transform.forward * _btParent.SlashRange / 2), Vector3.one * (_btParent.SlashRange / 2f), _transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;

                alreadyDamaged.Add(enemyHealth);
                enemyHealth.TakeDamage(_btParent.SlashDamage);
                enemyHealth.TakeKnockback(enemyHealth.transform.position - _transform.position, _btParent.SlashKnockback);
            }
        }
        _btParent.SpawnPrefab(_btParent.SlashParticles, _transform.position + (_transform.forward * _btParent.SlashRange / 2), _transform.rotation);
    }

    public void FinishedAttack()
    {

    }
}
