using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class RB_TimeBodyRewind : MonoBehaviour
{
    [SerializeField] private float _recordTime = 5f;
    [SerializeField] private ENTITYTYPES _entityType = ENTITYTYPES.Ai;
    private bool _isRewinding = false;

    private Rigidbody _rb;
    public  List<RB_PointInTime> _pointsInTime = new();


    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        // a mettre dans le player controller

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRewind();
        }

        if (Input.GetKeyUp(KeyCode.R))
        { 
            Debug.Log("Relache");
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
        // Play SFX
    }
    
    private void StopRewind()
    {
        _isRewinding = false;
        _rb.isKinematic = false;
        // Play SFX
    }

    private void Rewind()
    {

        switch (_entityType)
        {
            case ENTITYTYPES.Ai:
                break;
            
            case ENTITYTYPES.Player:
                if (_pointsInTime.Count > 0)
                {
                    RB_PointInTime pit = _pointsInTime[0];
                    transform.position = pit.Position;
                    transform.rotation = pit.Rotation;
                    _pointsInTime.RemoveAt(0);
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
        if (_pointsInTime.Count > Mathf.Round(_recordTime / Time.fixedDeltaTime))
            _pointsInTime.RemoveAt(_pointsInTime.Count - 1);

        // On ajoute un nouveau point
        _pointsInTime.Insert(0, new RB_PointInTime(transform.position, transform.rotation));
    }
}