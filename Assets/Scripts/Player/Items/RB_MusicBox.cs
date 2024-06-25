using MANAGERS;
using System.Collections.Generic;
using UnityEngine;

public class RB_MusicBox : RB_Items
{
    //Enums
    

    //Zone
    bool _shouldGrow = false;
    private float _stayTime = 0;
    private bool _charging = false;

    //Properties
    [SerializeField] private float _zoneGrowthSpeed;
    [SerializeField] private float _maxZoneSize;
    [SerializeField] private float _cooldownBeforeNextAttack = 1f;
    private int _nbOfWaves = 1;
    private float _currentCooldownBetweenSpecialAttack;

    //Music Notes
    public List<Sprite> NoteSprites = new();
    private GameObject _currentZone;

    //Prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject _musicZonePrefab;

    //Music zone
    [Header("Music Zone Properties")]
    public ZonePropertiesClass ZoneProperties = new();


    protected override void Start()
    {
        base.Start();
        EventOnEndOfAttack.AddListener(EndOfAttack);
        ZoneProperties.Damages = _chargedAttackDamage;
    }
   
    public override void Bind() //Override the bind of the item
    {
        base.Bind();
        if (RB_PlayerAction.Instance.PickupGathered != null && RobertShouldTalk)
        {
            RB_PlayerAction.Instance.PickupGathered.StartDialogue(2);
            RobertShouldTalk = false;
        }
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 2);
        _colliderAnimator.SetFloat("WeaponID", 2);
    }

    public override void Attack() //Attack
    {
        base.Attack();
        ShootProjectile("MusicNote"); //Instantiate the music note
        RB_AudioManager.Instance.PlaySFX("Test", RB_PlayerController.Instance.transform .position, false, 0.15f, 1);
    }

    public override void ChargedAttack() //Charged attack
    {
        base.ChargedAttack();
        RB_AudioManager.Instance.StopSFX();
        _charging = false;
    }

    public override void StartChargingAttack() //Start the charging attack animation
    {
        base.StartChargingAttack();
        
        if (_currentZone == null || _charging == false)
        {
            _charging = true;
            _currentZone = Instantiate(_musicZonePrefab); //Instantiate the music zone
            StartChargeZone(); //Start the charge zone
            RB_AudioManager.Instance.PlaySFX("Music_Box_Charged_Attack", RB_PlayerController.Instance.transform.position, true, 0, 1f);
        }
    }

    public override void FinishChargingAttack() //Stop the charging animation attack
    {
        base.FinishChargingAttack();
        _shouldGrow = false;
    }

    public void StartChargeZone() //Start the animation of the zone
    {
        _stayTime = 0;
        _shouldGrow = true;
    }

    public void StopChargeZone() //Stop the animation of the zone
    {
        if( _currentZone != null )
        {
            _shouldGrow = false;
            _currentZone.GetComponent<RB_MusicZone>().StopTakeAway();
        }
    }

    public void ChargeZone() //Get bigger charge zone
    {
        if (_shouldGrow)
        {
            _stayTime += (Time.deltaTime / (ZoneProperties.MaxMusicNoteDistance / ZoneProperties.TakeAwaySpeed)) * 3;
        }
    }

    public override void StopChargingAttack() //Stop the charging animations
    {
        base.StopChargingAttack();
        StopChargeZone();
        Invoke(nameof(DestroyZone), _stayTime);
        if (_charging)
        {
            _charging = false;
            _playerAnimator.SetBool("ChargingAttack", false);
            _playerAction.StopAttack();
            _playerAction.StopChargedAttack();
            _playerAction.StopSpecialAttack();
        }
    }

    private void DestroyZone() //Destroy the zone
    {
        if(_currentZone != null)
            _currentZone.GetComponent<RB_MusicZone>().StartDisapear();
    }

    private void Update()
    {
        ChargeZone();
        _currentCooldownBetweenSpecialAttack -= Time.deltaTime;
    }

    private void EndOfAttack() 
    {
        RB_AudioManager.Instance.StopSFXByClip("Music_Box_Charged_Attack");
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("sheating_music_box", RB_PlayerController.Instance.transform.position, false, 0,1f);
    }

    public override void SpecialAttack()
    {
        base.SpecialAttack();
        RB_AudioManager.Instance.PlaySFX("Music_Box_Special_Attack", RB_PlayerController.Instance.transform.position, false, 0, 1f);
    }
    

}
