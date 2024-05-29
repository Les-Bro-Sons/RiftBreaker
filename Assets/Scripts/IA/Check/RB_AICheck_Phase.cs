using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class RB_AICheck_Phase : RB_BTNode
{
    private List<PHASES> _checkPhases = new();

    public RB_AICheck_Phase(List<PHASES> checkPhases)
    {
        _checkPhases = checkPhases;
    }

    public override BTNodeState Evaluate()
    {
        if (_checkPhases.Contains(RB_LevelManager.Instance.CurrentPhase))
        {
            _state = BTNodeState.SUCCESS;
            return _state;
        }
        else
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }
    }
}
