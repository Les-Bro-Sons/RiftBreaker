using UnityEngine;

public class RB_SeeThroughWalls : MonoBehaviour
{
    private new Transform transform;

    public static int PosID = Shader.PropertyToID("_PlayerPosition");
    public static int SizeID = Shader.PropertyToID("_Size");

    [SerializeField] private Material _wallMaterial;
    private Camera _camera;
    [SerializeField] private LayerMask _mask;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _camera = RB_Camera.Instance.GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 dir = _camera.transform.position - transform.position;

        /*Debug.DrawRay(transform.position, dir.normalized * 10, Color.magenta);
        if (Physics.Raycast(transform.position, dir.normalized, 10, (1 << 3)))
        {
            _wallMaterial.SetFloat(SizeID, 1);
        }
        else
        {
            _wallMaterial.SetFloat(SizeID, 0);
        }*/


        Vector3 view = _camera.WorldToViewportPoint(transform.position);
        _wallMaterial.SetVector(PosID, view);
    }
}
