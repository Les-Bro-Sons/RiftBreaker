using Cinemachine;
using UnityEngine;

public class RB_Camera : MonoBehaviour
{
    public static RB_Camera Instance;

    //Components
    private Transform _transform;
    public CinemachineVirtualCamera VirtualCam;
    private CinemachineRecomposer _recomposer;

    private float _currentZoomValue = 1;
    private float _currentZoomForce = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _transform = transform;
        VirtualCam = GetComponentInChildren<CinemachineVirtualCamera>();
        _recomposer = VirtualCam.GetComponent<CinemachineRecomposer>();
    }

    private void Start()
    {
        VirtualCam.Follow = RB_PlayerController.Instance.transform;
    }

    private void Update() 
    { 
        ApplyZoom();
    }

    public void Zoom(float zoomValue, float zoomForce = 4)
    {
        _currentZoomValue = zoomValue;
        _currentZoomForce = zoomForce;
    }

    private void ApplyZoom()
    {
        _recomposer.m_ZoomScale = Mathf.Lerp(_recomposer.m_ZoomScale, _currentZoomValue, _currentZoomForce * Time.deltaTime);
    }
}
