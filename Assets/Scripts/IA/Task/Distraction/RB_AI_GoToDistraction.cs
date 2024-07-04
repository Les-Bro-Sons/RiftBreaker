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

        Vector3 distractionDirection = (currentDistraction.transform.position - _transform.position);
        if (distractionDirection.magnitude > _btParent.DistractionDistanceNeeded || (Physics.Raycast(_transform.position, distractionDirection.normalized, Mathf.Clamp(distractionDirection.magnitude - 0.5f, 0, float.MaxValue), (1 << 3))))
        {
            _btParent.AiMovement.MoveToPosition(currentDistraction.transform.position, _btParent.MoveToDistractionSpeed);
            return _state = BTNodeState.RUNNING;
        }
        else
        {
            return _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
