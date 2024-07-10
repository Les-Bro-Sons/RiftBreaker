using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<GameObject> _currentZones = new();
    private List<GameObject> _currentSpecialZones = new();

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
        ZoneProperties.Damages = ChargedAttackDamage;
    }
   
    public override void Bind() //Override the bind of the item
    {
        base.Bind();
        if (RobertShouldTalk && !RB_LevelManager.SavedData.HasReachedWeapon)
        {
            RobertPickupDialogue(2);
        }
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 2);
        _colliderAnimator.SetFloat("WeaponID", 2);
    }

    public override void Attack() //Attack
    {
        base.Attack();
        ShootProjectile("MusicNote").Damage = AttackDamage; //Instantiate the music note
        RB_AudioManager.Instance.PlaySFX("Test", RB_PlayerController.Instance.transform.position, false, 0.25f, 0.15f);
    }

    public override void ChargedAttack() //Charged attack
    {
        base.ChargedAttack();
        _charging = false;
    }

    public override void StartChargingAttack() //Start the charging attack animation
    {
        base.StartChargingAttack();
        
        if (_currentZones == null || _charging == false)
        {
            _charging = true;
            _currentZones.Add(Instantiate(_musicZonePrefab)); //Instantiate the music zone
            StartChargeZone(); //Start the charge zone
            RB_AudioManager.Instance.PlaySFX("Music_Box_Charged_Attack", RB_PlayerController.Instance.transform.position, true, 0, 0.5f);
        }
    }

    public override void FinishChargingAttack() //Stop the charging animation attack
    {
        base.FinishChargingAttack();
        foreach(var zone in _currentZones.ToList())
        {
            if(zone != null) 
                zone.GetComponent<RB_MusicZone>().StopTakeAway();
        }
        _shouldGrow = false;
    }

    public void StartChargeZone() //Start the animation of the zone
    {
        _stayTime = 0;
        _shouldGrow = true;
    }

    public void StopChargeZone() //Stop the animation of the zone
    {
        if( _currentZones != null )
        {
            _shouldGrow = false;
            foreach (var zone in _currentZones.ToList())
            {
                if(zone != null)
                    zone.GetComponent<RB_MusicZone>().StopTakeAway();
            }
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
        if(_currentZones.Count > 0)
            StartCoroutine(DestroyZone(_currentZones[_currentZones.Count - 1]));
        if (_charging)
        {
            _charging = false;
            _playerAnimator.SetBool("ChargingAttack", false);
            _playerAction.StopAttack();
            _playerAction.StopChargedAttack();
            _playerAction.StopSpecialAttack();
        }
    }

    private IEnumerator DestroyZone(GameObject zone) //Destroy the zone
    {
        yield return new WaitForSeconds(_stayTime);
        if (zone != null)
            zone.GetComponent<RB_MusicZone>().StartDisapear();
        
    }

    protected override void Update()
    {
        base.Update();
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
        var currentZone = Instantiate(_musicZonePrefab);
        _currentSpecialZones.Add(currentZone);
        StartCoroutine(DestroySpecialZone(currentZone));
        RB_AudioManager.Instance.PlaySFX("Music_Box_Special_Attack", RB_PlayerController.Instance.transform.position, false, 0, 1f);
    }

    private IEnumerator DestroySpecialZone(GameObject zone)
    {
        yield return new WaitForSeconds(8);
        if(zone != null)
            zone.GetComponent<RB_MusicZone>().StartDisapear();
    }
    

}
