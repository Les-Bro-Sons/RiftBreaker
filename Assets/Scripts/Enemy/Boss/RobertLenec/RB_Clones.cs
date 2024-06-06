using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Clones : RB_Enemy
{
    public GameObject RedBall;
    [SerializeField] private float _redBallOffset = 1f;
    [SerializeField] private float _shootCooldown = 1f;
    protected float _currentCooldown;
    
    private void Update()
    {
        _currentCooldown -= Time.deltaTime;
        GetTarget();
        if (_currentCooldown <= 0)
        {
            CloneShoot();
        }
    }

    public void CloneShoot()
    {
        transform.forward = _currentTarget.position - transform.position;
        Vector3 offset = transform.forward * _redBallOffset;
        Vector3 spawnProjectile = transform.position + offset;
        Instantiate(RedBall, spawnProjectile, transform.rotation);

        _currentCooldown = _shootCooldown;
    }
}
