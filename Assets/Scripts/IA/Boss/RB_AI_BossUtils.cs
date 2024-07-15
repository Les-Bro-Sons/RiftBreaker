using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_WaitInIdle : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    public RB_AI_WaitInIdle(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        if (_btParent.CurrentWaitInIdle > 0)
        {
            _btParent.CurrentWaitInIdle -= Time.deltaTime;
            _state = BTNodeState.RUNNING;
        }

        if (_btParent.CurrentWaitInIdle <= 0) 
        {
            _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}

public class RB_AI_CooldownProgress : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    public RB_AI_CooldownProgress(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.SUCCESS;

        _btParent.Attack1CurrentCooldown -= Time.deltaTime;
        _btParent.Attack2CurrentCooldown -= Time.deltaTime;
        _btParent.Attack3CurrentCooldown -= Time.deltaTime;
        _btParent.CurrentCooldownBetweenAttacks -= Time.deltaTime;
        _btParent.CurrentWaitInIdle -= Time.deltaTime;

        return _state;
    }
}
