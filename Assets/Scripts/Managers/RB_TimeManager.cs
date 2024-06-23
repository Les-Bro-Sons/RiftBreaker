using System.Collections;
using System.Collections.Generic;
using MANAGERS;
using UnityEngine;
using UnityEngine.Events;

public class RB_TimeManager : MonoBehaviour
{
    public static RB_TimeManager Instance;

    [HideInInspector] public UnityEvent EventRecordFrame;
    [HideInInspector] public UnityEvent EventStartRewinding;
    [HideInInspector] public UnityEvent EventStopRewinding;
    [HideInInspector] public UnityEvent EventResetRewinding;
    [HideInInspector] public UnityEvent EventStartFullRewind;
    [HideInInspector] public UnityEvent EventStartNormalRewind;
    [HideInInspector] public UnityEvent EventStopFullRewind;
    [HideInInspector] public UnityEvent EventStopNormalRewind;



    [SerializeField] private float _recordDelay = 0.1f;
    public float DurationRewind = 5f;
    private float _timeWaited = 9999f; // used for delay, it's set at 9999 so it record the first frame

    private float _currentTime = 0; public float CurrentTime { get { return _currentTime; } }
    private float _startRewindTime = 0; public float StartRewindtTime { get { return _startRewindTime; } }

    private bool _isRecording = true;
    public bool IsRewinding = false;
    private bool _fullRewind = false;
    [SerializeField] private float _maxRewindSpeed = 15f;

    [Header("Hourglass")]
    public List<GameObject> HourglassList = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        StartRecording();

        RB_UxHourglassManager.Instance?.NumberOfHourglass(3);
    }
    private void FixedUpdate()
    {
        if (_isRecording && !IsRewinding) // doesn't record if rewinding
        {
            _currentTime += Time.fixedDeltaTime;
            _timeWaited += Time.fixedDeltaTime;
            if (_timeWaited >= _recordDelay) // record a frame when the time waited is superior to the delay
            {
                _timeWaited = 0;
                RecordFrame();
            }
        }
        else if (IsRewinding)
        {
            if (GetRewindLastingTime() >= DurationRewind && !_fullRewind) 
            {
                StopRewinding();
                return;
            }
            else if (_currentTime - Time.fixedDeltaTime <= 0.5f) //stop rewinding if going before the scene was loaded
            {
                if (_fullRewind) RB_UxHourglassManager.Instance?.NumberOfHourglass(3);
                StopRewinding(true);
                return;
            }
            Rewind();
            if (_fullRewind)
            {
                Time.timeScale = Mathf.Clamp(Time.timeScale + Time.fixedDeltaTime / 2.5f, 0, _maxRewindSpeed);
            }
        }
    }

    public float GetRewindLastingTime()
    {
        return Mathf.Abs(_startRewindTime - Time.time);
    }

    public float GetRewindRemainingTime()
    {
        return DurationRewind - Mathf.Abs(_startRewindTime - Time.time);
    }

    public float GetRewindRemainingTimeInSecond()
    {
        return (DurationRewind - Mathf.Abs(_startRewindTime - Time.time)) ;
    }

    private void RecordFrame()
    {
        EventRecordFrame?.Invoke(); // call every rewindable object to record the current frame at this time
    }

    public void StartRecording()
    {
        _isRecording = true;
    }

    public void StopRecording()
    {
        _isRecording = false;
    }

    public void StartRewinding(bool skipChecks = false, bool fullRewind = false)
    {
        if (IsRewinding) return;

        _startRewindTime = Time.time;
        EventRecordFrame?.Invoke(); // used for interpolation
        IsRewinding = true;
        _fullRewind = fullRewind;
        UxStartRewind(fullRewind);
        EventStartRewinding?.Invoke();
        if (fullRewind)
        {
            EventStartFullRewind?.Invoke();
        }
        else 
        {
            EventStartNormalRewind?.Invoke();
        }
    }

    public void StopRewinding(bool stopFullRewind = false, bool recordFrame = false)
    {
        if (!IsRewinding) return;

        if (IsRewinding && (!_fullRewind || stopFullRewind))
        {
            Time.timeScale = 1;
            IsRewinding = false;
            EventStopRewinding?.Invoke();
            UxStopRewind();
            if (recordFrame) EventRecordFrame?.Invoke(); // used for interpolation
            if (stopFullRewind && _fullRewind)
            {
                EventStopFullRewind?.Invoke();
            }
            else
            {
                EventStopNormalRewind?.Invoke();
            }
        }
    }

    public void ResetCurrentRewind()
    {
        StopRewinding(false, false);
        _currentTime = _startRewindTime;
        EventResetRewinding?.Invoke();
    }

    private void Rewind()
    {
        _currentTime -= Time.fixedDeltaTime;
    }

    private void UxStartRewind(bool fullRewind = false)
    {
        RB_UXRewindManager.Instance.StartRewindTransition(fullRewind);
    }

    private void UxStopRewind()
    {
        RB_UXRewindManager.Instance.StopRewindTransition();
    }
}