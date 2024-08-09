using MANAGERS;
using System.Collections;
using UnityEngine;

public class RB_ShurikenPatrick : RB_Items
{
    [SerializeField] private float _patrickRange; public float PatrickRange { get { return _patrickRange; } set { _patrickRange = value; } }
    [SerializeField] private float _patrickFOV; public float PatrickFOV { get { return _patrickFOV; } set { _patrickFOV = value; } }
    [SerializeField] private float _patrickSpeed; public float PatrickSpeed{ get { return _patrickSpeed; } set { _patrickSpeed = value; } }

    private bool _patrickShouldMove = false;
    private Transform _currentTarget = null;
    private Vector3 _currentStartPos = new();
    private Rigidbody _rb;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();
        MovePatrick();
        StayWithPlayer();
    }
    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("LittleSwoosh", RB_PlayerController.Instance.transform.position,false, 0, 1);
        ThrowPatrick();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(RB_Tools.TryGetComponentInParent<RB_Health>(other.transform, out RB_Health entity))
        {
            if (_patrickShouldMove && _currentTarget == entity.transform)
            {
                print("target reached");
                if(entity == RB_PlayerController.Instance.PlayerHealth)
                {
                    _patrickShouldMove = false;
                    RB_PlayerAction.Instance.Interact();
                }
                else
                {
                    GoTo(RB_PlayerController.Instance.PlayerHealth);
                }
            }
        }

        
    }

    public void StayWithPlayer()
    {
        if (!_patrickShouldMove && BindedOnPlayer)
        {
            _rb.position = _playerTransform.position;
        }
    }

    public void MovePatrick()
    {
        if (_patrickShouldMove)
        {
            _rb.position += (_currentTarget.position - _currentStartPos) * Time.deltaTime * _patrickSpeed;
            if (Vector3.Distance(_currentStartPos, _currentTarget.position) < Vector3.Distance(_rb.position, _currentStartPos))
            {
                print("target not reached");
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

    public void ThrowPatrick()
    {
        Drop();
        GoTo(GetNearestValidEnemy());
    }

    /// <summary>
    /// This function detect all the enemies around the player and check if the enemy is valid. It means that the enemy is within the range and unsude the field of view of the player.
    /// </summary>
    /// <returns> The nerest enemy valid</returns>
    public RB_Health GetNearestValidEnemy()
    {
        RB_Health nearestEnemy = null;
        Collider[] hitEnemies = Physics.OverlapSphere(_transform.position, _patrickRange);

        foreach (var hitEnemy in hitEnemies)
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(hitEnemy.transform, out RB_Health entity) && entity.Team == TEAMS.Ai && Mathf.Abs(Vector3.Angle(RB_InputManager.Instance.GetMouseDirection().normalized, (entity.transform.position - _playerAction.transform.position))) < _patrickFOV)
            {
                nearestEnemy = entity;
            }
        }

        return nearestEnemy;
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
