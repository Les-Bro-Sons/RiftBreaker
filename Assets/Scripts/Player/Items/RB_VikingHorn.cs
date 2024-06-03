using UnityEngine;

public class RB_VikingHorn : RB_Items
{
    //Jump attack
    [SerializeField] private AnimationCurve _jumpCurve;
    [SerializeField] private float _jumpDistance;
    [SerializeField] private float _jumpHeight;
    private float _heightIndex = 0;
    private bool _shouldJump = false;
    private bool _isJumping = false;

    //Timer
    [SerializeField] private float _comboDelay;
    private float _attackUseTime;

    //Components
    private Rigidbody _rb;

    //Particles
    [SerializeField] private string _landingOnDirt;
    [SerializeField] private string _specialAttackParticle;
    private string _currentParticle;



    public override void Bind()
    {
        base.Bind();
        _rb = GetComponentInParent<Rigidbody>();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 3);
        _colliderAnimator.SetFloat("WeaponID", 3);
    }

    public override void Attack()
    {
        base.Attack();
        //Start the timer
        _attackUseTime = Time.time;
        //Increase the combo
        CurrentAttackCombo += 1;
        if(CurrentAttackCombo == 4)
        {
            StartJumpAttack(_landingOnDirt);
        }
        
    }

    public override void SpecialAttack()
    {
        base.SpecialAttack();
        StartJumpAttack(_specialAttackParticle);
    }

    private void UpdateAnim()
    {
        //Constantly updating animation
        _playerAnimator.SetFloat("Combo", CurrentAttackCombo);
        _colliderAnimator.SetFloat("Combo", CurrentAttackCombo);
    }

    private void ComboTimer()
    {
        if(Time.time > _attackUseTime + _comboDelay && CurrentAttackCombo != 4)
        {
            //If the combo delay is timeout then reset it
            CurrentAttackCombo = 0;
        }
    }

    public void StartJumpAttack(string particle)
    {
        if (!_isJumping)
        {
            _currentParticle = particle;
            //Start the jump attack
            _shouldJump = true;
        }
    }

    public void JumpAttack()
    {
        if(_rb != null && _shouldJump)
        {
            _isJumping = true;
            //Get the horizontal and vertical next position
            Vector3 horizontalPosition = _rb.position + RB_PlayerAction.Instance.transform.forward * _jumpDistance * Time.fixedDeltaTime;
            horizontalPosition.y = 0;
            Vector3 verticalPosition = new Vector3(0, _jumpCurve.Evaluate(_heightIndex) * _jumpHeight);
            _heightIndex += Time.fixedDeltaTime;
            //Set the position to the player
            Vector3 position = new Vector3(_rb.position.x, verticalPosition.y, _rb.position.z);
            _rb.MovePosition(position);
            if (_heightIndex > 1)
            {
                GameObject newObject = Instantiate(Resources.Load("Prefabs/Particles/" + _currentParticle), _transform.position, _transform.rotation) as GameObject;
                if (newObject.TryGetComponent<RB_Projectile>(out RB_Projectile projectile))
                {
                    newObject.transform.position += _transform.forward * projectile.SpawnDistanceFromPlayer;
                    projectile.Team = TEAMS.Player;
                }
                //If the jump is finished, stop the jump
                _heightIndex = 0;
                _shouldJump = false;
                _isJumping = false;
                CurrentAttackCombo = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        JumpAttack();
    }

    private void Update()
    {
        ComboTimer();
        UpdateAnim();
    }
}
