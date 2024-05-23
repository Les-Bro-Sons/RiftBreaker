using BehaviorTree;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Splines;

public class RB_AI_Task_DefaultPatrol : RB_BTNode
{
    private RB_AIInf_BTTree _btParent;

    //private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;
    private int _patrolDirection = 1; // used if patrol spline not closed

    private int _currentInterval = 0;

    private Transform _transform;
    //private Animator _animator;
    private bool _isWaiting = false;
    private float _waitCounter = 0f;

    private SplineContainer _splineContainer;
    private float _splineDistPercentage = 0f;
    private float _splineLength = 0f;
    private float _someThreshold = 1f; // Plus la distance est petite plus elle est precise

    private List<Vector3> _splinePointsPos = new();

    public RB_AI_Task_DefaultPatrol(RB_AIInf_BTTree BtParent)
    {
        _btParent = BtParent;

        _transform = _btParent.transform;
        _splineContainer = _btParent.SplineContainer;


        _splineLength = _splineContainer.CalculateLength();



        if (_btParent.PatrolSplineIndex < _splineContainer.Splines.Count)
            foreach (BezierKnot points in _splineContainer.Splines[_btParent.PatrolSplineIndex].Knots)
                            _splinePointsPos.Add(points.Position);

        //_animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        if (_isWaiting)
        {
            _waitCounter += Time.deltaTime;
            
            if (_waitCounter >= _btParent.WaitBeforeToMoveToNextWaypoint)
            {
                _isWaiting = false;
                //_animator.SetBool("Walking", true);
            }
        }
        else
        {
/*            Vector3 currentPosition = _splineContainer.EvaluatePosition(_splineDistPercentage);
            //Vector3 nextPosition = _splineContainer.EvaluatePosition(_splineDistPercentage + 0.05f);
            Vector3 nextPosition = _splinePointsPos[(_currentWaypointIndex + 1) % _splinePointsPos.Count]; // Utilise le prochain point de la spline

            // Vérifie si la position actuelle est proche de la position suivante
            if (Vector3.Distance(currentPosition, nextPosition) < _someThreshold) // '_someThreshold' distance acceptable
            {
                _waitCounter = 0f;
                _isWaiting = true;
            }
            else
            {
                _splineDistPercentage += _btParent.MovementSpeed * Time.deltaTime / _splineLength;
                _transform.position = currentPosition;
                _splineDistPercentage %= 1;

            
                Vector3 currenRotation = _splineContainer.EvaluatePosition(_splineDistPercentage);
                //Vector3 nextPosition = _splineContainer.EvaluatePosition(_splineDistPercentage + 0.05f);
                Vector3 nextRotation = _splinePointsPos[(_currentWaypointIndex + 1) % _splinePointsPos.Count]; // Utilise le prochain point de la spline
                
                Vector3 direction = nextPosition - currentPosition;
                _transform.rotation = Quaternion.LookRotation(direction, _transform.up);
            }*/
            
            
            float t = _currentWaypointIndex / (float)(_splinePointsPos.Count - 1);
            Vector3 targetPosition = _splineContainer.Splines[_btParent.PatrolSplineIndex].EvaluatePosition(t);

            //int t = (_currentWaypointIndex + 1) / _splinePointsPos.Count; // t => periode

            if (Vector3.Distance(_transform.position, targetPosition) < 0.01f)
            {
                _transform.position = targetPosition;
                _currentInterval += 1;
                if (_btParent.HasAnInterval && _currentInterval >= _btParent.StartWaitingWaypointInterval)
                {
                    StartWaiting();
                    _currentInterval = 0;
                }


                if (_splineContainer.Splines[_btParent.PatrolSplineIndex].Closed)
                {
                    _currentWaypointIndex = (_currentWaypointIndex + 1) % _splinePointsPos.Count;
                    if (_currentWaypointIndex == 0) // at the end of spline
                    {
                        StartWaiting();
                    }
                }
                else
                {
                    _currentWaypointIndex += _patrolDirection;
                    if (_currentWaypointIndex >= _splinePointsPos.Count || _currentWaypointIndex < 0) // at the end of spline
                    {
                        _patrolDirection *= -1;
                        _currentWaypointIndex += _patrolDirection;
                        StartWaiting();
                    }

                }

//                _currentWaypointIndex = (_currentWaypointIndex + 1) % spline.KnotCount;
                //_animator.SetBool("Walking", false);
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, targetPosition, _btParent.MovementSpeed * Time.deltaTime);
                //_transform.LookAt(wp.position);
            }
        }

        _state = BTNodeState.RUNNING;
        return _state;
    }

    private void StartWaiting()
    {
        _waitCounter = 0f;
        _isWaiting = true;
    }
}