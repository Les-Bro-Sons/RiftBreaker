using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RB_CollisionDetection : MonoBehaviour
{

    //Detection
    private List<GameObject> _detectedObjects = new();

    //Events
    [HideInInspector] public UnityEvent EventOnObjectEntered;
    [HideInInspector] public UnityEvent EventOnObjectExit;

    private void OnTriggerEnter(Collider other)
    {
        //When an object is entering the trigger and has a life script add it to the list
        if(RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health enemyHealth))
        {
            _detectedObjects.Add(enemyHealth.gameObject);
        }
        EventOnObjectEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        //When an object is exiting the trigger, if it's in the DetectedObjects list then remove it
        if (RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health enemyHealth))
        {
            if (_detectedObjects.Contains(enemyHealth.gameObject))
            {
                _detectedObjects.Remove(enemyHealth.gameObject);
                EventOnObjectExit?.Invoke();
            }
        }
        
    }

    public List<GameObject> GetDetectedObjects()
    {
        //Getter to have the detected objects
        //Destroy all empty objects before getting the list
        DestroyDeletedObject();
        return _detectedObjects;
    }

    public void DestroyDeletedObject()
    {
        foreach (GameObject detectedObject in _detectedObjects.ToList())
        {
            if (detectedObject == null)
            {
                //If something in the list is empty then destroy it
                _detectedObjects.Remove(detectedObject);
            }
        }
    }
}
