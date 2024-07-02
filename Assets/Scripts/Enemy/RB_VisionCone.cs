using Unity.VisualScripting;
using UnityEngine;

public class RB_VisionCone : MonoBehaviour
{
    public Material VisionConeMaterial;
    public float VisionRange;
    public float VisionAngle;
    public LayerMask VisionObstructingLayer; // Layer with objects that obstruct the enemy view, like walls
    public int VisionConeResolution = 120; // Number of triangles in the vision cone for visual quality

    private Mesh VisionConeMesh;
    private MeshFilter MeshFilter_;

    private Transform _transform;
    private Transform _playerTransform;
    private RB_Health _health;
    [SerializeField] private float _distanceRequiredToDraw = 25;
    private bool _isInReach = false;
    private float _baseAlpha;

    /// <summary>
    /// Initializes the vision cone components and sets up initial values.
    /// </summary>
    private void Start()
    {
        _transform = transform;
        RB_Tools.TryGetComponentInParent<RB_Health>(gameObject, out _health);
        if (RB_Tools.TryGetComponentInParent<RB_AI_BTTree>(_transform, out RB_AI_BTTree btTree))
        {
            VisionAngle = btTree.FovAngle;
            VisionRange = btTree.FovRange + 0.75f;
        }

        VisionConeMaterial = new Material(VisionConeMaterial);
        _baseAlpha = VisionConeMaterial.GetFloat("_BaseAlpha");

        _transform.AddComponent<MeshRenderer>().material = VisionConeMaterial;
        MeshFilter_ = _transform.AddComponent<MeshFilter>();
        VisionConeMesh = new Mesh();
        VisionAngle *= Mathf.Deg2Rad;
        _playerTransform = RB_PlayerController.Instance.transform;
        CheckDistance();
    }

    /// <summary>
    /// Updates the vision cone every frame if the player is within reach.
    /// </summary>
    private void Update()
    {
        if (_isInReach)
        {
            DrawVisionCone();
        }
        else if (RB_LevelManager.Instance.CurrentPhase != PHASES.Infiltration)
        {
            VisionConeMaterial.SetFloat("_BaseAlpha", 0);
        }
    }

    /// <summary>
    /// Checks the distance between the player and the enemy to determine if the vision cone should be drawn.
    /// </summary>
    private void CheckDistance()
    {
        _isInReach = Vector3.Distance(_playerTransform.position, _transform.position) < _distanceRequiredToDraw;
        Invoke("CheckDistance", 1);
    }

    /// <summary>
    /// Draws the vision cone mesh based on the enemy's vision parameters.
    /// </summary>
    private void DrawVisionCone()
    {
        if (RB_LevelManager.Instance.CurrentPhase == PHASES.Infiltration && !_health.Dead)
        {
            VisionConeMaterial.SetFloat("_BaseAlpha", Mathf.Lerp(VisionConeMaterial.GetFloat("_BaseAlpha"), _baseAlpha, 4 * Time.deltaTime));

            int[] triangles = new int[(VisionConeResolution - 1) * 3];
            Vector3[] vertices = new Vector3[VisionConeResolution + 1];
            vertices[0] = Vector3.zero;
            float currentAngle = -VisionAngle / 2;
            float angleIncrement = VisionAngle / (VisionConeResolution - 1);

            for (int i = 0; i < VisionConeResolution; i++)
            {
                float sine = Mathf.Sin(currentAngle);
                float cosine = Mathf.Cos(currentAngle);
                Vector3 raycastDirection = (_transform.forward * cosine) + (_transform.right * sine);
                Vector3 vertForward = (Vector3.forward * cosine) + (Vector3.right * sine);

                if (Physics.Raycast(_transform.position, raycastDirection, out RaycastHit hit, VisionRange, VisionObstructingLayer))
                {
                    vertices[i + 1] = vertForward * hit.distance;
                }
                else
                {
                    vertices[i + 1] = vertForward * VisionRange;
                }

                currentAngle += angleIncrement;
            }

            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                triangles[i] = 0;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j + 2;
            }

            VisionConeMesh.Clear();
            VisionConeMesh.vertices = vertices;
            VisionConeMesh.triangles = triangles;
            MeshFilter_.mesh = VisionConeMesh;
        }
        else
        {
            VisionConeMaterial.SetFloat("_BaseAlpha", Mathf.Lerp(VisionConeMaterial.GetFloat("_BaseAlpha"), 0, 4 * Time.deltaTime));
        }
    }
}
