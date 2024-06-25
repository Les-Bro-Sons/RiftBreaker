using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RB_AICheck_EnemyInRoom : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private List<RB_Health> _enemies;

    private TARGETMODE _targetmode = TARGETMODE.Closest;

    private bool _setTarget;

    private float _checkNearbyTimer = 0;
    private float _checkNearbyTime = 1;

    public RB_AICheck_EnemyInRoom(RB_AI_BTTree btParent, TARGETMODE targetMode, bool setTarget = true)
    {
        _btParent = btParent;
        _transform = btParent.transform;
        _targetmode = targetMode;
        _setTarget = setTarget;
        _checkNearbyTimer += Random.Range(-0.25f, 0.25f);
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        int? room = RB_RoomManager.Instance.GetEntityRoom(_btParent.AiHealth.Team, _btParent.gameObject);
        if (room == null) return BTNodeState.FAILURE;

        _enemies = (_btParent.AiHealth.Team == TEAMS.Ai)
            ? RB_RoomManager.Instance.GetDetectedAllies(room.Value).ToList()
            : RB_RoomManager.Instance.GetDetectedEnemies(room.Value).ToList();

        if (_enemies.Count == 0)
        {
            float nearbyDetectionRange = _btParent.FovRange;
            if (_checkNearbyTimer >= _checkNearbyTime)
            {
                _checkNearbyTimer = 0;
                foreach (Collider collider in Physics.OverlapSphere(_transform.position, nearbyDetectionRange))
                {
                    if (RB_Tools.TryGetComponentInParent<RB_Health>(collider.gameObject, out RB_Health enemyHealth)
                        && enemyHealth.Team != _btParent.AiHealth.Team
                        && Physics.Raycast(_transform.position, (enemyHealth.transform.position - _transform.position).normalized, out RaycastHit hit, nearbyDetectionRange, ~((1 << 6) | (1 << 10)))
                        && RB_Tools.TryGetComponentInParent<RB_Health>(hit.collider.gameObject, out RB_Health enemyCheck)
                        && enemyCheck == enemyHealth
                        && !_enemies.Contains(enemyHealth))
                    {
                        _enemies.Add(enemyHealth);
                    }
                }
            }
            else
            {
                _checkNearbyTimer += Time.deltaTime;
            }
        }

        _enemies.RemoveAll(enemy => enemy.Dead);

        if (_enemies.Count == 0) return BTNodeState.FAILURE;

        RB_Health targetEnemy = null;
        float targetDistance = Mathf.Infinity;

        switch (_targetmode)
        {
            case TARGETMODE.Closest:
                foreach (RB_Health enemy in _enemies)
                {
                    float enemyDistance = Vector3.Distance(_transform.position, enemy.transform.position);
                    if (enemyDistance < targetDistance)
                    {
                        targetDistance = enemyDistance;
                        targetEnemy = enemy;
                    }
                }
                break;
            case TARGETMODE.Furthest:
                targetDistance = 0;
                foreach (RB_Health enemy in _enemies)
                {
                    targetDistance = 0;
                    float enemyDistance = Vector3.Distance(_transform.position, enemy.transform.position);
                    if (enemyDistance > targetDistance)
                    {
                        targetDistance = enemyDistance;
                        targetEnemy = enemy;
                    }
                }
                break;
            case TARGETMODE.Random:
                targetEnemy = _enemies[Random.Range(0, _enemies.Count)];
                break;
        }

        if (_setTarget && targetEnemy != null)
        {
            _btParent.Root.SetData("target", targetEnemy.transform);
        }

        _state = BTNodeState.SUCCESS;
        return _state;
    }

}
