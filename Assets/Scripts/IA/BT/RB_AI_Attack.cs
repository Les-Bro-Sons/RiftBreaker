using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_Attack : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private float _attackCounter = 0f;
    private float _waitBeforeAttackCounter = 0f;
    private bool _hasAlreadyInit = false;

    //private Animator _animator;

    public RB_AI_Attack(RB_AI_BTTree BtParent)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target == null)
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }

        if (!_hasAlreadyInit)
        {
            _hasAlreadyInit = true;
        }

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _btParent.AttackSpeed)
        {
            _btParent.IsAttacking = true;
            //LANCER ANIMATION D'ATTAQUE
            switch (_btParent.AiType)
            {
                case ENEMYCLASS.Light:
                    break;
                case ENEMYCLASS.Medium:
                    break;
                case ENEMYCLASS.Heavy:
                    break;
            }

            //Quand y'aura l'animation, a modifier a partir d'ici
            
            if (_waitBeforeAttackCounter >= _btParent.WaitBeforeAttack) 
            { 
                if (target != null)
                {
                    Vector3 direction = target.position - _transform.position;
                    float distance = direction.magnitude;

                    switch (_btParent.AiType)
                    {
                        case ENEMYCLASS.Light:
                            if (distance <= _btParent.AttackRange || true) // Vérifie si l'agent est suffisamment proche de la cible
                            {
                                RB_Tools.TryGetComponentInParent<RB_Health>(target.gameObject, out RB_Health _targetHealth); // A REMPLACER QUAND IL Y AURA UNE ANIMATION
                                _targetHealth.TakeDamage(_btParent.AttackDamage);
                            }
                            break;

                        case ENEMYCLASS.Medium:
                            break;

                        case ENEMYCLASS.Heavy:
                            break;

                        default:
                            //_state = BTNodeState.FAILURE;
                            break;
                    }
                    _attackCounter = 0f;
                    _waitBeforeAttackCounter = 0f;
                    _btParent.IsAttacking = false;
                }
                else
                {
                    ClearData("target");
                    Debug.LogWarning("ClearData : target");
                    _hasAlreadyInit = false;
                }
            }
            else
            {
                _waitBeforeAttackCounter += Time.deltaTime;
                _btParent.transform.forward = (target.transform.position - _btParent.transform.position).normalized;
            }
        }

        _state = BTNodeState.RUNNING;
        return _state;
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
