using BehaviorTree;
using UnityEngine;

public class RB_AICheck_IsTargetClose : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    private Transform _target;

    private float _distanceNeeded;

    public RB_AICheck_IsTargetClose(RB_AI_BTTree btParent, float distanceNeeded)
    {
        _btParent = btParent;
        _transform = btParent.transform;
        _distanceNeeded = distanceNeeded;
    }

    public override BTNodeState Evaluate()
    {
        _target = (Transform)_btParent.Root.GetData("target");

        if (_target == null)
        {
            return _state = BTNodeState.FAILURE;
        }

        if (Vector3.Distance(_transform.position, _target.position) <= _distanceNeeded)
        {
            return _state = BTNodeState.SUCCESS;
        }
        else
        {
            return _state = BTNodeState.FAILURE;
        }
    }
}

public class RB_AICheck_IsTargetFar : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    private Transform _target;

    private float _distanceNeeded;

    public RB_AICheck_IsTargetFar(RB_AI_BTTree btParent, float distanceNeeded)
    {
        _btParent = btParent;
        _transform = btParent.transform;
        _distanceNeeded = distanceNeeded;
    }

    public override BTNodeState Evaluate()
    {
        _target = (Transform)_btParent.Root.GetData("target");

        if (_target == null)
        {
            return _state = BTNodeState.FAILURE;
        }

        if (Vector3.Distance(_transform.position, _target.position) > _distanceNeeded)
        {
            return _state = BTNodeState.SUCCESS;
        }
        else
        {
            return _state = BTNodeState.FAILURE;
        }
    }
}