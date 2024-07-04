using Cinemachine.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RB_TimeBodyRecorder : MonoBehaviour
{
    
    new Transform transform; // to reduce performance cost of calling transform
    private Rigidbody _rb;
    private List<PointInTime> _pointsInTime = new();
    private List<PointInTime> _oldPointsInTime = new();

    private bool _isRewinding = false;

    [SerializeField] private ENTITYTYPES _entityType = ENTITYTYPES.Ai;
    private RB_UXRewindManager _uxRewind;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private RB_Health _health;
    [SerializeField] private RB_Enemy _enemy;
    [SerializeField] private RB_Door _door;
    private RB_AI_BTTree _btTree;

    private Vector3 _savedVelocity; //used because setting the velocity every frame is very unoptimized

    private RB_LevelManager _levelManager;

    private List<EventInTime> _timeEventForNextPoint = new(); //events to save in the next point in time

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>(); // to reduce performance cost of calling transform
        _uxRewind = FindObjectOfType<RB_UXRewindManager>();
        if (!_health)
            _health = GetComponent<RB_Health>();
        if (!_enemy)
            _enemy = GetComponent<RB_Enemy>();
        _levelManager = GetComponent<RB_LevelManager>();
        _btTree = GetComponent<RB_AI_BTTree>();
        _door = GetComponent<RB_Door>();
    }

    private void Start()
    {
        RB_TimeManager.Instance.EventStartRewinding.AddListener(StartRewinding);
        RB_TimeManager.Instance.EventStopRewinding.AddListener(StopRewinding);
        RB_TimeManager.Instance.EventResetRewinding.AddListener(ResetRewinding);
        RB_TimeManager.Instance.EventRecordFrame.AddListener(delegate { RecordTimeFrame(RB_TimeManager.Instance.CurrentTime); });
    }

    private void FixedUpdate()
    {
        if (_isRewinding)
        {
            Rewind();
        }
    }

    public void RecordTimeEvent(EventInTime timeEvent)
    {
        _timeEventForNextPoint.Add(timeEvent);
    }

    private void RecordTimeFrame(float time) // record a new frame with position and rotation of the gameObject at a specific time
    {
        PointInTime newPoint = new PointInTime();
        newPoint.Time = time;
        newPoint.Position = transform.position;
        newPoint.Rotation = transform.rotation;
        if (_spriteRenderer)
            newPoint.Sprite = _spriteRenderer.sprite; //save current sprite
        if (_health)
        {
            newPoint.Health = _health.Hp;
            newPoint.MaxHealth = _health.HpMax;
            newPoint.Dead = _health.Dead;
            newPoint.Team = _health.Team;
        }
        if (_rb) newPoint.Velocity = _rb.velocity;
        if (_levelManager) newPoint.Phase = _levelManager.CurrentPhase;
        if (_btTree)
        {
            newPoint.BoolDictionnary = _btTree.BoolDictionnary.ToDictionary(entry => entry.Key, entry => entry.Value); //save the bool value of the bt tree
            if (_btTree.ImageSpotBar) newPoint.SpotValue = _btTree.ImageSpotBar.fillAmount; //save the current spot value
            newPoint.CurrentWaypointIndex = _btTree.CurrentWaypointIndex; //save the current patrol index
            newPoint.Distractions = _btTree.Distractions.ToList();
            newPoint.AlreadySeenDistractions = _btTree.AlreadySeenDistractions.ToList();
        }
        newPoint.TimeEvents = _timeEventForNextPoint.ToList();

        _pointsInTime.Add(newPoint);
        _timeEventForNextPoint.Clear();
    }

    private void Rewind(bool interpolate = true, PointInTime? GoToPointInTime = null) //rewind the body
    {
        if (_pointsInTime.Count <= 1)
        {
            Destroy(gameObject);
            return;
        }

        float currentTime = RB_TimeManager.Instance.CurrentTime;

        //PointInTime closestPointInTime = GetClosestPointInTime(currentTime, true);

        PointInTime currentP;
        if (!GoToPointInTime.HasValue)
        {
            RemoveLastPointIfFuture(currentTime); // remove the points that are in the future


            PointInTime closestPointInTime = _pointsInTime[_pointsInTime.Count - 1];
            if (_pointsInTime.Count > 1) 
            {
                PointInTime nextPointInTime = _pointsInTime[_pointsInTime.Count - 2];
                if (interpolate)
                {
                    currentP = closestPointInTime.InterpolateValues(nextPointInTime, currentTime); //INTERPOLATION
                }
                else
                {
                    currentP = nextPointInTime;
                }
            }
            else
            {
                currentP = closestPointInTime;
            }
        }
        else
        {
            currentP = GoToPointInTime.Value;
        }

        if (!currentP.Position.IsNaN()) //fix errors
        {
            if (_rb && !GoToPointInTime.HasValue) //rigidbody rewind
            {
                _rb.MovePosition(currentP.Position);
                _rb.rotation = currentP.Rotation;
                _savedVelocity = currentP.Velocity;
            }
            else
            {
                transform.SetPositionAndRotation(currentP.Position, currentP.Rotation); 
            }
        }

        if (GoToPointInTime.HasValue)
        {
            _savedVelocity = Vector3.zero;
        }

        if (currentP.Sprite) _spriteRenderer.sprite = currentP.Sprite;
        if (_health) //health rewind
        {
            _health.HpMax = currentP.MaxHealth;
            if (_health.Hp > currentP.Health)
                _health.TakeDamage(Mathf.Abs(_health.Hp - currentP.Health), true);
            else if (_health.Hp < currentP.Health)
                _health.Heal(Mathf.Abs(_health.Hp - currentP.Health), true);

            if (_health.Dead != currentP.Dead && _enemy)
            {
                if (currentP.Dead) //die
                {
                    _enemy.Tombstone();
                }
                else //revive
                {
                    _enemy.UnTombstone();
                }
            }
            _health.Dead = currentP.Dead;
            if (_health.Team != currentP.Team)
            {
                _health.Team = currentP.Team;
                _enemy.SetLayerToTeam();
            }
        }
        if (_levelManager && _levelManager.CurrentPhase != currentP.Phase) //level manager rewind
            _levelManager.SwitchPhase(currentP.Phase);
        if (_btTree) //bttree rewind
        {
            _btTree.BoolDictionnary = currentP.BoolDictionnary.ToDictionary(entry => entry.Key, entry => entry.Value);
            if (_btTree.ImageSpotBar) _btTree.ImageSpotBar.fillAmount = currentP.SpotValue;
            _btTree.CurrentWaypointIndex = currentP.CurrentWaypointIndex;
            _btTree.Distractions = currentP.Distractions.ToList();
            _btTree.AlreadySeenDistractions = currentP.AlreadySeenDistractions.ToList();
        }
        foreach (EventInTime timeEvent in currentP.TimeEvents) // EVENTS
        {
            switch(timeEvent.TypeEvent)
            {
                case TYPETIMEEVENT.TookWeapon: // if an item has been taken
                    timeEvent.ItemTook.Drop();
                    break;
                case TYPETIMEEVENT.CloseDoor:
                    _door.Open();
                    break;
                case TYPETIMEEVENT.OpenDoor:
                    _door.Close();
                    break;
            }
        }
    }

    public void GoToFirstPointInTime()
    {
        if (_pointsInTime.Count > 0) Rewind(false, _pointsInTime[0]);
    }

    private void StartRewinding()
    {
        _oldPointsInTime = _pointsInTime.ToList(); //used for reset rewind
        _isRewinding = true;
        if (_rb)
            _rb.isKinematic = true;
        if (_animator)
            _animator.enabled = false;
        if (_btTree)
            _btTree.enabled = false;
        if (_enemy)
            _enemy.enabled = false;
    }

    private void StopRewinding()
    {
        _isRewinding = false;
        if (_rb)
        {
            _rb.isKinematic = false;
            _rb.velocity = _savedVelocity;
        }
        if (_health && !_health.Dead && _animator)
            _animator.enabled = true;
        if (_health && !_health.Dead && _btTree)
            _btTree.enabled = true;
        if (_enemy)
            _enemy.enabled = true;
        RemoveFuturePointsInTime(RB_TimeManager.Instance.CurrentTime); // remove the points that are in the future since we stop rewinding
    }
    
    private void ResetRewinding() //scrapped
    {
        _pointsInTime = _oldPointsInTime.ToList();
        Rewind(false, _pointsInTime[_pointsInTime.Count - 1]);
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

    private void RemoveLastPointIfFuture(float currentTime)
    {
        if (_pointsInTime[_pointsInTime.Count - 2].Time > currentTime) //if the last point is completely in the future, it deletes it
        {
            _pointsInTime.RemoveAt(_pointsInTime.Count - 1);
        }
    }
}