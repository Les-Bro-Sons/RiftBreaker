using BehaviorTree;

public class RB_AICheck_CurrentDistractionType : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private DISTRACTIONTYPE _distractionType;

    public RB_AICheck_CurrentDistractionType(RB_AI_BTTree btParent, DISTRACTIONTYPE distractionType)
    {
        _btParent = btParent;
        _distractionType = distractionType;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        if (_distractionType == _btParent.CurrentDistraction.DistractionType)
        {
            return _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}
