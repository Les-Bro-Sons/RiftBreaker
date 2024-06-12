using System.Collections.Generic;
using MANAGERS;
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
        _playerTransform.forward = RB_InputManager.Instance.GetMouseDirection();
        _spawnPos = _playerTransform.position + _playerTransform.forward * _pawnSpawnDistance;
        GameObject spawnedChessPawn = Instantiate(_pawnPrefab, _spawnPos, Quaternion.identity);
        if (_shouldBoost)
        {
            //spawnedChessPawn.Boost();
        }
        SpawnedChessPawns.Add(spawnedChessPawn);
        RB_AudioManager.Instance.PlaySFX("chess_move", RB_PlayerController.Instance.transform.position,0, 1);

    }

    public override void ChargedAttack()
    {
        base.ChargedAttack();
        _playerTransform.forward = RB_InputManager.Instance.GetMouseDirection();
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
        RB_AudioManager.Instance.PlaySFX("attack_Spe_Chess", _transform.position, 0, 1);
        foreach(GameObject spawnedChessPawn in SpawnedChessPawns)
        {
            //spawnedChessPawn.Boost();
        }
    }

    public override void ChooseSfx() {
        base.ChooseSfx();
        RB_AudioManager.Instance.PlaySFX("sheating_Chess", RB_PlayerController.Instance.transform.position, 0,1f);
    }
}
