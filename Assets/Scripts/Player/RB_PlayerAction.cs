using Cinemachine;
using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_PlayerAction : MonoBehaviour
{
    public static RB_PlayerAction Instance;

    //Conditions
    [HideInInspector] public bool IsChargingAttack = false;
    [HideInInspector] public bool IsChargedAttacking;
    [HideInInspector] public bool IsSpecialAttacking;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsOnCooldown; //Cannot attack
    [HideInInspector] public bool IsRewinding;

    private float _currentDashCooldown;
    private float _chargeAttackPressTime;
    private bool _shouldStartCharging = false;

    //Components
    private RB_PlayerMovement _playerMovement;
    private RB_PlayerController _playerController;
    public Animator PlayerAnimator;
    public Animator ColliderAnimator;
    public RB_CollisionDetection CollisionDetection;
    [SerializeField] private GameObject _chargedAttackReadyMark;
    private Transform _transform;
    private CinemachineImpulseSource _impulseSource;
    private RB_TimeBodyRecorder _timeRecorder;
    public RB_Dialogue PickupGathered;

    //Charge attack
    private Coroutine _currentChargedAttack;
    [SerializeField] private float _startChargingDelay; public float StartChargingDelay { get { return _startChargingDelay; } }
    private bool _isChargingAnimation = false;

    //Events
    public UnityEvent EventBasicAttack;
    public UnityEvent EventChargedAttack;
    public UnityEvent EventSpecialAttack;
    public UnityEvent EventStartChargingAttack;
    public UnityEvent EventStopChargingAttack;
    public UnityEvent EventItemGathered;
    public UnityEvent EventItemDropped;
    public UnityEvent EventOnChargeSpecialAttackGathered;

    //Interacts
    [SerializeField] private float _interactRange;

    //item
    public List<RB_Items> Items = new();
    public bool IsItemNearby;
    public int ItemId = 0;
    public bool FirstItemGathered = false;
    public RB_Items Item; public RB_Items CurrentItem { get { return Item; } }

    [Header("Rewind")]
    public int RewindLeft = 3;
    public int MaxRewind = 3;

    //Debug
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI _debugCurrentWeaponFeedback;

    //Awake
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _playerMovement = GetComponent<RB_PlayerMovement>();
        _playerController = GetComponent<RB_PlayerController>();
        _transform = transform;
        Item = GetComponentInChildren<RB_Items>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _timeRecorder = GetComponent<RB_TimeBodyRecorder>();
    }

    private void Start()
    {
        RB_Health playerHealth = GetComponent<RB_Health>();
        playerHealth.EventDeath.AddListener(OnDeath);
        RB_StatsParser.Instance.SetStats(playerHealth, STATSCONTAINER._playerStats, STATSREGION.Health, RB_DifficultyManager.Instance.GetCurrentDifficulty());
    }
    //Update
    private void Update()
    {
        //count the time the player press the attack button
        TimerChargeAttack();
    }

    private void OnDeath()
    {
        StopChargeAttack();
        StopAttack();
        StopSpecialAttack();
    }



    

    

    public void SetCurrentWeapon(string currentWeapon)
    {
        if(_debugCurrentWeaponFeedback != null)
        {
            //set a debug feedback to know the current weapon
            _debugCurrentWeaponFeedback.text = currentWeapon;
        }
    }

    public void SetItem(int id)
    {
        //When the item is gathered, get it
        Item = Items[id];
        ItemId = id;
        Item.ChooseSfx();
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
        if (Item != null && ((CanAttack() && Item.CanAttack() && Item.CurrentAttackCombo < 4) || (Item.CanAttackDuringAttack && Item.CanAttack())))
        {
            //Attack
            IsAttacking = true;
            EventBasicAttack?.Invoke();
            Item.Attack();
            //_impulseSource.GenerateImpulse(RB_Tools.GetRandomVector(-1, 1, true, true, false) * Random.Range(0.1f, 0.2f));
        }
    }

    public void AddItemToList(RB_Items itemToAdd)
    {

        itemToAdd.transform.position = itemToAdd.transform.parent.position;
        int currentItemId = Items.IndexOf(Item);
        //Add the item gathered to the items
        if (Items.Count >= 3)
        {
            Item.Drop();
            Items.Insert(currentItemId, itemToAdd);
        }
        else
        {
            Items.Add(itemToAdd);
            currentItemId += 1;
        }
        _playerController.ChoseItem(currentItemId);
        EventItemGathered?.Invoke();
    }

    public void Interact()
    {
        IsItemNearby = false;
        foreach (Collider collider in Physics.OverlapSphere(_transform.position, _interactRange))
        {
            if(RB_Tools.TryGetComponentInParent<RB_Items>(collider.gameObject, out RB_Items itemGathered))
            {
                //For each object around the player, verify if it's an item
                //If it is then put it in the player child
                itemGathered.transform.parent = _transform;
                
                IsItemNearby = true;
                AddItemToList(itemGathered);
                

                RB_AudioManager.Instance.PlaySFX("Pick_Object", RB_PlayerController.Instance.transform.position, false, 0, 1);
                RB_AudioManager.Instance.PlaySFX("LightningStrike", false, false, 0, 0.75f);
                
                EventInTime timeEvent = new EventInTime(); //create a time event so the item will be dropped when rewinding
                timeEvent.TypeEvent = TYPETIMEEVENT.TookWeapon;
                timeEvent.ItemTook = itemGathered;
                _timeRecorder.RecordTimeEvent(timeEvent);
                OnPickUpWeaponUX(itemGathered);

                if (RB_LevelManager.Instance.CurrentPhase == PHASES.Infiltration)
                {
                    RB_LevelManager.Instance.SwitchPhase();
                }
            }
        }
        if (!IsItemNearby)
        {
            //if there's no item around then attack
            _playerController.OnChargeAttackStart();
        }
    }

    private void OnPickUpWeaponUX(RB_Items itemGathered)
    {
        PlayerAnimator.SetTrigger("ShowingWeapon");
        StartCoroutine(OnPickupWeaponAnimation(itemGathered));
    }

    private IEnumerator OnPickupWeaponAnimation(RB_Items itemGathered)
    {
        RB_InputManager.Instance.InputEnabled = false;
        float timer = 0;
        float duration = 1;
        GameObject showedWeapon = new GameObject();
        SpriteRenderer showedSpriteRenderer = showedWeapon.AddComponent<SpriteRenderer>();
        showedWeapon.transform.localScale = Vector3.one * 4;
        showedSpriteRenderer.sprite = itemGathered.CurrentSprite;
        GameObject weaponRay = Instantiate(Resources.Load<GameObject>("Prefabs/Player/ShowWeaponRay"), showedWeapon.transform.position, Quaternion.identity);
        _impulseSource.GenerateImpulseAtPositionWithVelocity(showedWeapon.transform.position, Vector3.up);
        StartCoroutine(RB_SunManager.Instance.OnLightningStrike(5, 0.5f));
        while (timer < duration)
        {
            showedWeapon.transform.position = _transform.position + Vector3.up * 1.5f;
            weaponRay.transform.position = showedWeapon.transform.position;
            timer += Time.deltaTime;
            yield return null;
        }
        RB_InputManager.Instance.InputEnabled = true;
        Destroy(showedWeapon);
        Destroy(weaponRay, 5);
        yield return null;
    }

    public void ChargedAttack()
    {
        if(Item != null)
        {
            //Charge attack
            IsChargedAttacking = true;
            Item.ChargedAttack();
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
        _shouldStartCharging = true;
        _isChargingAnimation = false;
    }

    public bool IsDoingAnyAttack()
    {
        //If the player is attacking in any way possible (normal attack, charging attack, charged attack or special attack)
        return IsChargedAttacking || IsSpecialAttacking || IsAttacking;
    }

    public bool IsDoingAnyNotNormalAttack()
    {
        return IsChargingAttack || IsChargedAttacking || IsSpecialAttacking;
    }

    public void StopChargeAttack()
    {
        if(Item != null && !IsItemNearby)
        {
            //Stop charging attack
            if(_currentChargedAttack != null)
                StopCoroutine(_currentChargedAttack);
            if(_chargeAttackPressTime < Item.ChargeTime)
            {
                //If the player didn't press long enough, normal attack
                Item.StopChargingAttack();
                IsChargingAttack = false;
                Attack();
            }
            else if(IsChargingAttack)
            {
                //Otherwise do the charged attack
                ChargedAttack();
            }
            Item.StopChargingAttack();
            IsChargingAttack = false;
            _shouldStartCharging = false;
            _chargeAttackPressTime = 0;
            EventStopChargingAttack?.Invoke();
        }
        
    }

    private IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(Item.ChargeTime);
        if (IsChargingAttack)
        {
            GameObject instantiatedChargedAttackReadyMark = Instantiate(_chargedAttackReadyMark, _transform);
            instantiatedChargedAttackReadyMark.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            Item.FinishChargingAttack();
            //When the charge of the attack is ready
            print("attaque chargée prête");
        }
    }

    public void SpecialAttack()
    {
        if(Item != null && CanAttack() && Item.SpecialAttackCharge >= 100)
        {
            //Special Attack
            IsSpecialAttacking = true;
            Item.SpecialAttackCharge = 0;
            Item.SpecialAttack();
            EventSpecialAttack?.Invoke();
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
        RB_TimeManager timeManager = RB_TimeManager.Instance;
        return (!timeManager.IsRewinding && RewindLeft > 0 && RB_LevelManager.Instance.CurrentPhase == PHASES.Infiltration);
    }

    public void Rewind()
    {
        if (CanRewind())
        {
            RewindLeft -= 1;
            RB_TimeManager.Instance.StartRewinding(REWINDENTITYTYPE.All, false, false);
            IsRewinding = true;
        }
    }

    public void StopRewind()
    {
        RB_TimeManager.Instance.StopRewinding(false);
        IsRewinding = false;
    }

    private void TimerChargeAttack()
    {
        if (Item != null && _shouldStartCharging && !IsItemNearby)
        {
            //count the time the player press the attack button
            _chargeAttackPressTime += Time.deltaTime;
            if (Item != null && _chargeAttackPressTime > _startChargingDelay && !_isChargingAnimation  && CanAttack())
            {
                //Start charging attack
                IsChargingAttack = true;
                if (_currentChargedAttack != null)
                    StopCoroutine(_currentChargedAttack);
                _currentChargedAttack = StartCoroutine(ChargeAttack());

                Item.StartChargingAttack();
                _isChargingAnimation = true;
                EventStartChargingAttack?.Invoke();
            } 
        }
    }
}
