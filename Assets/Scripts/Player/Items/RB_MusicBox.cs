using MANAGERS;
using System.Collections.Generic;
using UnityEngine;

public class RB_MusicBox : RB_Items
{
    //Zone
    [SerializeField] private GameObject _zonePrefab;
    private GameObject _instantiatedZone;
    bool _shouldGrow = false;
    private float _stayTime = 0;
    private bool _charging = false;

    //Properties
    [SerializeField] private float _zoneGrowthSpeed;
    [SerializeField] private float _maxZoneSize;

    //Music Notes
    public List<Sprite> NoteSprites = new();


    protected override void Start()
    {
        base.Start();
        EventOnEndOfAttack.AddListener(EndOfAttack);
    }
   
    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 2);
        _colliderAnimator.SetFloat("WeaponID", 2);
    }

    public override void Attack()
    {
        base.Attack();
        ShootProjectile("MusicNote");
        RB_AudioManager.Instance.PlaySFX("musicbox", RB_PlayerController.Instance.transform.position, 0.15f, 1);
    }

    public override void ChargedAttack()
    {
        base.ChargedAttack();

        _charging = false;
        RB_AudioManager.Instance.PlaySFX("musicbox_Loop", RB_PlayerController.Instance.transform.position, 0, 1);
    }

    public override void StartChargingAttack()
    {
        base.StartChargingAttack();
        _charging = true;
        if (_instantiatedZone == null)
        {
            _instantiatedZone = Instantiate(_zonePrefab, _playerTransform.position, Quaternion.identity);
            StartChargeZone();
            RB_AudioManager.Instance.PlaySFX("musicBoxManivelle", RB_PlayerController.Instance.transform.position, .15f, .05f);
        }
        

    }

    public void StartChargeZone()
    {
        _instantiatedZone.transform.localScale = Vector3.zero;
        _stayTime = 0;
        _shouldGrow = true;
    }

    public void StopChargeZone()
    {
        _shouldGrow = false;
    }

    public void ChargeZone()
    {
        if (_shouldGrow && _instantiatedZone.transform.localScale.magnitude < _maxZoneSize)
        {
            _instantiatedZone.transform.localScale += (Vector3.one * _zoneGrowthSpeed) * Time.deltaTime;
            _stayTime += (Time.deltaTime / (_maxZoneSize / _zoneGrowthSpeed)) * 3;
        }
    }

    public override void StopChargingAttack()
    {
        base.StopChargingAttack();
        StopChargeZone();
        Destroy(_instantiatedZone, _stayTime);
        if (_charging)
        {
            _charging = false;
            _playerAnimator.SetBool("ChargingAttack", false);
            _playerAction.StopAttack();
            _playerAction.StopChargedAttack();
            _playerAction.StopSpecialAttack();
        }
    }

    private void Update()
    {
        ChargeZone();
    }

    private void EndOfAttack() 
    {
        RB_AudioManager.Instance.StopSFX();
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("sheating_music_box", RB_PlayerController.Instance.transform.position, 0,.5f);
    }
}
