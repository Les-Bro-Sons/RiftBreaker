using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RB_CollisionDetection : MonoBehaviour
{

    //Detection
    private List<GameObject> _detectedObjects = new();
    private List<GameObject> _detectedEnemies = new();

    //Events
    [HideInInspector] public UnityEvent EventOnObjectEntered;
    [HideInInspector] public UnityEvent EventOnObjectExit;
        //Enemy
    [HideInInspector] public UnityEvent EventOnEnemyEntered;
    [HideInInspector] public UnityEvent EventOnEnemyExit;

    private void OnTriggerEnter(Collider other)
    {
        //When an object is entering the trigger and has a life script add it to the list
        if(RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health enemyHealth))
        {
            _detectedEnemies.Add(enemyHealth.gameObject);
            EventOnEnemyEntered?.Invoke();
        }
        EventOnObjectEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        //When an object is exiting the trigger, if it's in the DetectedObjects list then remove it
        if (RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health enemyHealth))
        {
            if (_detectedEnemies.Contains(enemyHealth.gameObject))
            {
                _detectedEnemies.Remove(enemyHealth.gameObject);
                EventOnEnemyExit?.Invoke();
            }
        }
        EventOnObjectExit?.Invoke();


    }

    public List<GameObject> GetDetectedObjects()
    {
        //Getter to have the detected objects
        //Destroy all empty objects before getting the list
        DestroyDeletedObject();
        return _detectedEnemies;
    }

    public void DestroyDeletedObject()
    {
        foreach (GameObject detectedObject in _detectedEnemies.ToList())
        {
            if (detectedObject == null)
            {
                //If something in the list is empty then destroy it
                _detectedEnemies.Remove(detectedObject);
            }
        }
    }
}
