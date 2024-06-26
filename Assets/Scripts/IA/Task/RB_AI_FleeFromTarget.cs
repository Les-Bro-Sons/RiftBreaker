using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_FleeFromTarget : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    private Transform _target;

    float _distanceNeeded;
    float _speed;
    float _acceleration;

    public RB_AI_FleeFromTarget(RB_AI_BTTree btParent, float distanceNeeded, float speed = -1, float acceleration = -1)
    {
        _btParent = btParent;
        _transform = btParent.transform;
        _distanceNeeded = distanceNeeded;
        if (speed == -1)
            _speed = _btParent.AiMovement.MovementMaxSpeed;
        else
            _speed = speed;
        if (acceleration == -1)
            _acceleration = _btParent.AiMovement.MovementAcceleration;
        else
            _acceleration = acceleration;
    }

    public override BTNodeState Evaluate()
    {
        _target = (Transform)_btParent.Root.GetData("target");
        if (_target == null)
        {
            return _state = BTNodeState.FAILURE;
        }

        if (Vector3.Distance(_target.position, _transform.position) >= _distanceNeeded )
        {
            return _state = BTNodeState.SUCCESS;
        }
        else
        {
            _btParent.AiMovement.MoveIntoDirection((_transform.position - _target.position).normalized, _speed, _acceleration);
            return _state = BTNodeState.RUNNING;
        }
    }
}
