using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RB_AICheck_IsDistracted : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    public RB_AICheck_IsDistracted(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        _btParent.Distractions.RemoveAll(value => value == null);

        if (_btParent.Distractions.Count > 0 )
        {
            return _state = BTNodeState.SUCCESS;
        }

        return _state;
    }
}

public class RB_AICheck_IsDistractedByType : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private DISTRACTIONTYPE _typeCheck;

    public RB_AICheck_IsDistractedByType(RB_AI_BTTree btParent, DISTRACTIONTYPE distractionType)
    {
        _btParent = btParent;
        _typeCheck = distractionType;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        _btParent.Distractions.RemoveAll(value => value == null);

        bool hasType = false;

        foreach (RB_Distraction distraction in _btParent.Distractions)
        {
            if (distraction.DistractionType == _typeCheck) return _state = BTNodeState.SUCCESS;
        }

        return _state = BTNodeState.FAILURE;
    }
}