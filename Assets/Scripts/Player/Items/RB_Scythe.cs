using MANAGERS;
using System.Collections.Generic;
using UnityEngine;

public class RB_Scythe : RB_Items
{
    //Zone
    [SerializeField] private GameObject _zonePrefab;
    private GameObject _instantiatedZone;
    bool _shouldGrow = false;
    private float _stayTime = 0;
    float _timer = 0;

    private bool _stopSound = false;

    //Properties
    [SerializeField] private float _zoneGrowthSpeed;
    [SerializeField] private float _maxZoneSize;

    public override void Attack() {
        base.Attack();
        RB_AudioManager.Instance.PlaySFX("Scythe_basic_attack", RB_PlayerController.Instance.transform.position, false, 0, 1);
    }

    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 5);
        _colliderAnimator.SetFloat("WeaponID", 5);
    }
    
    public override void StartChargingAttack()
    {
        base.StartChargingAttack();
        _stopSound = true;
        if(_instantiatedZone == null)
        {
            _instantiatedZone = Instantiate(_zonePrefab, _playerTransform.position, Quaternion.identity);
            RB_AudioManager.Instance.PlaySFX("darkMagic", RB_PlayerController.Instance.transform.position, true, 0, 1f);
            StartChargeZone();
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
        if(_shouldGrow && _instantiatedZone.transform.localScale.magnitude < _maxZoneSize)
        {
            _instantiatedZone.transform.localScale += (Vector3.one * _zoneGrowthSpeed) * Time.deltaTime;
            _stayTime += (Time.deltaTime/(_maxZoneSize/_zoneGrowthSpeed))*3;
        }
    }

    public override void StopChargingAttack()
    {
        base.StopChargingAttack();
        StopChargeZone();
        Invoke(nameof(StopSound), _stayTime);
        Destroy(_instantiatedZone, _stayTime);
    }

    private void StopSound() {
        _stopSound = false;
        RB_AudioManager.Instance.StopSFX();
    }

    private void Update()
    {
        ChargeZone();
    }

    public override void SpecialAttack() {
        base.SpecialAttack();
        int? currentPlayerRoom = RB_RoomManager.Instance.GetPlayerCurrentRoom();
        print(currentPlayerRoom);
        if (currentPlayerRoom != null)
        {
            List<RB_Health> detectedEnemies = RB_RoomManager.Instance.GetAllRooms()[currentPlayerRoom.Value].DetectedEnemies;
            foreach (RB_Health enemy in detectedEnemies)
            {
                if (enemy.Dead)
                {
                    enemy.Team = TEAMS.Player;
                    enemy.Heal();
                }
            }
        }
        

        RB_AudioManager.Instance.PlaySFX("summon-dark", RB_PlayerController.Instance.transform.position, false, 0, 1f);
    }
    
    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("sheating_Scythe", RB_PlayerController.Instance.transform.position, false, 0,1f);
    }
}
