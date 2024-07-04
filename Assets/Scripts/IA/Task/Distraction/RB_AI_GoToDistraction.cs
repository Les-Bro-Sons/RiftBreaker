using BehaviorTree;
using UnityEngine;

public class RB_AI_GoToDistraction : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    public RB_AI_GoToDistraction(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _transform = btParent.transform;
    }

    public override BTNodeState Evaluate()
    {
        RB_Distraction currentDistraction = _btParent.CurrentDistraction;
        _state = BTNodeState.FAILURE;

        if (Vector3.Distance(_transform.position, currentDistraction.Position) > _btParent.DistractionDistanceNeeded)
        {
            _btParent.AiMovement.MoveToPosition(currentDistraction.Position, _btParent.MoveToDistractionSpeed);
            return _state = BTNodeState.RUNNING;
        }
        else
        {
            return _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
