using Unity.VisualScripting;
using UnityEngine;

public class RB_VisionCone : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer;//layer with objects that obstruct the enemy view, like walls, for example
    public int VisionConeResolution = 120;//the vision cone will be made up of triangles, the higher this value is the pretier the vision cone will be
    Mesh VisionConeMesh;
    MeshFilter MeshFilter_;
    //Create all of these variables, most of them are self explanatory, but for the ones that aren't i've added a comment to clue you in on what they do
    //for the ones that you dont understand dont worry, just follow along

    private Transform _transform;
    private Transform _playerTransform;
    [SerializeField] private float _distanceRequiredToDraw = 30;
    private bool _isInReach = false;
    void Start()
    {
        _transform = transform;
        if (RB_Tools.TryGetComponentInParent<RB_AI_BTTree>(_transform, out RB_AI_BTTree btTree))
        {
            VisionAngle = btTree.FovAngle;
            VisionRange = btTree.FovRange;

            _transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
            MeshFilter_ = _transform.AddComponent<MeshFilter>();
            VisionConeMesh = new Mesh();
            VisionAngle *= Mathf.Deg2Rad;
            _playerTransform = RB_PlayerController.Instance.transform;
            CheckDistance();
        }
    }


    void Update()
    {
        if (_isInReach)
        {
            DrawVisionCone();//calling the vision cone function everyframe just so the cone is updated every frame
        }
        
    }

    private void CheckDistance()
    {
        _isInReach = Vector3.Distance(_playerTransform.position, _transform.position) < _distanceRequiredToDraw;
        Invoke("CheckDistance", 1);
    }

    void DrawVisionCone()//this method creates the vision cone mesh
    {
        
        int[] triangles = new int[(VisionConeResolution - 1) * 3];
        Vector3[] Vertices = new Vector3[VisionConeResolution + 1];
        Vertices[0] = Vector3.zero;
        float Currentangle = -VisionAngle / 2;
        float angleIcrement = VisionAngle / (VisionConeResolution - 1);
        float Sine;
        float Cosine;

        for (int i = 0; i < VisionConeResolution; i++)
        {
            Sine = Mathf.Sin(Currentangle);
            Cosine = Mathf.Cos(Currentangle);
            Vector3 RaycastDirection = (transform.forward * Cosine) + (transform.right * Sine);
            Vector3 VertForward = (Vector3.forward * Cosine) + (Vector3.right * Sine);
            if (Physics.Raycast(transform.position, RaycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
            {
                Vertices[i + 1] = VertForward * hit.distance;
            }
            else
            {
                Vertices[i + 1] = VertForward * VisionRange;
            }


            Currentangle += angleIcrement;
        }
        for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;
        }
        VisionConeMesh.Clear();
        VisionConeMesh.vertices = Vertices;
        VisionConeMesh.triangles = triangles;
        MeshFilter_.mesh = VisionConeMesh;
    }


}