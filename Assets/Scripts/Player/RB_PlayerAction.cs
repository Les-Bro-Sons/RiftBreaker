using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_PlayerAction : MonoBehaviour
{
    public static RB_PlayerAction Instance;

    //Conditions
    [HideInInspector] public bool IsChargingAttack;
    [HideInInspector] public bool IsChargedAttacking;
    [HideInInspector] public bool IsSpecialAttacking;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsOnCooldown; //Cannot attack


    [Range(0, 100)] public float SpecialAttackCharge; //from 0 to 100
    private float _currentDashCooldown;
    private float _chargeAttackPressTime;

    //Components
    private RB_PlayerMovement _playerMovement;
    private RB_PlayerController _playerController;
    public Animator PlayerAnimator;
    public Animator ColliderAnimator;
    public RB_CollisionDetection CollisionDetection;
    [SerializeField] private GameObject _chargedAttackReadyMark;
    private Transform _transform;
    private CinemachineImpulseSource _impulseSource;

    //Charge attack
    private Coroutine _currentChargedAttack;
    [SerializeField] private float _startChargingDelay; public float StartChargingDelay { get { return _startChargingDelay; } }
    private bool _isChargingAnimation = false;

    //Events
    public UnityEvent EventBasicAttack;
    public UnityEvent EventChargedAttack;
    public UnityEvent EventStartChargingAttack;
    public UnityEvent EventStopChargingAttack;
    public UnityEvent EventItemGathered;

    //Interacts
    [SerializeField] private float _interactRange;

    //item
    public List<RB_Items> Items = new();
    public bool IsItemNearby;
    private int _itemId = 0;
    RB_Items _item; public RB_Items CurrentItem { get { return _item; } }

    //Debug
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _debugCurrentWeaponFeedback; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _playerMovement = GetComponent<RB_PlayerMovement>();
        _playerController = GetComponent<RB_PlayerController>();
        _transform = transform;
        _item = GetComponentInChildren<RB_Items>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void SetCurrentWeapon(string currentWeapon)
    {
        if(_debugCurrentWeaponFeedback != null)
        {
            //set a debug feedback to know the current weapon
            _debugCurrentWeaponFeedback.text = currentWeapon;
        }
    }

    public void Rebind()
    {
        //When the item is gathered, get it
        _item = GetComponentInChildren<RB_Items>();
    }

    public void StartDash()
    {
        if (_playerMovement.CanDash())
        {
            //Start Dash
            _playerMovement.StartDash();
        }
    }

    public void Attack()
    {
        if (_item != null && CanAttack() && _item.CanAttack())
        {
            //Attack
            IsAttacking = true;
            _item.Attack();
            EventBasicAttack?.Invoke();
            print("charge attack annulé et attaque commencé");
            //_impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * Random.Range(0.1f, 0.2f));
        }
    }

    public void Interact()
    {
        IsItemNearby = false;
        foreach (Collider collider in Physics.OverlapSphere(_transform.position, _interactRange))
        {
            print("trying to gather item");
            if(RB_Tools.TryGetComponentInParent<RB_Items>(collider.gameObject, out RB_Items itemGathered))
            {
                //For each object around the player, verify if it's an item
                print("item gathered");
                //If it is then put it in the player child
                itemGathered.transform.parent = _transform;
                EventItemGathered?.Invoke();
                itemGathered.Bind();
                IsItemNearby = true;
                //Add the item gathered to the items
                Items.Add(itemGathered);
                _playerController.ChoseItem(_itemId);
                _itemId++;
            }
        }
        if (!IsItemNearby)
        {
            //if there's no item around then attack
            _playerController.OnChargeAttackStart();
        }
    }

    public void ChargedAttack()
    {
        if(_item != null)
        {
            //Charge attack
            _item.ChargedAttack();
            IsChargedAttacking = true;
            EventChargedAttack?.Invoke();
        }
        
    }

    public void StopChargedAttack()
    {
        //Stop charged attack
        IsChargedAttacking = false;
    }

    public void StopAttack()
    {
        //Stop normal attack
        IsAttacking = false;
    }

    public void StartChargeAttack()
    {
        if (_item != null && CanAttack())
        {
            //Start charging attack
            print("IsChargedAttacking : " + IsChargedAttacking + ", Starting charge attack");
            IsChargingAttack = true;
            _isChargingAnimation = false;
            _chargeAttackPressTime = 0;
            if(_currentChargedAttack != null)
                StopCoroutine(_currentChargedAttack);
            _currentChargedAttack = StartCoroutine(ChargeAttack());
        }
    }

    public bool IsDoingAnyAttack()
    {
        //If the player is attacking in any way possible (normal attack, charging attack, charged attack or special attack)
        return IsChargingAttack  || IsChargedAttacking || IsSpecialAttacking || IsAttacking;
    }

    public bool IsDoingAnyNotNormalAttack()
    {
        return IsChargingAttack || IsChargedAttacking || IsSpecialAttacking;
    }

    public void StopChargeAttack()
    {
        if(_item != null && !IsItemNearby)
        {
            //Stop charging attack
            if(_currentChargedAttack != null)
                StopCoroutine(_currentChargedAttack);
            if(_chargeAttackPressTime < _item.ChargeTime)
            {
                //If the player didn't press long enough, normal attack
                _item.StopChargingAttack();
                IsChargingAttack = false;
                Attack();
            }
            else if(IsChargingAttack)
            {
                //Otherwise do the charged attack
                ChargedAttack();
            }
            _item.StopChargingAttack();
            IsChargingAttack = false;
            EventStopChargingAttack?.Invoke();
        }
        
    }

    private IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(_item.ChargeTime);
        if (IsChargingAttack)
        {
            GameObject instantiatedChargedAttackReadyMark = Instantiate(_chargedAttackReadyMark, _transform);
            instantiatedChargedAttackReadyMark.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            _item.FinishChargingAttack();
            //When the charge of the attack is ready
            print("attaque chargée prête");
        }
    }

    public void SpecialAttack()
    {
        if(_item != null && CanAttack() && SpecialAttackCharge >= 100)
        {
            //Special Attack
            IsSpecialAttacking = true;
            SpecialAttackCharge = 0;
            _item.SpecialAttack();
        }
    }

    public void StopSpecialAttack()
    {
        IsSpecialAttacking = false;
    }

    public bool CanAttack()
    {
        //If there's no cooldown left and is not attacking
        return !IsDoingAnyAttack();
    }

    public bool CanSpecialAttack()
    {
        //If there's no cooldown left and is not attacking
        return !(IsOnCooldown && (IsAttacking || IsChargingAttack));
    }

    public bool CanRewind()
    {
        //If can rewind
        return false;
    }

    private void Update()
    {
        //count the time the player press the attack button
        TimerChargeAttack();
    }

    private void TimerChargeAttack()
    {
        if (_item != null && IsChargingAttack)
        {
            //count the time the player press the attack button
            _chargeAttackPressTime += Time.deltaTime;
            if (_chargeAttackPressTime > _startChargingDelay && !_isChargingAnimation)
            {
                _item.StartChargingAttack();
                _isChargingAnimation = true;
                EventStartChargingAttack?.Invoke();
            } 
        }
    }
}
