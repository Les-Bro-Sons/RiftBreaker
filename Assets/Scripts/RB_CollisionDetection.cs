using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RB_CollisionDetection : MonoBehaviour
{

    //Detection
    [HideInInspector] public List<GameObject> DetectedObjects;

    //Events
    [HideInInspector] public UnityEvent EventOnObjectEntered;
    [HideInInspector] public UnityEvent EventOnObjectExit;

    private void OnTriggerEnter(Collider other)
    {
        //When an object is entering the trigger add it to the list
        print("objectDetected");
        DetectedObjects.Add(other.gameObject);
        EventOnObjectEntered?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        //When an object is exiting the trigger, if it's in the DetectedObjects list then remove it
        if (DetectedObjects.Contains(other.gameObject))
        {
            DetectedObjects.Remove(other.gameObject);
            EventOnObjectExit?.Invoke();
        }
    }
}
