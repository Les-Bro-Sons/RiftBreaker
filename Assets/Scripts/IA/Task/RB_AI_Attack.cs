using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using MANAGERS;
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

    private bool _playSoundDamaged;
    
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

        if (!_btParent.GetBool("IsWaitingForAttack"))
        {
            _btParent.AiRigidbody.MoveRotation(Quaternion.LookRotation(RB_Tools.GetHorizontalDirection(_target.transform.position - _btParent.transform.position).normalized));
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
                            if (WaitBeforeAttackCounter(_btParent.InfSlashDelay))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                Slash(_btParent.InfSlashDamage, _btParent.InfSlashRange, _btParent.InfSlashKnockback, _btParent.InfSlashCollisionSize, _btParent.InfSlashParticles);
                                if(!_playSoundDamaged)
                                    RB_AudioManager.Instance.PlaySFX("SwordSwing", RB_PlayerController.Instance.transform.position, false, 0.5f, 1f);
                                StopAttacking();
                            }
                            break;
                        case 0: //slash attack
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                Slash(_btParent.SlashDamage, _btParent.SlashRange, _btParent.SlashKnockback, _btParent.SlashCollisionSize, _btParent.SlashParticles);
                                if(!_playSoundDamaged)
                                    RB_AudioManager.Instance.PlaySFX("SwordSwing", RB_PlayerController.Instance.transform.position, false, 0.1f, 1f);
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
                            if (WaitBeforeAttackCounter(_btParent.InfSlashDelay))
                            {
                                Slash(_btParent.InfSlashDamage, _btParent.InfSlashRange, _btParent.InfSlashKnockback, _btParent.InfSlashCollisionSize, _btParent.InfSlashParticles);
                                if(!_playSoundDamaged)
                                    RB_AudioManager.Instance.PlaySFX("SwordSwing", RB_PlayerController.Instance.transform.position, false, 0.5f, 1f);
                                StopAttacking();
                            }
                            break;
                        case 0: //bow attack
                            if (WaitBeforeAttackCounter(_btParent.BowDelay))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                RB_AudioManager.Instance.PlaySFX("bowArrow", RB_PlayerController.Instance.transform.position, false, 0f, 1f);
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
                            if (WaitBeforeAttackCounter(_btParent.InfSlashDelay))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                Slash(_btParent.InfSlashDamage, _btParent.InfSlashRange, _btParent.InfSlashKnockback, _btParent.InfSlashCollisionSize, _btParent.InfSlashParticles);
                                if(!_playSoundDamaged)
                                    RB_AudioManager.Instance.PlaySFX("SwordSwing", RB_PlayerController.Instance.transform.position, false, 0.5f, 1f);
                                StopAttacking();
                            }
                            break;
                        case 0: //Heavy bow attack (3 projectiles)
                            if (WaitBeforeAttackCounter(_btParent.HeavyBowDelay, true, true)) 
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("DistanceAttack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                _btParent.StartCoroutine(HeavyBowShoot());
                            }
                            break;
                        case 1: //heavy slash attack
                            if (WaitBeforeAttackCounter((_btParent.MaxHeavySlashCombo != 0)? _btParent.HeavySlashFirstDelay : _btParent.HeavySlashComboDelay, true, false))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                RB_AudioManager.Instance.PlaySFX("BigSwooosh", RB_PlayerController.Instance.transform.position, false, 0f, 1f);
                                HeavySlash();
                            }
                            break;
                        default:
                            StopAttacking();
                            break;
                    }
                    break;
                case ENEMYCLASS.Pawn:
                    switch (_attackIndex)
                    {
                        case 0:
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                RB_AudioManager.Instance.PlaySFX("Basic_attack_Chess", _transform.position, false, 0, 1);
                                Slash(_btParent.SlashDamage, _btParent.SlashRange, _btParent.SlashKnockback, _btParent.SlashCollisionSize, _btParent.SlashParticles);
                                StopAttacking();
                            }
                            break;
                    }
                    break;
                case ENEMYCLASS.Tower:
                    switch (_attackIndex)
                    {
                        case 0:
                            if (WaitBeforeAttackCounter(_btParent.SlashDelay))
                            {
                                if (_btParent.AiAnimator) _btParent.AiAnimator.SetTrigger("Attack"); else Debug.LogWarning("No AiAnimator on " + _transform.name);
                                KamikazeExplosion();
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

    private void KamikazeExplosion()
    {
        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapSphere(_transform.position, _btParent.ExplosionRadius))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;
                RB_AudioManager.Instance.PlaySFX("Chess_explosion", _transform.position, false, 0, 1);
                alreadyDamaged.Add(enemyHealth);
                _btParent.ApplyDamage(enemyHealth, _btParent.ExplosionDamage);
                enemyHealth.TakeKnockback(RB_Tools.GetHorizontalDirection(enemyHealth.transform.position, _transform.position), _btParent.ExplosionKnockback);
            }
        }
        if (_btParent.ExplosionParticles)
            _btParent.SpawnPrefab(_btParent.ExplosionParticles, _transform.position, Quaternion.identity);
        _btParent.AiHealth.TakeDamage(9999);
    }

    private bool WaitBeforeAttackCounter(float wait, bool rotateTowardTarget = false, bool rotateWhenAttacking = false, bool applyBoost = true) //used for the waitbeforeattack
    {
        _waitBeforeAttackCounter += Time.deltaTime;

        if (_waitBeforeAttackCounter > wait / _btParent.BoostMultiplier && !_btParent.GetBool("AlreadyAttacked"))
        {
            _btParent.BoolDictionnary["AlreadyAttacked"] = true;

            return true;
        }
        else
        {
            _btParent.BoolDictionnary["IsWaitingForAttack"] = true;
            if (rotateTowardTarget && (!_btParent.GetBool("AlreadyAttacked") || rotateWhenAttacking))
            {
                _btParent.AiRigidbody.MoveRotation(Quaternion.LookRotation((RB_Tools.GetHorizontalDirection(_target.transform.position - _btParent.transform.position).normalized)));
            }
            return false;
        }
    }

    public void Slash(float damage, float range, float knockback, float collSize, GameObject particle) //ATTACK 0 LIGHT
    {
        _playSoundDamaged = false;
        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapBox(_transform.position + (_transform.forward * collSize / 2), Vector3.one * (collSize / 2f), _transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;
                RB_AudioManager.Instance.PlaySFX("BloodStab", RB_PlayerController.Instance.transform.position, false, 0f, 1f);
                _playSoundDamaged = true;
                
                alreadyDamaged.Add(enemyHealth);
                _btParent.ApplyDamage(enemyHealth, damage);
                enemyHealth.TakeKnockback(enemyHealth.transform.position - _transform.position, knockback);
            }
        }
        _btParent.SpawnPrefab(particle, _transform.position + (_transform.forward * collSize / 2), _transform.rotation);
    }

    private void HeavySlash() //ATTACK 1 HEAVY
    {
        _playSoundDamaged = true;
        
        _btParent.CurrentHeavySlashCombo += 1;

        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapBox(_transform.position + (_transform.forward * _btParent.HeavySlashCollisionSize / 2), Vector3.one * (_btParent.HeavySlashCollisionSize / 2f), _transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;
                if (_playSoundDamaged)
                {
                    RB_AudioManager.Instance.PlaySFX("BloodStab", RB_PlayerController.Instance.transform.position, false, 0f, 1f);
                    _playSoundDamaged = false;
                }
                alreadyDamaged.Add(enemyHealth);
                _btParent.ApplyDamage(enemyHealth, _btParent.HeavySlashDamage);
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
            projectile.Damage = _btParent.ApplyDamage(_btParent.HeavyBowDamage);
            projectile.KnocbackExplosionForce = _btParent.HeavyBowKnockback;
            projectile.TotalDistance = _btParent.HeavyArrowDistance;
            projectile.Speed = _btParent.HeavyArrowSpeed;
            RB_AudioManager.Instance.PlaySFX("bowArrow", RB_PlayerController.Instance.transform.position, false, 0f, 1f);
        }
        _btParent.BoolDictionnary["HeavyAttackSlash"] = true;
        StopAttacking();
    }

    private void StopAttacking() //reset variables
    {
        _attackCounter = 0f;
        _waitBeforeAttackCounter = 0f;
        _btParent.BoolDictionnary["IsAttacking"] = false;
        _btParent.BoolDictionnary["AlreadyAttacked"] = false;
        _btParent.BoolDictionnary["IsWaitingForAttack"] = false;
    }

    public void LaunchArrow(GameObject arrowPrefab, float damage, float knockback, float speed, float distance) //ATTACK 0 MEDIUM
    {
        RB_Projectile projectile = _btParent.SpawnPrefab(arrowPrefab, _transform.position + _transform.forward, _transform.rotation).GetComponent<RB_Projectile>();
        projectile.Team = _btParent.AiHealth.Team;
        projectile.Damage = damage * _btParent.BoostMultiplier;
        projectile.KnocbackExplosionForce = knockback;
        projectile.Speed = speed;
        projectile.TotalDistance = distance;
    }
}
