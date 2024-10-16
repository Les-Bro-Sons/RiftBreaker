using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RB_CollisionDetection : MonoBehaviour
{

    //Detection
    private List<GameObject> _detectedObjects = new();
    private List<GameObject> _detectedEnemies = new();
    private List<GameObject> _detectedEntity = new();
    private bool _isPlayerIn = false;

    //Events
    [HideInInspector] public UnityEvent EventOnObjectEntered;
    [HideInInspector] public UnityEvent EventOnObjectExit;
    [HideInInspector] public UnityEvent EventOnPlayerEntered;
    [HideInInspector] public UnityEvent EventOnPlayerExit;

    [HideInInspector] public UnityEvent EventOnEntityEntered;
    [HideInInspector] public UnityEvent EventOnEntityExit;
        //Enemy
    [HideInInspector] public UnityEvent EventOnEnemyEntered;
    [HideInInspector] public UnityEvent EventOnEnemyExit;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        //When an object is entering the trigger and has a life script add it to the list
        if(RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health enemyHealth))
        {
            if(enemyHealth.Team == TEAMS.Ai)
            {
                _detectedEnemies.Add(enemyHealth.gameObject);
                EventOnEnemyEntered?.Invoke();
            }
            else if (RB_Tools.TryGetComponentInParent(other.gameObject, out RB_PlayerAction playerAction)) //When the player enter
            {
                _isPlayerIn = true;
                EventOnPlayerEntered?.Invoke();
            }
            _detectedEntity.Add(enemyHealth.gameObject);
            EventOnEntityEntered?.Invoke();
        }
        _detectedObjects.Add(other.gameObject);
        EventOnObjectEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        //When an object is exiting the trigger, if it's in the DetectedObjects list then remove it
        if (RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health enemyHealth))
        {
            if(RB_Tools.TryGetComponentInParent(other.gameObject, out RB_PlayerAction playerAction)) //When the player exit
            {
                _isPlayerIn = false;
                EventOnPlayerExit?.Invoke();
            }
            if (_detectedEnemies.Contains(enemyHealth.gameObject))
            {
                _detectedEnemies.Remove(enemyHealth.gameObject);
                EventOnEnemyExit?.Invoke();
            }
            if (_detectedEntity.Contains(enemyHealth.gameObject))
            {
                _detectedEntity.Remove(enemyHealth.gameObject);
                EventOnEntityExit?.Invoke();
            }
        }
        _detectedObjects.Remove(other.gameObject);
        EventOnObjectExit?.Invoke();


    }

    public List<GameObject> GetDetectedEnnemies()
    {
        //Getter to have the detected enemies
        DestroyDeletedObject();
        return _detectedEnemies;
    }

    public List<GameObject> GetDetectedObjects()
    {
        //Getter to have the detected objects
        DestroyDeletedObject();
        return _detectedObjects;
    }

    public List<GameObject> GetDetectedEntity()
    {
        DestroyDeletedObject();
        return _detectedEntity;
    }
    public bool IsPlayerIn()
    {
        //Getter to have if the player is in
        return _isPlayerIn;
    }

    public void DestroyDeletedObject()
    {
        foreach (GameObject detectedEnemy in _detectedEnemies.ToList())
        {
            if (detectedEnemy == null)
            {
                //If something in the list is empty then destroy it
                _detectedEnemies.Remove(detectedEnemy);
            }
        }
        foreach (GameObject detectedObject in _detectedObjects.ToList())
        {
            if (detectedObject == null)
            {
                //If something in the list is empty then destroy it
                _detectedEnemies.Remove(detectedObject);
            }
        }
        foreach (GameObject detectedObject in _detectedEntity.ToList())
        {
            if (detectedObject == null)
            {
                //If something in the list is empty then destroy it
                _detectedEntity.Remove(detectedObject);
            }
        }
    }
}
