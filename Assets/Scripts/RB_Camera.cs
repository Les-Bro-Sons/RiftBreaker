using UnityEngine;

public class RB_Camera : MonoBehaviour
{
    //Components
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }
}
