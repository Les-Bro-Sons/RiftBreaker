using Cinemachine;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RB_3DParallax : MonoBehaviour
{
    private new Transform transform;
    private Vector3 _baseOffset;
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _material;

    [SerializeField] private float _parallax = 1;

    private void Start()
    {
        transform = GetComponent<Transform>();
        _baseOffset = transform.position;
        _material = _meshRenderer.material;
        _material = new Material(_material);
        _meshRenderer.material = _material;
    }

    private void Update()
    {
        CinemachineVirtualCamera virtualCamera = RB_Camera.Instance.GetComponentInChildren<CinemachineVirtualCamera>();
        Vector3 cameraPos = -virtualCamera.State.CorrectedPosition;
        cameraPos = new Vector3(cameraPos.x, 0, cameraPos.z);
        _material.SetVector("_Offset", cameraPos/_parallax);
    }
}
