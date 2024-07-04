using BehaviorTree;
using UnityEngine;

public class RB_AICheck_WaitForSeconds : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    private float _waitingForSecond;
    private BTBOOLVALUES _settingBool;
    private bool _boolValue;

    private float _timer; 
    private float _lastTimeWaited; //used to check if the node has been running before

    public RB_AICheck_WaitForSeconds(RB_AI_BTTree btParent, float waitingForSecond)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _waitingForSecond = waitingForSecond;
    }

    public RB_AICheck_WaitForSeconds(RB_AI_BTTree btParent, float waitingForSecond, BTBOOLVALUES settingBool, bool boolValue)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _waitingForSecond = waitingForSecond;
        _settingBool = settingBool;
        _boolValue = boolValue;
    }

    public override BTNodeState Evaluate()
    {
        if (Mathf.Abs(_lastTimeWaited - Time.time) > 2) //time reset if not counted for x seconds
        {
            _timer = 0;
        }

        _lastTimeWaited = Time.time;

        _timer += Time.deltaTime;
        if (_timer >= _waitingForSecond)
        {
            _btParent.BoolDictionnary[_settingBool] = _boolValue;
            _state = BTNodeState.SUCCESS;
        }
        else
        {
            _state = BTNodeState.RUNNING;
        }

        return _state;
    }
}
