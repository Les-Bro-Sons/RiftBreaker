using BehaviorTree;
using UnityEngine;

public class RB_AI_GoToLastTargetPos : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    private float _distanceRequired;
    private float _speed = -1;
    private float _acceleration = -1;

    public RB_AI_GoToLastTargetPos(RB_AI_BTTree btParent, float distanceRequired)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _distanceRequired = distanceRequired;
    }

    public RB_AI_GoToLastTargetPos(RB_AI_BTTree btParent, float distanceRequired, float speed)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _distanceRequired = distanceRequired;
        _speed = speed;
    }

    public RB_AI_GoToLastTargetPos(RB_AI_BTTree btParent, float distanceRequired, float speed, float acceleration)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _distanceRequired = distanceRequired;
        _speed = speed;
        _acceleration = acceleration;
    }

    public override BTNodeState Evaluate()
    {
        Vector3 lastTargetPos = _btParent.LastTargetPos;

        if (Vector3.Distance(lastTargetPos, _transform.position) < _distanceRequired)
        {
            _state = BTNodeState.SUCCESS;
        }
        else
        {
            //_btParent.AiMovement.MoveIntoDirection(lastTargetPos - _transform.position, _speed, _acceleration);
            _btParent.AiMovement.MoveToPosition(lastTargetPos, _speed, _acceleration);
            _state = BTNodeState.RUNNING;
        }

        return _state;
    }
}
