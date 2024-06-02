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
        _defaultRotation = Quaternion.LookRotation(Vector3.forward);
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
