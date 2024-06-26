using BehaviorTree;
using UnityEngine;

public class RB_AICheck_IsTargetAlive : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    public RB_AICheck_IsTargetAlive(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _transform = btParent.transform;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        Transform target = (Transform)_btParent.Root.GetData("target");
        
        if (target != null)
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(target, out RB_Health health) && !health.Dead)
            {
                _state = BTNodeState.SUCCESS;
            }
            else
            {
                _state = BTNodeState.FAILURE;
            }
        }

        return _state;
    }
}