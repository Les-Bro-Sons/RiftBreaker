using UnityEngine;
using UnityEngine.Events;

public class RB_Door : MonoBehaviour
{
    private Animator _doorAnimator;
    public bool IsControledByRoom = true;

    public UnityEvent EventOpenDoor;
    public UnityEvent EventCloseDoor;

    private RB_TimeBodyRecorder _timeRecorder;

    private void Awake()
    {
        _doorAnimator = GetComponent<Animator>();
        _timeRecorder = GetComponent<RB_TimeBodyRecorder>();
    }

    public void Open()
    {
        _doorAnimator.SetTrigger("Down");
        EventOpenDoor?.Invoke();

        EventInTime openDoor = new EventInTime();
        openDoor.TypeEvent = TYPETIMEEVENT.OpenDoor;
        if (_timeRecorder) _timeRecorder.RecordTimeEvent(openDoor);
    }

    public void Close()
    {
        _doorAnimator.SetTrigger("Up");
        EventCloseDoor?.Invoke();

        EventInTime closeDoor = new EventInTime();
        closeDoor.TypeEvent = TYPETIMEEVENT.CloseDoor;
        if (_timeRecorder) _timeRecorder.RecordTimeEvent(closeDoor);
    }
}
