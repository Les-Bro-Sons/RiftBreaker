using BehaviorTree;
using UnityEngine;

public class RB_AI_FollowLeader : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private Transform _leader;

    private float _distanceRequired;

    public RB_AI_FollowLeader(RB_AI_BTTree BtParent, float distanceRequired = 3)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        _leader = RB_PlayerController.Instance.transform;

        _distanceRequired = distanceRequired;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        if (_leader == null) return _state;

        if (Vector3.Distance(_transform.position, _leader.position) > _distanceRequired)
        {
            _btParent.AiMovement.MoveToPosition(_leader.position, _btParent.MovementSpeed);
            _state = BTNodeState.RUNNING;
        }
        else
        {
            _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
