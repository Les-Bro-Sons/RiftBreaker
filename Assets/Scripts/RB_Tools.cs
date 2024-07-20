using UnityEngine;

public class RB_Tools
{
    public static Vector3 GetRandomVector(float min, float max, bool x = true, bool y = true, bool z = true)
    {
        return new Vector3((x)? Random.Range(min, max) : 0, (y)? Random.Range(min, max) : 0, (z)? Random.Range(min, max) : 0);
    }

    public static bool TryGetComponentInParent<T>(Transform selfObject, out T componentToGet) where T : Component
    {
        return TryGetComponentInParent<T>(selfObject.gameObject, out componentToGet);
    }

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
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public static Vector3 GetHorizontalDirection(Vector3 posA, Vector3 posB)
    {
        Vector3 knockbackDir = posA - posB;
        knockbackDir = new Vector3(knockbackDir.x, 0, knockbackDir.z);
        return knockbackDir.normalized;
    }
    public static Vector3 GetHorizontalDirection(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z).normalized;
    }

    public static float GetPlayerDistance(Vector3 position)
    {
        return Vector3.Distance(position, RB_PlayerController.Instance.transform.position);
    }
}
