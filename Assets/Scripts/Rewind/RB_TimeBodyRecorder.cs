using System.Collections.Generic;
using UnityEngine;

public class RB_TimeBodyRecorder : MonoBehaviour
{
    new Transform transform; // to reduce performance cost of calling transform
    private Rigidbody _rb;
    private List<RB_PointInTime> _pointsInTime = new();

    private bool _isRewinding = false;

    [SerializeField] private ENTITYTYPES _entityType = ENTITYTYPES.Ai;
    private RB_UXRewindManager _uxRewind;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>(); // to reduce performance cost of calling transform
        _uxRewind = FindObjectOfType<RB_UXRewindManager>();
    }

    private void Start()
    {
        RB_InputManager.Instance.EventRewindStarted.AddListener(StartRewinding); // TO TEST
        RB_InputManager.Instance.EventRewindCanceled.AddListener(StopRewinding); // TO TEST
        RB_TimeManager.Instance.EventRecordFrame.AddListener(delegate { RecordTimeFrame(RB_TimeManager.Instance.CurrentTime); });
    }

    private void FixedUpdate()
    {
        if (_isRewinding)
        {
            Rewind();
        }
    }

    private void RecordTimeFrame(float time) // record a new frame with position and rotation of the gameObject at a specific time
    {
        _pointsInTime.Add(new RB_PointInTime(time, transform.position, transform.rotation));
    }

    private void StartRewinding()
    {
        _isRewinding = true;
        if (_rb)
        {
            _rb.isKinematic = true;
        }
        UxStartRewind();
    }

    private void StopRewinding()
    {
        _isRewinding = false;
        if (_rb)
        {
            _rb.isKinematic = false;
        }
        RemoveFuturePointsInTime(RB_TimeManager.Instance.CurrentTime); // remove the points that are in the future since we stop rewinding

        UxStopRewind();
    }

    private void Rewind()
    {
        if (_pointsInTime.Count == 0)
            return;

        float currentTime = RB_TimeManager.Instance.CurrentTime;

        RB_PointInTime closestPointInTime = GetClosestPointInTime(currentTime, true);
        transform.position = closestPointInTime.Position;
        transform.rotation = closestPointInTime.Rotation;    
    }

    private RB_PointInTime GetClosestPointInTime(float currentTime, bool removeFuture = false) //removeFuture: remove the point in future when it's not the closest anymore
    {
        if (_pointsInTime.Count == 1)
            return _pointsInTime[0];

        float timeDifference = 0;
        RB_PointInTime lastPoint = _pointsInTime[_pointsInTime.Count - 1];
        RB_PointInTime beforeLastPoint = _pointsInTime[_pointsInTime.Count - 2];

        timeDifference = Mathf.Abs(currentTime - lastPoint.Time); // time difference between last point and now

        if (timeDifference > Mathf.Abs(beforeLastPoint.Time - currentTime)) // check if the last point in the list is not the closest
        {
            if (removeFuture)
                _pointsInTime.Remove(lastPoint); // delete the last point in the list if it's not the closest

            return beforeLastPoint;
        }
        else
        {
            return lastPoint;
        }
    }

    private void RemoveFuturePointsInTime(float currentTime)
    {
        foreach (RB_PointInTime point in _pointsInTime)
        {
            if (point.Time > currentTime)
            {
                _pointsInTime.Remove(point);
                RemoveFuturePointsInTime(currentTime); // we do this since if we don't it breaks
                return;
            }
        }
    }

    private void UxStartRewind()
    {
        if (_entityType == ENTITYTYPES.Player)
            _uxRewind?.StartRewindTransition();
    }

    private void UxStopRewind()
    {
        if (_entityType == ENTITYTYPES.Player)
        _uxRewind?.StopRewindTransition();
    }
}