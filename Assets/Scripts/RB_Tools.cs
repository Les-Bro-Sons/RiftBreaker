using UnityEngine;

public class RB_Tools
{
    public static bool TryGetComponentInParent<T>(GameObject selfObject, out T componentToGet) where T : Component
    {
        componentToGet = null;
        GameObject currentObject = selfObject;
        int maxIter = 20;
        int currentIter = 0;
        while (componentToGet == null && currentObject.transform.parent != null)
        {
            //Trying to get component on current object
            if(currentObject.TryGetComponent<T>(out componentToGet))
            {
                Debug.Log("component found on " + currentObject.name);
                return true;
            }
            //If not found then trying on its parent
            if (currentObject.transform.parent.gameObject != null)
            {
                currentObject = currentObject.transform.parent.gameObject;
            }
            else
                return false;
            

            //Security max iteration to not crash
            currentIter++;
            if (currentIter >= maxIter)
            {
                Debug.Log("something went wrong");
                return false;
            }
        }

        //If we exited loop then check on last object
        if (componentToGet == null)
        {
            if (currentObject.TryGetComponent<T>(out componentToGet))
            {
                Debug.Log("Component found on " + currentObject.name);
                return true;
            }
            else
            {
                Debug.Log("Component not found");
                return false;
            }
        }

        return false;
    }
}
