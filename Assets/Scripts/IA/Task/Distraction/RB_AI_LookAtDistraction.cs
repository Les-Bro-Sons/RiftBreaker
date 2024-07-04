using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_LookAtDistraction : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Rigidbody _rb;
    private Transform _transform;

    public RB_AI_LookAtDistraction(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _rb = _btParent.AiRigidbody;
    }

    public override BTNodeState Evaluate()
    {
        if (_btParent.CurrentDistraction == null) return _state = BTNodeState.FAILURE;

        _btParent.AiMovement.RotateToward((_btParent.CurrentDistraction.transform.position - _rb.position).normalized);

        return _state = BTNodeState.SUCCESS;
    }
}
