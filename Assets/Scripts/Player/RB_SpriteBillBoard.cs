using UnityEngine;

public class RB_SpriteBillBoard : MonoBehaviour
{
    [SerializeField] private bool _billboardXAxis = false;

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
        if (_billboardXAxis)
        {
            _transform.eulerAngles += Vector3.right * RB_Camera.Instance.transform.eulerAngles.x;
        }
    }

    private void LateUpdate()
    {
        //Constantly looking at camera in y axe
        LookAtCamera();
    }
}
