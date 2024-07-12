using BehaviorTree;
using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RB_AI_Attack : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private Transform _target;

    private float _attackCounter = 0f;
    private float _waitBeforeAttackCounter = 0f;

    private int _attackIndex = 0;
    private string? _variableAttackIndex = null;

    private bool _playSoundDamaged;

    private bool _random = false;
    private Dictionary<int, float> _randomIndex = new();
    
    public RB_AI_Attack(RB_AI_BTTree BtParent, int attackIndex)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        _attackIndex = attackIndex;
    }

    public RB_AI_Attack(RB_AI_BTTree btParent, string attackIndex)
    {
        _btParent = btParent;
        _transform = btParent.transform;
        _variableAttackIndex = attackIndex;
    }

    /// <summary>
    /// Pick a random index each time it begins an attack
    /// </summary>
    /// <param name="BtParent">Behavior tree</param>
    /// <param name="randomIndex">Int: index of attack, float: percentage</param>
    public RB_AI_Attack(RB_AI_BTTree BtParent, Dictionary<int, float> randomIndex)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        _randomIndex = randomIndex;
        _random = true;

        float randomSum = 0;
        foreach(float randomValue in _randomIndex.Values.ToList())
        {
            randomSum += randomValue;
        }
        float currentSum = 0;
        foreach (int i in _randomIndex.Keys.ToList())
        {
            _randomIndex[i] /= randomSum;
            float oldRandom = _randomIndex[i];
            _randomIndex[i] += currentSum;
            currentSum += oldRandom;
        }
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;
        _target = (Transform)GetData("target");

        if (_random)
        {
            if(!_btParent.GetBool(BTBOOLVALUES.IsWaitingForAttack) && !_btParent.GetBool(BTBOOLVALUES.IsAttacking))
            {
                float randomValue = Random.value;
                foreach (int i in _randomIndex.Keys.ToList())
                {
                    if (randomValue < _randomIndex[i])
                    {
                        _attackIndex = i;
                        break;
                    }
                }
            }
        }
        else if (_variableAttackIndex != null) _attackIndex = (int)_btParent.GetType().GetField(_variableAttackIndex).GetValue(_btParent);

        if (_target == null)
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }

        if (!_btParent.GetBool(BTBOOLVALUES.IsWaitingForAttack))
        {
            _btParent.AiRigidbody.MoveRotation(Quaternion.LookRotation(RB_Tools.GetHorizontalDirection(_target.transform.position - _btParent.transform.position).normalized));
        }

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _btParent.AttackSpeed)
        {
            _btParent.BoolDictionnary[BTBOOLVALUES.IsAttacking] = true;
            _btParent.CurrentAttackIndex = _attackIndex;
            _state = BTNodeState.RUNNING;

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
                case ENEMYCLASS.Megaknight:
                    switch (_attackIndex)
                    {
                        case 0:
                            if (WaitBeforeAttackCounter(_btParent.MegaSlashDelay))
                            {
                                if (_btParent.AiEnemyAnimation) _btParent.AiEnemyAnimation.TriggerBasicAttack(); else Debug.LogWarning("No AiEnemyAnimation on " + _transform.name);
                                RB_AudioManager.Instance.PlaySFX("BigSwooosh", _transform.position, false, 1, 1f);
                                Slash(_btParent.MegaSlashDamage, _btParent.MegaSlashRange, _btParent.MegaSlashKnockback, _btParent.MegaSlashCollisionSize, _btParent.MegaSlashParticles);
                                _btParent.Attack1CurrentCooldown = _btParent.Attack1Cooldown;
                                _btParent.CurrentWaitInIdle = _btParent.WaitInIdleAfterAttack/2f;
                                StopAttacking();
                            }
                            break;
                        case 1:
                            if (_btParent.AiEnemyAnimation) _btParent.AiEnemyAnimation.TriggerSecondAttack(); else Debug.LogWarning("No AiEnemyAnimation on " + _transform.name);
                            MegaKickAttack();
                            _btParent.Attack2CurrentCooldown = _btParent.Attack2Cooldown;
                            _btParent.CurrentWaitInIdle = _btParent.WaitInIdleAfterAttack/2f;
                            StopAttacking();
                            break;
                        case 2:
                            if (_btParent.AiEnemyAnimation) _btParent.AiEnemyAnimation.TriggerThirdAttack(); else Debug.LogWarning("No AiEnemyAnimation on " + _transform.name);
                            if (_btParent.CurrentJumpDuration == 0) StartMegaJumpAttack();
                            MegaJumpAttack();
                            _btParent.Attack3CurrentCooldown = _btParent.Attack3Cooldown;
                            _btParent.CurrentWaitInIdle = _btParent.WaitInIdleAfterAttack;
                            break;
                    }
                    break;
                case ENEMYCLASS.RobertLeNec:
                    break;
                case ENEMYCLASS.Yog:
                    break;
            }
        }

        
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

        if (_waitBeforeAttackCounter > wait / _btParent.BoostMultiplier && !_btParent.GetBool(BTBOOLVALUES.AlreadyAttacked))
        {
            _btParent.BoolDictionnary[BTBOOLVALUES.AlreadyAttacked] = true;

            return true;
        }
        else
        {
            _btParent.BoolDictionnary[BTBOOLVALUES.IsWaitingForAttack] = true;
            if (rotateTowardTarget && (!_btParent.GetBool(BTBOOLVALUES.AlreadyAttacked) || rotateWhenAttacking))
            {
                _btParent.AiRigidbody.MoveRotation(Quaternion.LookRotation((RB_Tools.GetHorizontalDirection(_target.transform.position - _transform.position).normalized)));
            }
            return false;
        }
    }

    public void MegaKickAttack() //ATTACK 2 MegaKnight
    {
        _transform.GetComponent<RB_Mega_knight>().AlreadySpikeDamaged.Clear();

        float currentLength = 0;
        Vector3 placingdir = (_target.position - _transform.position);
        placingdir = RB_Tools.GetHorizontalDirection(placingdir);
        Vector3 placingPos = _transform.position + (placingdir * _btParent.SpikesSpaces);
        placingPos.y = _btParent.Spikes.transform.position.y;

        _btParent.AiRigidbody.MoveRotation(Quaternion.LookRotation(placingdir));
        if (_btParent.AiEnemyAnimation) _btParent.AiEnemyAnimation.TriggerSecondAttack();

        float delay = 0;

        while (currentLength < _btParent.SpikesLength)
        {
            placingPos.y = _btParent.Spikes.transform.position.y;
            RB_Spikes spike = _btParent.SpawnPrefab(_btParent.Spikes, placingPos, Quaternion.identity).GetComponent<RB_Spikes>();
            spike.MegaKnight = _transform.GetComponent<RB_Mega_knight>();
            spike.GoingUpDelay = delay;

            delay += _btParent.SpikeDelayIncrementation;
            placingPos += placingdir * _btParent.SpikesSpaces;
            currentLength += _btParent.SpikesSpaces;
        }
    }

    private void StartMegaJumpAttack() //START ATTACK 3 MegaKnight
    {
        _btParent.AiEnemyAnimation.TriggerThirdAttack();
        _btParent.CurrentJumpDuration = 0;
        _btParent.JumpStartPos = _transform.position;
        _btParent.JumpEndPos = _target.position;
        _btParent.AiRigidbody.velocity = Vector3.zero;
    }

    public void MegaJumpAttack() //ATTACK 3 MegaKnight
    {
        //jump calculation
        _btParent.CurrentJumpDuration += Time.fixedDeltaTime;
        float percentComplete = _btParent.CurrentJumpDuration / _btParent.JumpDuration;
        float yPos = _btParent.JumpAttackCurve.Evaluate(percentComplete) * _btParent.JumpHeight;
        Vector3 horizontalPos = Vector3.Lerp(_btParent.JumpStartPos, _btParent.JumpEndPos, percentComplete);
        _btParent.AiRigidbody.MovePosition(new Vector3(horizontalPos.x, yPos, horizontalPos.z));

        if (_btParent.CurrentJumpDuration >= _btParent.JumpDuration) //when landed
        {
            List<RB_Health> alreadyDamaged = new();
            foreach (Collider collider in Physics.OverlapSphere(_transform.position, _btParent.LandingRadius)) //landing collider check
            {
                if (RB_Tools.TryGetComponentInParent<RB_Health>(collider.gameObject, out RB_Health enemyHealth))
                {
                    if (enemyHealth.Team == _btParent.AiHealth.Team || alreadyDamaged.Contains(enemyHealth)) continue;
                    alreadyDamaged.Add(enemyHealth);
                    enemyHealth.TakeDamage(_btParent.LandingDamage);
                    enemyHealth.TakeKnockback(RB_Tools.GetHorizontalDirection(collider.transform.position, _transform.position), _btParent.LandingKnockback);
                }
            }

            if (_btParent.LandingParticles)
            {
                _btParent.SpawnPrefab(_btParent.LandingParticles, _transform.position, Quaternion.identity);
            }

            //ENDING STATE ATTACK 3
            RB_AudioManager.Instance.PlaySFX("Jump_Attack_Viking_Horn", _transform.position, false, 0, 1f);
            _btParent.CurrentJumpDuration = 0;
            StopAttacking();
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
                RB_AudioManager.Instance.PlaySFX("BloodStab", _transform.position, false, 0.1f, 1f);
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
                    RB_AudioManager.Instance.PlaySFX("BloodStab", _transform.position, false, 0.1f, 1f);
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
            _btParent.BoolDictionnary[BTBOOLVALUES.HeavyAttackSlash] = false;
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
        _btParent.BoolDictionnary[BTBOOLVALUES.HeavyAttackSlash] = true;
        StopAttacking();
    }

    private void StopAttacking(bool stateToSuccess = true) //reset variables
    {
        _attackCounter = 0f;
        _waitBeforeAttackCounter = 0f;
        _btParent.BoolDictionnary[BTBOOLVALUES.IsAttacking] = false;
        _btParent.BoolDictionnary[BTBOOLVALUES.AlreadyAttacked] = false;
        _btParent.BoolDictionnary[BTBOOLVALUES.IsWaitingForAttack] = false;
        if (stateToSuccess) _state = BTNodeState.SUCCESS;
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
