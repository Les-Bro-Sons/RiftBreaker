using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RB_Mega_knight : RB_Boss
{
    public BOSSSTATES CurrentState;

    [Header("Projectiles")]
    public GameObject Spikes;
    private Vector2 _rangeOfAttack = new Vector2(200, 50);
    public int RowsOfSpikes = 5;
    public int ColumnsOfSpikes = 5;

    [Header("Jump")]
    public float JumpForce = 10f;
    public float JumpHeight = 5f;
    public float DamageRadius = 5f;
    private bool isGrounded;

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
            float startY = transform.position.y - _rangeOfAttack.y / 2;
            float spacingX = _rangeOfAttack.x / RowsOfSpikes;
            float spacingY = _rangeOfAttack.y / ColumnsOfSpikes;

            for (int i = 0; i < RowsOfSpikes; i++)
            {
                for (int j = 0; j < ColumnsOfSpikes; j++)
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
            Vector3 Direction = (PlayerPosition.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, PlayerPosition.position);

            //Vector3 jumpDirection = new Vector3(Direction.x, CalculateVerticalJumpSpeed(float distance), 0);
            CooldownAttack3 = 1f;
            Health.TakeDamage(50f);
        }
    }

   //float CalculateVerticalJumpSpeed(float distance)
   //{
   //
   //}
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
