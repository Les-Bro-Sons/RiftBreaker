using System.Collections.Generic;
using UnityEngine;

public class RB_TimeBodyRecorder : MonoBehaviour
{
    
    new Transform transform; // to reduce performance cost of calling transform
    private Rigidbody _rb;
    private List<PointInTime> _pointsInTime = new();

    private bool _isRewinding = false;

    [SerializeField] private ENTITYTYPES _entityType = ENTITYTYPES.Ai;
    private RB_UXRewindManager _uxRewind;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private RB_Health _health;
    [SerializeField] private RB_Enemy _enemy;
    [SerializeField] private ParticleSystem _particleSystem;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>(); // to reduce performance cost of calling transform
        _uxRewind = FindObjectOfType<RB_UXRewindManager>();
        if (!_health)
            _health = GetComponent<RB_Health>();
        if (!_enemy)
            _enemy = GetComponent<RB_Enemy>();
        if (!_particleSystem)
            _particleSystem = GetComponent<ParticleSystem>();
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
        PointInTime newPoint = new PointInTime();
        newPoint.Time = time;
        newPoint.Position = transform.position;
        newPoint.Rotation = transform.rotation;
        if (_spriteRenderer)
            newPoint.Sprite = _spriteRenderer.sprite;
        if (_health)
        {
            newPoint.Health = _health.Hp;
            newPoint.Dead = _health.Dead;
        }

        _pointsInTime.Add(newPoint);
    }

    private void Rewind()
    {
        if (_pointsInTime.Count <= 1)
        {
            Destroy(gameObject);
            return;
        }

        float currentTime = RB_TimeManager.Instance.CurrentTime;

        PointInTime closestPointInTime = GetClosestPointInTime(currentTime, true);
        transform.position = closestPointInTime.Position;
        transform.rotation = closestPointInTime.Rotation;
        if (closestPointInTime.Sprite) _spriteRenderer.sprite = closestPointInTime.Sprite;
        if (_health)
        {
            _health.Hp = closestPointInTime.Health;
            if (_health.Dead != closestPointInTime.Dead && _enemy)
            {
                if (closestPointInTime.Dead) //die
                {
                    _enemy.Tombstone();
                }
                else //revive
                {
                    _enemy.UnTombstone();
                }
                _health.Dead = closestPointInTime.Dead;
            }
        }
    }

    private void StartRewinding()
    {
        _isRewinding = true;
        if (_rb)
            _rb.isKinematic = true;
        if (_animator)
            _animator.enabled = false;
        UxStartRewind();
    }

    private void StopRewinding()
    {
        _isRewinding = false;
        if (_rb)
            _rb.isKinematic = false;
        if (_animator)
            _animator.enabled = true;
        RemoveFuturePointsInTime(RB_TimeManager.Instance.CurrentTime); // remove the points that are in the future since we stop rewinding

        UxStopRewind();
    }

    private PointInTime GetClosestPointInTime(float currentTime, bool removeFuture = false) //removeFuture: remove the point in future when it's not the closest anymore
    {
        if (_pointsInTime.Count == 1)
            return _pointsInTime[0];

        float timeDifference = 0;
        PointInTime lastPoint = _pointsInTime[_pointsInTime.Count - 1];
        PointInTime beforeLastPoint = _pointsInTime[_pointsInTime.Count - 2];

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
        foreach (PointInTime point in _pointsInTime)
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