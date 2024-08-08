using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RB_Items : MonoBehaviour
{
    public int PickupDialogueId = 0;

    //Attack
    protected float _lastUsedAttackTime;
    private float _currentDamage;
    private float _currentKnockbackForce;
    protected float _currentHitScreenshakeForce;
    [HideInInspector] public float CurrentAttackCombo;
    public float ChargeTime;
    public float SpecialAttackChargeTime;

    [SerializeField] protected float _chargeZoom = 0.85f; public float ChargeZoom { get { return _chargeZoom; } set { _chargeZoom = value; } }
    [Header("Cooldowns")]
    [SerializeField] protected float _attackCooldown; [HideInInspector] public float AttackCooldown(float? amount = null) { if (amount != null) { _attackCooldown = amount.Value; } return _attackCooldown;  }
    [SerializeField] protected float _chargeAttackCooldown; [HideInInspector] public float ChargeAttackCooldown(float? amount = null) { if (amount != null) { _chargeAttackCooldown = amount.Value; } return _chargeAttackCooldown; }
    [SerializeField] protected float _specialAttackCooldown; [HideInInspector] public float SpecialAttackCooldown(float? amount = null) { if (amount != null) { _specialAttackCooldown = amount.Value; } return _specialAttackCooldown; }

    [Header("Damages")]
    public float AttackDamage = 10;
    public float ChargedAttackDamage = 10;
    public float SpecialAttackDamage = 10;

    [Header("Knockback")]
    [SerializeField] protected float _normalKnockbackForce; public float NormalKnockbackForce { get { return _normalKnockbackForce; } set{ _normalKnockbackForce = value; } }
    [SerializeField] protected float _chargeAttackKnockbackForce = 8; public float ChargeAttackKnockbackForce { get { return _chargeAttackKnockbackForce; } set { _chargeAttackKnockbackForce = value; } }
    [SerializeField] protected float _specialAttackKnockbackForce = 10; public float SpecialAttackKnockbackForce { get { return _specialAttackKnockbackForce; } set { _specialAttackKnockbackForce = value; } }

    [Header("Screenshake")]
    [SerializeField] protected float _normalAttackScreenshakeForce = 0.025f; public float NormalAttackScreenshakeForce { get { return _normalAttackScreenshakeForce; } set { _normalAttackScreenshakeForce = value; } }
    [SerializeField] protected float _normalHitScreenshakeForce = .1f; public float NormalHitScreenshakeForce { get { return _normalHitScreenshakeForce; } set { _normalHitScreenshakeForce = value; } }
    [SerializeField] protected float _chargedAttackScreenshakeForce = .3f; public float ChargedAttackScreenshakeForce { get { return _chargedAttackScreenshakeForce; } set { _chargedAttackScreenshakeForce = value; } }
    [SerializeField] protected float _chargedHitScreenshakeForce = .75f; public float ChargedHitScreenshakeForce { get { return _chargedHitScreenshakeForce; } set { _chargedHitScreenshakeForce = value; } }
    [SerializeField] protected float _specialAttackScreenshakeForce = .5f; public float SpecialAttackScreenshakeForce { get { return _specialAttackScreenshakeForce; } set { _specialAttackScreenshakeForce = value; } }
    [SerializeField] protected float _specialHitScreenshakeForce = 1; public float SpecialHitScreenshakeForce { get { return _specialHitScreenshakeForce; } set { _specialHitScreenshakeForce = value; } }


    //Components
    [Header("Components")]
    protected Animator _playerAnimator;
    protected Animator _colliderAnimator;
    private RB_CollisionDetection _collisionDetection;
    [SerializeField] private GameObject _objectToRemove;
    protected Transform _transform;
    protected RB_PlayerAction _playerAction;
    public Sprite HudSprite;
    protected CinemachineImpulseSource _impulseSource;
    public Sprite CurrentSprite;
    public float ScaleSpriteHUD = 1;
    //Player
    protected Transform _playerTransform;

    //bool
    public bool FollowMouseOnChargeAttack;
    public bool CanMoveDuringSpecialAttack;
    public bool CanAttackDuringAttack;
    public bool RobertShouldTalk = true;

    //Events
    public UnityEvent EventOnEndOfAttack;
    public UnityEvent EventOnItemGathered;

    //Special attack
    [HideInInspector] public float SpecialAttackCharge = 100;

    protected virtual void Awake()
    {
        _transform = transform;
        
        if(_playerAction != null )
        {
            Bind();
        }
    }

    protected virtual void Update()
    {
        RechargeSpecialAttack();
    }

    public virtual void AddToSpecialChargeAttack(float amountToAdd)
    {
        //Add the specialAttackChargeAmount
        SpecialAttackCharge += amountToAdd;
    }

    public void RechargeSpecialAttack()
    {
        //Recharge over time the special attack
        if (SpecialAttackCharge <= 100)
        {
            SpecialAttackCharge += (Time.deltaTime / SpecialAttackChargeTime) * 100;
        }
    }


    public virtual RB_Projectile ShootProjectile(string projectileToShoot)
    {
        GameObject newObject = Instantiate(Resources.Load("Prefabs/Projectiles/" + projectileToShoot), _playerTransform.position, Quaternion.LookRotation(RB_PlayerMovement.Instance.DirectionToAttack)) as GameObject;
        if (newObject.TryGetComponent<RB_Projectile>(out RB_Projectile projectile))
        {
            newObject.transform.position += RB_PlayerMovement.Instance.DirectionToAttack * projectile.SpawnDistanceFromPlayer;
            projectile.Team = TEAMS.Player;
        }
        return projectile;
    }

    protected virtual void Start()
    {
        SpecialAttackCharge = 100;
        _playerTransform = RB_PlayerAction.Instance.transform;
        _playerAction = RB_PlayerAction.Instance;
        _playerAnimator = _playerAction.PlayerAnimator;
        _colliderAnimator = _playerAction.ColliderAnimator;
        _collisionDetection = _playerAction.CollisionDetection;
        if (RB_Tools.TryGetComponentInParent<CinemachineImpulseSource>(gameObject, out CinemachineImpulseSource impulseSource))
            _impulseSource = impulseSource;
        //Get the sprite of the item
        CurrentSprite = GetComponentInChildren<SpriteRenderer>().sprite;

        if(RB_Tools.TryGetComponentInParent<RB_PlayerAction>(gameObject, out _playerAction))
        {
            _playerAction.AddItemToList(this);
        }
        STATSREGION currentWeapon = (STATSREGION)Enum.Parse(typeof(STATSREGION), GetType().ToString().Substring(3));
        RB_StatsParser.Instance.SetStats(this, STATSCONTAINER._weaponsStats, currentWeapon, RB_DifficultyManager.Instance.GetCurrentDifficulty());
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
        EventOnItemGathered?.Invoke();
        if (RB_Tools.TryGetComponentInParent<CinemachineImpulseSource>(gameObject, out CinemachineImpulseSource impulseSource))
            _impulseSource = impulseSource;

    }

    public virtual void Drop()
    {
        _objectToRemove.SetActive(true);
        _transform.parent = null;
        _playerAction.Items.Remove(this);
        if (_playerAction.Item == this)
        {
            _playerAction.ItemId = Mathf.Clamp(_playerAction.ItemId - 1, 0, 5);
            _playerAction.Item = null;
            _playerAction.SetCurrentWeapon("");
        }
        _playerAction.EventItemDropped?.Invoke();
        //RobertShouldTalk = true;

    }

    public void RobertPickupDialogue(int? idOverride = null)
    {
        if (RB_PlayerAction.Instance.PickupGathered != null)
        {
            int dialogueID = (idOverride != null)? idOverride.Value : PickupDialogueId;
            RB_PlayerAction.Instance.PickupGathered.StartDialogue(dialogueID);
            RobertShouldTalk = false;
            RB_LevelManager.SavedData.HasReachedWeapon = true;

        }
    }

    public virtual void ResetAttack()
    {
        //Turning off all attack animations
        _playerAction.StopAttack();
    }

    public virtual void ResetChargeAttack()
    {
        _playerAnimator.SetBool("ChargeAttack", false);
        _playerAction.StopChargedAttack();
    }

    public virtual void ResetSpecialAttack()
    {

        _playerAnimator.SetBool("SpecialAttack", false);
        _playerAction.StopSpecialAttack();
    }

    public virtual void Attack()
    {
        _lastUsedAttackTime = Time.time;
        _currentDamage = AttackDamage;
        _currentKnockbackForce = _normalKnockbackForce;
        //Cooldown for attack
        //Starting and resetting the attack animation
        _playerAnimator.SetTrigger("Attack");
        _colliderAnimator.SetTrigger("Attack");
        //Reset attack
        Invoke(nameof(ResetAttack), _attackCooldown);

        /////UX/////
        if (_impulseSource)
            _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * UnityEngine.Random.Range(_normalAttackScreenshakeForce * 0.9f, _normalAttackScreenshakeForce * 1.1f));
        _currentHitScreenshakeForce = _normalHitScreenshakeForce;
        //Degats
        //KBs

    }

    public virtual void DealDamage()
    {
        List<RB_Health> alreadyDamaged = new();
        foreach (GameObject detectedObject in _collisionDetection.GetDetectedEnnemies())
        {
            //If on the detected object, there's life script, it deals damage
            if(RB_Tools.TryGetComponentInParent<RB_Health>(detectedObject, out RB_Health _enemyHealth) && _enemyHealth.Team != TEAMS.Player && !alreadyDamaged.Contains(_enemyHealth))
            {
                if(Physics.Raycast(_playerTransform.position, RB_PlayerMovement.Instance.DirectionToAttack, (_enemyHealth.transform.position - _playerTransform.position).magnitude, 1 << 3))
                {
                    return;
                }
                alreadyDamaged.Add(_enemyHealth);
                _enemyHealth.TakeKnockback((_enemyHealth.transform.position - _playerAction.transform.position).normalized, _currentKnockbackForce);
                _enemyHealth.TakeDamage(_currentDamage);

                /////UX/////
                if (_impulseSource)
                    _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * UnityEngine.Random.Range(_currentHitScreenshakeForce * 0.9f, _currentHitScreenshakeForce * 1.1f));
            }
        }
    }

    public virtual bool CanAttack()
    {
        //Cooldown dash
        return Time.time >= (_lastUsedAttackTime + _attackCooldown);
    }

    public virtual IEnumerator OnEndOfAttack()
    {
        yield return new WaitForSeconds(_playerAnimator.GetCurrentAnimatorClipInfo(0).Length);
        EventOnEndOfAttack?.Invoke();
    }

    public virtual void ChargedAttack()
    {
        print("charged attack");
        //Starting charge attack animations
        _currentDamage = ChargedAttackDamage;
        _currentKnockbackForce = _chargeAttackKnockbackForce;
        _playerAnimator.SetTrigger("ChargeAttack");
        _colliderAnimator.SetTrigger("ChargeAttack");
        //Reset attack
        Invoke(nameof(ResetChargeAttack), _chargeAttackCooldown);

        /////UX/////
        if (_impulseSource)
            _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * UnityEngine.Random.Range(_chargedAttackScreenshakeForce * 0.9f, _chargedAttackScreenshakeForce * 1.1f));
        RB_Camera.Instance.Zoom(1);
        _currentHitScreenshakeForce = _chargedHitScreenshakeForce;
        //A COMPLETER
        StartCoroutine(OnEndOfAttack());
    }

    public virtual void SpecialAttack()
    {
        //Starting special attack
        _currentDamage = SpecialAttackDamage;
        _currentKnockbackForce = _specialAttackKnockbackForce;
        _playerAnimator.SetTrigger("SpecialAttack");
        _colliderAnimator.SetTrigger("SpecialAttack");
        //Reset attack
        Invoke(nameof(ResetSpecialAttack), _specialAttackCooldown);

        /////UX/////
        if (_impulseSource)
            _impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * UnityEngine.Random.Range(_specialAttackScreenshakeForce * 0.9f, _specialAttackScreenshakeForce * 1.1f));
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

    public virtual void ChooseSfx() {
        
    }
}
