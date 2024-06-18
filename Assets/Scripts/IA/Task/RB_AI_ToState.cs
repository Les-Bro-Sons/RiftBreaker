using BehaviorTree;

public class RB_AI_ToState : RB_BTNode
{
    private RB_BTNode _nodeChanged;
    private BTNodeState _toState;

    public RB_AI_ToState(RB_BTNode nodeChanged, BTNodeState toState)
    {
        _nodeChanged = nodeChanged;
        _toState = toState;
    }

    public override BTNodeState Evaluate()
    {
        _state = _nodeChanged.Evaluate();
        return _state = _toState;
    }
}
