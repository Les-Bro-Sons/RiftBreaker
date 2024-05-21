using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RB_TimeBodyRewind : MonoBehaviour
{
    [SerializeField] private float _recordTime = 5f;
    [SerializeField] private ENTITYTYPES _entityType = ENTITYTYPES.Ai;
    private bool _isRewinding = false;

    private Rigidbody _rb;
    public  List<RB_PointInTime> PointsInTime = new();

    int _pointCountFrame = 0;

    RB_UXRewindManager _uxRewind;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RB_InputManager.Instance.EventRewindStarted.AddListener(StartRewind); //POUR TESTER
        RB_InputManager.Instance.EventRewindCanceled.AddListener(StopRewind); //POUR TESTER
    }

    private void Update()
    {
        Debug.Log($"_pointCountFrame : {_pointCountFrame}");

        // a mettre dans le player controller

        if (Input.GetKeyDown(KeyCode.R))
        {
            print("R");
            StartRewind();
        }

        if (Input.GetKeyUp(KeyCode.R))
        { 
            StopRewind();
        }
    }

    private void FixedUpdate()
    {
        if (_isRewinding)
            Rewind();
        else
            Record();
    }

    private void StartRewind()
    {
        _isRewinding = true;
        _rb.isKinematic = true;
        
        //UxStartRewind();
        // Play SFX
    }

    private void StopRewind()
    {
        _isRewinding = false;
        _rb.isKinematic = false;

        //UxStopRewind();
        // Play SFX
    }

    private void Rewind()
    {

        switch (_entityType)
        {
            case ENTITYTYPES.Ai:
                break;
            
            case ENTITYTYPES.Player:
                if (PointsInTime.Count > 0)
                {
                    RB_PointInTime pit = PointsInTime[0];
                    transform.position = pit.Position;
                    transform.rotation = pit.Rotation;
                    PointsInTime.RemoveAt(0);
                    _pointCountFrame--;
                }
                break;
                
            default:
                StopRewind();
                break;
        }
    }

    private void Record()
    {
        // Si il y a deja "recordTime" points enregistres alors on enleve le plus ancien
        if (PointsInTime.Count > Mathf.Round(_recordTime / Time.fixedDeltaTime))
            PointsInTime.RemoveAt(PointsInTime.Count - 1);

        // On ajoute un nouveau point
        PointsInTime.Insert(0, new RB_PointInTime(transform.position, transform.rotation));
        _pointCountFrame++;
    }





    private void UxStartRewind()
    {
        _uxRewind?.StartRewindTransition();
    }

    private void UxStopRewind()
    {
        _uxRewind?.StopRewindTransition();
    }
}