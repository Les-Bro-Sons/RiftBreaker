using BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RB_AICheck_EnemyInRoom : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private List<RB_Health> _enemies;

    private TARGETMODE _targetmode = TARGETMODE.Closest;

    private bool _setTarget;

    public RB_AICheck_EnemyInRoom(RB_AI_BTTree btParent, TARGETMODE targetMode, bool setTarget = true)
    {
        _btParent = btParent;
        _transform = btParent.transform;
        _targetmode = targetMode;
        _setTarget = setTarget;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        int? room = RB_RoomManager.Instance.GetEntityRoom(_btParent.AiHealth.Team, _btParent.gameObject);
        if (room == null) return BTNodeState.FAILURE;

        if (_btParent.AiHealth.Team == TEAMS.Ai)
        {
            _enemies = RB_RoomManager.Instance.GetDetectedAllies(room.Value);
        }
        else
        {
            _enemies = RB_RoomManager.Instance.GetDetectedEnemies(room.Value);
        }

        foreach (RB_Health enemy in _enemies.ToList())
        {
            if (enemy.Dead)
                _enemies.Remove(enemy);
        }

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
                foreach (RB_Health enemy in _enemies)
                {
                    float enemyDistance = Vector3.Distance(_transform.position, enemy.transform.position);
                    if (enemyDistance > targetDistance)
                    {
                        targetDistance = enemyDistance;
                        targetEnemy = enemy;
                    }
                }
                break;
            case TARGETMODE.Random:
                targetEnemy = _enemies[Random.Range(0, _enemies.Count - 1)];
                break;
        }

        if (_setTarget)
        {
            _btParent.Root.SetData("target", targetEnemy.transform);
        }
        _state = BTNodeState.SUCCESS;

        return _state;
    }
}
