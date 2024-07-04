using BehaviorTree;
using UnityEngine;

public class RB_AI_CompleteDistraction : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    public RB_AI_CompleteDistraction(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _transform = btParent.transform;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        if (_btParent.CurrentDistraction != null)
        {
            _btParent.AlreadySeenDistractions.Add(_btParent.CurrentDistraction);
            _btParent.Distractions.Remove(_btParent.CurrentDistraction);
            return _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
