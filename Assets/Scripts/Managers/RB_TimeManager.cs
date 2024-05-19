using UnityEngine;
using UnityEngine.Events;

public class RB_TimeManager : MonoBehaviour
{
    public static RB_TimeManager Instance;

    public UnityEvent EventRecordFrame;

    [SerializeField] private float _recordDelay = 0.1f;
    private float _timeWaited = 9999f; // used for delay, it's set at 9999 so it record the first frame

    private float _currentTime = 0; public float CurrentTime { get { return _currentTime; } }

    private bool _isRecording = true;
    private bool _isRewinding = false;

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
        RB_InputManager.Instance.EventRewindStarted.AddListener(StartRewinding); // TO TEST
        RB_InputManager.Instance.EventRewindCanceled.AddListener(StopRewinding); // TO TEST

        StartRecording();
    }

    private void FixedUpdate()
    {
        if (_isRecording && !_isRewinding) // doesn't record if rewinding
        {
            _currentTime += Time.fixedDeltaTime;
            _timeWaited += Time.fixedDeltaTime;
            if (_timeWaited >= _recordDelay) // record a frame when the time waited is superior to the delay
            {
                _timeWaited = 0;
                RecordFrame();
            }
        }
        else if (_isRewinding)
        {
            Rewind();
        }
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

    private void StartRewinding()
    {
        _isRewinding = true;
    }

    private void StopRewinding()
    {
        _isRewinding = false;
    }

    private void Rewind()
    {
        _currentTime -= Time.fixedDeltaTime;
    }
}
