using BehaviorTree;
using System.Linq;
using UnityEngine;

public class RB_AI_GetHighestPriorityDistraction : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;

    private TARGETMODE _targetMode;

    public RB_AI_GetHighestPriorityDistraction(RB_AI_BTTree btParent, TARGETMODE targetMode)
    {
        _btParent = btParent;
        _targetMode = targetMode;
        _transform = btParent.transform;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        switch(_targetMode)
        {
            case TARGETMODE.Closest:
                _btParent.Distractions.OrderBy(x => Vector2.Distance(_transform.position, x.Position)).ToList();
                _btParent.CurrentDistraction = _btParent.Distractions[0];
                break;
            case TARGETMODE.Furthest:
                _btParent.Distractions.OrderBy(x => Vector2.Distance(_transform.position, x.Position)).ToList();
                _btParent.CurrentDistraction = _btParent.Distractions[_btParent.Distractions.Count - 1];
                break;
            case TARGETMODE.Random:
                _btParent.CurrentDistraction = _btParent.Distractions[Random.Range(0, _btParent.Distractions.Count)];
                break;
        }
        _state = BTNodeState.SUCCESS;

        return _state;
    }
}
