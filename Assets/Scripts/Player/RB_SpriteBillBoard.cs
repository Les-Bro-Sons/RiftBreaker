using UnityEngine;

public class RB_SpriteBillBoard : MonoBehaviour
{
    //Components
    Transform _transform;

    //Rotation
    Quaternion _defaultRotation;

    private void Awake()
    {
        //Getting transform component
        _transform = transform;
    }

    private void Start()
    {
        //Getting default rotation
        _defaultRotation = _transform.rotation;
    }

    private void LookAtCamera()
    {
        _transform.rotation = _defaultRotation;
    }

    private void LateUpdate()
    {
        //Constantly looking at camera in y axe
        LookAtCamera();
    }
}
