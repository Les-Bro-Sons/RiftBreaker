using BehaviorTree;
using UnityEngine;

public class RB_AI_ReverseState : RB_BTNode
{

    private RB_BTNode _nodeReversed;

    public RB_AI_ReverseState(RB_BTNode nodeReversed)
    {
        _nodeReversed = nodeReversed;
    }

    public override BTNodeState Evaluate()
    {
        _state = _nodeReversed.Evaluate();
        switch (_state)
        {
            case BTNodeState.SUCCESS:
                _state = BTNodeState.FAILURE;
                break;
            case BTNodeState.FAILURE:
                _state = BTNodeState.SUCCESS;
                break;
        }

        return _state;
    }
}
