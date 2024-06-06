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


    [SerializeField] private float _recordDelay = 0.1f;
    public float DurationRewind = 5f;
    private float _startedRewind = 0f;
    private float _timeWaited = 9999f; // used for delay, it's set at 9999 so it record the first frame

    private float _currentTime = 0; public float CurrentTime { get { return _currentTime; } }

    private bool _isRecording = true;
    public bool IsRewinding = false;
    private bool _fullRewind = false;

    [Header("Hourglass")]
    public int NumberOfRewind = 3;
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

        RB_UxHourglass.Instance.CreateMaxNumberOfHourglass();
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
                StopRewinding(true);
                return;
            }
            Rewind();
            if (_fullRewind)
            {
                Time.timeScale += Time.fixedDeltaTime / 10f;
            }
        }
    }

    public float GetRewindLastingTime()
    {
        return Mathf.Abs(_startedRewind - Time.time);
    }

    public float GetRewindRemainingTime()
    {
        return DurationRewind - Mathf.Abs(_startedRewind - Time.time);
    }

    public float GetRewindRemainingTimeInSecond()
    {
        return (DurationRewind - Mathf.Abs(_startedRewind - Time.time)) ;
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

        if (NumberOfRewind > 0 || skipChecks)
        { 
            _startedRewind = Time.time;
            EventRecordFrame?.Invoke(); // used for interpolation
            IsRewinding = true;
            _fullRewind = fullRewind;
            UxStartRewind(fullRewind);
            RB_AudioManager.Instance.MusicSource.pitch = -1;
            EventStartRewinding?.Invoke();
        }
        else
        {
            Debug.LogWarning("Aucun sablier dans la liste !");
        }
    }

    public void StopRewinding(bool stopFullRewind = false)
    {
        if (!IsRewinding) return;

        if (IsRewinding && (!_fullRewind || stopFullRewind))
        {
            Time.timeScale = 1;
            EventStopRewinding?.Invoke();
            IsRewinding = false;
            UxStopRewind();
            if (!stopFullRewind)
            {
                NumberOfRewind -= 1;
            }
            else
            {
                NumberOfRewind = 3;
                RB_UxHourglass.Instance.CreateMaxNumberOfHourglass();
            }
            // RB_AudioManager.Instance.MusicSource.pitch = 1;
            EventRecordFrame?.Invoke(); // used for interpolation
        }
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