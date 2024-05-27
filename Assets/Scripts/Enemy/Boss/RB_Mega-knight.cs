using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RB_Mega_knight : RB_Boss
{
    public BOSSSTATES CurrentState;

    [Header("Projectiles")]
    public GameObject Spikes;

    private Vector2 _rangeOfAttack = new Vector2(200, 50);
    private int _rows = 5;
    private int _columns = 5;
    

    void Start()
    {
        JumpAttack();
    }

    private void Update()
    {
        CooldownAttack1 = CooldownAttack1 - Time.deltaTime;
        CooldownAttack2 = CooldownAttack2 - Time.deltaTime; 
        CooldownAttack3 = CooldownAttack3 - Time.deltaTime;

    }
    public void BaseAttack() 
    {
        if (DistanceFromPlayer <= 0.5f)
        {
            CooldownAttack1 = 1f;
            Health.TakeDamage(30f);
        }
    }

    public void KickAttack()
    {
        if (DistanceFromPlayer <= 10f && DistanceFromPlayer >= 5f)
        {
            float startX = transform.position.x - _rangeOfAttack.x / 2;
            float startY = transform.position.y - DistanceFromPlayer / 2;
            float spacingX = _rangeOfAttack.x / _rows;
            float spacingY = DistanceFromPlayer / _columns;

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    Vector3 SpawnPosition = new Vector3(startX + j * spacingX, startY + i * spacingY, 0);
                    Instantiate(Spikes, SpawnPosition, Quaternion.identity, transform);
                }
            }
            CooldownAttack2 = 1f;
        }
    }

    public void JumpAttack()
    {
        if (DistanceFromPlayer > 10f)
        {
            CooldownAttack3 = 1f;
            Health.TakeDamage(50f);
        }
    }

    private void SwitchBossState()
    {
        switch (CurrentState)
        {
            case BOSSSTATES.Attack1:
                BaseAttack();
                CurrentState = BOSSSTATES.Idle;
                break;
            case BOSSSTATES.Attack2:
                KickAttack();
                CurrentState = BOSSSTATES.Idle;
                break;
            case BOSSSTATES.Attack3:
                JumpAttack();
                CurrentState = BOSSSTATES.Idle;
                break;
            case BOSSSTATES.Idle:
                if (DistanceFromPlayer <= 0.5f)
                {
                    CurrentState = BOSSSTATES.Attack1;
                }

                if (DistanceFromPlayer <= 10f && DistanceFromPlayer >= 5f)
                {
                    CurrentState = BOSSSTATES.Attack2;
                }

                if (DistanceFromPlayer > 10f)
                {
                    CurrentState = BOSSSTATES.Attack3;
                }

                if (MovementSpeed >= 0.1f)
                {
                    CurrentState = BOSSSTATES.Moving;
                }
                break;

            case BOSSSTATES.Moving:
                if (DistanceFromPlayer <= 0.5f)
                {
                    CurrentState = BOSSSTATES.Attack1;
                }

                if (DistanceFromPlayer <= 10f && DistanceFromPlayer >= 5f)
                {
                    CurrentState = BOSSSTATES.Attack2;
                }

                if (DistanceFromPlayer > 10f)
                {
                    CurrentState = BOSSSTATES.Attack3;
                }

                if (MovementSpeed == 0f)
                {
                    CurrentState = BOSSSTATES.Idle;
                }
                break;

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _rangeOfAttack);
    }
}
