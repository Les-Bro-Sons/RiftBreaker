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
        //When an object is entering the trigger add it to the list
        _detectedObjects.Add(other.transform.root.gameObject);
        EventOnObjectEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        //When an object is exiting the trigger, if it's in the DetectedObjects list then remove it
        if (_detectedObjects.Contains(other.transform.root.gameObject))
        {
            _detectedObjects.Remove(other.transform.root.gameObject);
            EventOnObjectExit?.Invoke();
        }
    }

    public List<GameObject> GetDetectedObjects()
    {
        DestroyDeletedObject();
        return _detectedObjects;
    }

    public void DestroyDeletedObject()
    {
        foreach (GameObject detectedObject in _detectedObjects.ToList())
        {
            if (detectedObject == null)
            {
                _detectedObjects.Remove(detectedObject);
            }
        }
    }
}
