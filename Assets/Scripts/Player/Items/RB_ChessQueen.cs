using System.Collections.Generic;
using UnityEngine;

public class RB_ChessQueen : RB_Items
{
    //Pawns
    [Header("Prefabs")]
    [SerializeField] private GameObject _pawnPrefab;
    [SerializeField] private GameObject _towerPrefab;
    [Header("Properties")]
    [SerializeField] private float _pawnSpawnDistance;
    [SerializeField] private float _towerSpawnDistance;
    public List<GameObject> SpawnedChessPawns = new();
    private Vector3 _spawnPos = new();

    //Special attack
    private bool _shouldBoost = false;

    //Player
    private Transform _playerTransform;

    protected override void Start()
    {
        base.Start();
        _playerTransform = RB_PlayerAction.Instance.transform;
    }

    public override void Bind()
    {
        base.Bind();
        //Set the current weapon to the animator
        _playerAnimator.SetFloat("WeaponID", 4);
        _colliderAnimator.SetFloat("WeaponID", 4);
    }

    public override void Attack()
    {
        base.Attack();
        _playerTransform.forward = RB_InputManager.Instance.GetMouseDirection(_playerTransform.position);
        _spawnPos = _playerTransform.position + _playerTransform.forward * _pawnSpawnDistance;
        GameObject spawnedChessPawn = Instantiate(_pawnPrefab, _spawnPos, Quaternion.identity);
        if (_shouldBoost)
        {
            //spawnedChessPawn.Boost();
        }
        SpawnedChessPawns.Add(spawnedChessPawn);
    }

    public override void ChargedAttack()
    {
        base.ChargedAttack();
        _playerTransform.forward = RB_InputManager.Instance.GetMouseDirection(_playerTransform.position);
        _spawnPos = _playerTransform.position + _playerTransform.forward * _pawnSpawnDistance;
        GameObject spawnedChessPawn = Instantiate(_towerPrefab, _spawnPos, Quaternion.identity);
        if (_shouldBoost)
        {
            //spawnedChessPawn.Boost();
        }
        SpawnedChessPawns.Add(spawnedChessPawn);
    }

    public override void SpecialAttack()
    {
        base.SpecialAttack();
        foreach(GameObject spawnedChessPawn in SpawnedChessPawns)
        {
            //spawnedChessPawn.Boost();
        }
    }
}