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


    protected override void Start()
    {
        base.Start();

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
        if(_charging)
        {
            StartCoroutine(WaitToResetAttacks());
            StartCoroutine(WaitToResetAttacks());
            _charging = false;
            return;
        }
        base.Attack();
    }

    public override void ChargedAttack()
    {
        base.ChargedAttack();

        _charging = false;
    }

    public override void StartChargingAttack()
    {
        base.StartChargingAttack();
        _charging = true;
        if (_instantiatedZone == null)
        {
            _instantiatedZone = Instantiate(_zonePrefab, _playerTransform.position, Quaternion.identity);
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
    }

    private void Update()
    {
        ChargeZone();
    }


}
