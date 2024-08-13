using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_ShurikenPatrick : RB_Items
{
    [SerializeField] private float _patrickRange; public float PatrickRange { get { return _patrickRange; } set { _patrickRange = value; } }
    [SerializeField] private float _patrickSpeed; public float PatrickSpeed{ get { return _patrickSpeed; } set { _patrickSpeed = value; } }
    [SerializeField] private int _patrickAttackMaxBounce; public int PatrickAttackMaxBounce { get { return _patrickAttackMaxBounce; } set { _patrickAttackMaxBounce = value; } }

    private bool _patrickShouldMove = false;
    private Transform _currentTarget = null;
    private Vector3 _mouseDirection = new();
    private Vector3 _currentStartPos = new();
    private Rigidbody _rb;
    private List<RB_Health> _touchedEnemies = new();
    private bool _shouldCurrentlyBounceBackToPlayer = false;
    private int _currentMaxBounce = 0;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();
        MovePatrickToTarget();
    }
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("LittleSwoosh", RB_PlayerController.Instance.transform.position,false, 0, 1);
        _currentMaxBounce = _patrickAttackMaxBounce;
        _shouldCurrentlyBounceBackToPlayer = true;
        ThrowPatrick();
    }

    private void OnTriggerEnter(Collider other)
    {
        BounceOrStopPatrick(other.transform, _currentMaxBounce, _shouldCurrentlyBounceBackToPlayer);
    }
    /// <summary>
    /// This function is called when there's a collision detected with patrick, then it checks if it's an enemy or the player. If it's an enemy, it deals damage to it then bounce to the next 
    /// </summary>
    /// <param name="entityTouchedTransform"></param>
    public void BounceOrStopPatrick(Transform entityTouchedTransform, int maxBounce, bool shouldBounceBackToPlayer)
    {
        if (_patrickShouldMove && RB_Tools.TryGetComponentInParent<RB_Health>(entityTouchedTransform, out RB_Health entityTouchedHealth))
        {
            if (entityTouchedHealth == RB_PlayerController.Instance.PlayerHealth && _currentTarget != null) //If the entity touched is the player make him gather patrick
            {
                MakePlayerGatherPatrick();
            }
            else if (entityTouchedHealth.Team == TEAMS.Ai)  //If the entity touched is an enemy
            {
                _touchedEnemies.Add(entityTouchedHealth);
                entityTouchedHealth.TakeDamage(AttackDamage);
                if (_touchedEnemies.Count >= maxBounce)
                {
                    StopPatrick(shouldBounceBackToPlayer);
                    print(_touchedEnemies.Count);
                }
                else
                {
                    RB_Health nearestValidEnemy = GetNearestValidEnemy();
                    if (nearestValidEnemy != null)
                    {
                        GoTo(nearestValidEnemy);
                    }
                    else
                    {
                        print("no enemy nearby");
                        StopPatrick(shouldBounceBackToPlayer);
                    }
                }
            }
        }
    }

    public void StopPatrick(bool shouldBounceBackToPlayer)
    {
        if (shouldBounceBackToPlayer)
        {
            GoTo(RB_PlayerController.Instance.PlayerHealth);
        }
        else
        {
            _patrickShouldMove = false;
            _currentTarget = null;
        }
        
    }

    public void MakePlayerGatherPatrick()
    {
        _patrickShouldMove = false;
        _currentTarget = null;
        _touchedEnemies.Clear();
        _transform.parent = _transform;
        RB_PlayerAction.Instance.GetItem(this);
    }

    public void MovePatrickToTarget()
    {
        if (_patrickShouldMove)
        {
            Vector3 directionToGo = new();
            if (_currentTarget != null)
            {
                directionToGo = (_currentTarget.position - _rb.position).normalized;
                
            }
            else
            {
                directionToGo = _mouseDirection;
            }
            _rb.position += directionToGo * Time.deltaTime * _patrickSpeed;
            if (Vector3.Distance(_rb.position, _currentStartPos) > _patrickRange)
            {
                GoTo(RB_PlayerController.Instance.PlayerHealth);
            }

        }
    }

    public void GoTo(RB_Health target)
    {
        if (target != null)
        {
            _currentTarget = target.transform;
            _patrickShouldMove = true;
            _currentStartPos = _rb.position;
        }
    }

    public void GoBy()
    {
        _patrickShouldMove = true;
        _currentTarget = null;
        _mouseDirection = RB_InputManager.Instance.GetMouseDirection().normalized;
        _transform.position = RB_PlayerAction.Instance.transform.position;
    }

    public void ThrowPatrick()
    {
        Drop();
        GoBy();
    }

    /// <summary>
    /// This function detect all the enemies around the player and check if the enemy is valid. It means that the enemy is within the range and unsude the field of view of the player.
    /// </summary>
    /// <returns> The nerest enemy valid</returns>
    public RB_Health GetNearestValidEnemy()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(_transform.position, _patrickRange);

        foreach (var hitEnemy in hitEnemies)
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(hitEnemy.transform, out RB_Health entity) && entity.Team == TEAMS.Ai && !_touchedEnemies.Contains(entity))
            {
                return entity;
            }
        }
        print("no enemies found");
        return null;
    }
    
    public override void Bind()
    {
        base.Bind();
        if (RobertShouldTalk && !RB_LevelManager.SavedData.HasReachedWeapon)
        {
            RobertPickupDialogue(7);
        }
        //Set the current weapon on the animators
        _playerAnimator.SetFloat("WeaponID", 7);
        _colliderAnimator.SetFloat("WeaponID", 7);
    }

    public IEnumerator WaitForEndOfFrameToChargeAttack()
    {
        yield return new WaitForEndOfFrame();
        base.ChargedAttack();
    }

    public override void ChargedAttack()
    {
        //Reset directions
        RB_PlayerMovement.Instance.ResetDirection();
        StartCoroutine(WaitForEndOfFrameToChargeAttack());
        RB_AudioManager.Instance.PlaySFX("BigSwoosh", RB_PlayerController.Instance.transform.position,false, 0, 1);
    }

    public IEnumerator WaitForEndOfFrameToPlaySFX()
    {
        yield return new WaitForSeconds(0.6f);
        RB_AudioManager.Instance.PlaySFX("Jump_Attack_Viking_Horn", RB_PlayerController.Instance.transform.position, false, 0, 1);
    }
    public override void SpecialAttack() {
        base.SpecialAttack();
        
        RB_AudioManager.Instance.PlaySFX("SwordSwing", RB_PlayerController.Instance.transform.position,false, 0, 1);
        StartCoroutine(WaitForEndOfFrameToPlaySFX());
        
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("sheating_Katana", RB_PlayerController.Instance.transform.position, false, 0,1f);
    }
}
