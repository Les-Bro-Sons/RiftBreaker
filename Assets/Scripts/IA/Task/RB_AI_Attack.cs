using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RB_AI_Attack : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private Transform _target;

    private float _attackCounter = 0f;
    private float _waitBeforeAttackCounter = 0f;

    private int _attackIndex = 0;

    public RB_AI_Attack(RB_AI_BTTree BtParent, int attackIndex)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        _attackIndex = attackIndex;
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
            _btParent.BoolDictionnary["IsAttacking"] = true;

            switch (_btParent.AiType)
            {
                case ENEMYCLASS.Light:
                    switch (_attackIndex)
                    {
                        case -1: //infiltration attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash(_btParent.InfSlashDamage, _btParent.InfSlashRange, _btParent.InfSlashKnockback, _btParent.InfSlashCollisionSize, _btParent.InfSlashParticles);
                                StopAttacking();
                            }
                            break;
                        case 0: //slash attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash(_btParent.SlashDamage, _btParent.SlashRange, _btParent.SlashKnockback, _btParent.SlashCollisionSize, _btParent.SlashParticles);
                                StopAttacking();
                            }
                            break;
                        default:
                            StopAttacking();
                            break;
                    }
                    break;
                case ENEMYCLASS.Medium:
                    switch (_attackIndex)
                    {
                        case -1: //infiltration attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash(_btParent.InfSlashDamage, _btParent.InfSlashRange, _btParent.InfSlashKnockback, _btParent.InfSlashCollisionSize, _btParent.InfSlashParticles);
                                StopAttacking();
                            }
                            break;
                        case 0: //bow attack
                            if (WaitBeforeAttackCounter(_btParent.BowDelay))
                            {
                                LaunchArrow(_btParent.ArrowPrefab, _btParent.BowDamage, _btParent.BowKnockback, _btParent.ArrowSpeed, _btParent.ArrowDistance);
                                StopAttacking();
                            }
                            break;
                        default:
                            StopAttacking();
                            break;
                    }
                    break;
                case ENEMYCLASS.Heavy:
                    switch (_attackIndex)
                    {
                        case -1: //infiltration attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                Slash(_btParent.InfSlashDamage, _btParent.InfSlashRange, _btParent.InfSlashKnockback, _btParent.InfSlashCollisionSize, _btParent.InfSlashParticles);
                                StopAttacking();
                            }
                            break;
                        case 0: //Heavy bow attack (3 projectiles)
                            if (WaitBeforeAttackCounter(_btParent.HeavyBowDelay, true, true)) 
                            {
                                _btParent.StartCoroutine(HeavyBowShoot());
                            }
                            break;
                        case 1: //heavy slash attack
                            if (WaitBeforeAttackCounter((_btParent.MaxHeavySlashCombo != 0)? _btParent.HeavySlashFirstDelay : _btParent.HeavySlashComboDelay, true, false))
                            {
                                HeavySlash();
                            }
                            break;
                        default:
                            StopAttacking();
                            break;
                    }
                    break;
            }
        }

        _state = BTNodeState.RUNNING;
        return _state;
    }

    public void Slash(float damage, float range, float knockback, float collSize, GameObject particle) //ATTACK 0 LIGHT
    {
        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapBox(_transform.position + (_transform.forward * collSize / 2), Vector3.one * (collSize / 2f), _transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;

                alreadyDamaged.Add(enemyHealth);
                enemyHealth.TakeDamage(damage);
                enemyHealth.TakeKnockback(enemyHealth.transform.position - _transform.position, knockback);
            }
        }
        _btParent.SpawnPrefab(particle, _transform.position + (_transform.forward * collSize / 2), _transform.rotation);
    }

    private void HeavySlash() //ATTACK 1 HEAVY
    {
        _btParent.CurrentHeavySlashCombo += 1;

        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapBox(_transform.position + (_transform.forward * _btParent.HeavySlashCollisionSize / 2), Vector3.one * (_btParent.HeavySlashCollisionSize / 2f), _transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;

                alreadyDamaged.Add(enemyHealth);
                enemyHealth.TakeDamage(_btParent.HeavySlashDamage);
                enemyHealth.TakeKnockback(enemyHealth.transform.position - _transform.position, _btParent.HeavySlashKnockback);
            }
        }
        _btParent.SpawnPrefab(_btParent.HeavySlashParticles, _transform.position + (_transform.forward * _btParent.HeavySlashCollisionSize / 2), _transform.rotation);

        if (_btParent.CurrentHeavySlashCombo >= _btParent.MaxHeavySlashCombo) //check if combo is fully complete
        {
            _btParent.BoolDictionnary["HeavyAttackSlash"] = false;
            _btParent.CurrentHeavySlashCombo = 0;
        }

        StopAttacking();
    }

    private IEnumerator HeavyBowShoot() //ATTACK 0 HEAVY
    {
        for (int i = 0; i < _btParent.HeavyBowProjectileNumber; i++)
        {
            yield return new WaitForSeconds(_btParent.HeavyBowDelayBetweenProjectile);
            RB_Projectile projectile = _btParent.SpawnPrefab(_btParent.HeavyArrowPrefab, _transform.position, _transform.rotation).GetComponent<RB_Projectile>();
            projectile.Team = _btParent.AiHealth.Team;
            projectile.Damage = _btParent.HeavyBowDamage;
            projectile.KnocbackExplosionForce = _btParent.HeavyBowKnockback;
            projectile.TotalDistance = _btParent.HeavyArrowDistance;
            projectile.Speed = _btParent.HeavyArrowSpeed;
        }
        _btParent.BoolDictionnary["HeavyAttackSlash"] = true;
        StopAttacking();
    }

    private bool WaitBeforeAttackCounter(float wait, bool rotateTowardTarget = false, bool rotateWhenAttacking = false) //used for the waitbeforeattack
    {
        _waitBeforeAttackCounter += Time.deltaTime;
        
        if (_waitBeforeAttackCounter > wait && !_btParent.GetBool("AlreadyAttacked"))
        {
            _btParent.BoolDictionnary["AlreadyAttacked"] = true;
            return true;
        }
        else
        {
            if (rotateTowardTarget && (!_btParent.GetBool("AlreadyAttacked") || rotateWhenAttacking))
            {
                _btParent.AiRigidbody.MoveRotation(Quaternion.LookRotation((_target.transform.position - _btParent.transform.position).normalized));
            }
            return false;
        }
    }

    private void StopAttacking() //reset variables
    {
        _attackCounter = 0f;
        _waitBeforeAttackCounter = 0f;
        _btParent.BoolDictionnary["IsAttacking"] = false;
        _btParent.BoolDictionnary["AlreadyAttacked"] = false;
    }

    public void LaunchArrow(GameObject arrowPrefab, float damage, float knockback, float speed, float distance) //ATTACK 0 MEDIUM
    {
        RB_Projectile projectile = _btParent.SpawnPrefab(arrowPrefab, _transform.position + _transform.forward, _transform.rotation).GetComponent<RB_Projectile>();
        projectile.Team = _btParent.AiHealth.Team;
        projectile.Damage = damage;
        projectile.KnocbackExplosionForce = knockback;
        projectile.Speed = speed;
        projectile.TotalDistance = distance;
    }
}
