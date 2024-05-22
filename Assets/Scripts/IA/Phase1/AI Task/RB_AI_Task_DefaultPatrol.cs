using BehaviorTree;
using UnityEngine;

public class RB_AI_Task_DefaultPatrol : RB_BTNode
{
    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    private Transform _transform;
    //private Animator _animator;
    private bool _waiting = false;
    private float _waitBeforeToMove = 1f; // in seconds
    private float _waitCounter = 0f;


    public RB_AI_Task_DefaultPatrol(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        _waypoints = waypoints;
        //_animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {

        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitBeforeToMove)
            {
                _waiting = false;
                //_animator.SetBool("Walking", true);
            }
        }
        else
        {
            Transform wp = _waypoints[_currentWaypointIndex];

            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                //_animator.SetBool("Walking", false);
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, wp.position, RB_AIInf_BTTree.Speed * Time.deltaTime);
                //_transform.LookAt(wp.position);
            }
        }


        _state = BTNodeState.RUNNING;
        return _state;
    }
}