using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Rendering;

public class RB_RoomCreator : MonoBehaviour
{
    // Properties section
    [Header("Properties")]
    public bool IsCollidersTrigger = true; // Whether colliders should act as triggers
    public int PointsInterval; // Interval for points, not used in current code
    public bool ShouldDrawCollider; // Flag to determine if the collider should be drawn
    public bool ShouldDrawRenderer; // Flag to determine if the renderer should be drawn
    public float ColliderHeight; // Height of the collider
    public GameObject objectToDrawOn; // Object to draw on
    public Material ColliderMaterial; // Material for the collider's visual representation
    [HideInInspector] public bool ShowList; // Hidden property to control showing the list in the inspector

    // Mesh creation section
    [Header("Mesh Creation")]
    public List<Vector3> ColliderPoints = new List<Vector3>(); // List of points defining the collider
    private List<GameObject> _meshObjects = new List<GameObject>(); // List of mesh objects created

    //Room section
    [Header("Rooms")]
    [SerializeField] private RB_RoomManager _roomManager;
    private MeshCollider _currentCollider;
    private GameObject _currentColliderObject;
    private GameObject _currentMeshObject;

    // Updates the collider mesh based on the defined points
    public void UpdateCollider()
    {
        if (ColliderPoints.Count > 2)
        {
            // Calculate the center of the base
            Vector3 centerBase = CalculateCentroid(ColliderPoints);

            // Calculate the normal of the plane
            Vector3 normal = CalculateNormal(ColliderPoints);

            // Calculate the top center by translating along the inverted normal
            Vector3 centerTop = centerBase - normal * ColliderHeight;
            CreateMeshColliders(centerBase, centerTop, normal);
        }
    }

    public void CreateRoom()
    {
        int maxIteration = transform.childCount + 100;
        while (transform.childCount != 0)
        {
            foreach (Transform room in transform)
            {
                room.parent = _roomManager.transform;
            }
            maxIteration--;
            if (maxIteration <= 0)
                break;
        }
        _meshObjects.Clear();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    // Creates mesh colliders based on the given points and normal
    private GameObject CreateMeshColliders(Vector3 centerBase, Vector3 centerTop, Vector3 normal)
    {
        GameObject meshObject = new GameObject("Room");
        meshObject.AddComponent<RB_Room>();
        GameObject colliders = new GameObject("Colliders");
        colliders.layer = LayerMask.NameToLayer("Room");
        meshObject.transform.parent = transform;
        colliders.transform.parent = meshObject.transform;

        for (int i = 0; i < ColliderPoints.Count; i++)
        {
            GameObject childObject = new GameObject("MeshCollider_" + i);

            childObject.layer = LayerMask.NameToLayer("Room");
            childObject.AddComponent<RB_EntityDetector>();
            childObject.transform.parent = colliders.transform;
            childObject.transform.localPosition = Vector3.zero;

            MeshCollider meshCollider = childObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.isTrigger = IsCollidersTrigger;

            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[6];
            vertices[0] = ColliderPoints[i];
            vertices[1] = ColliderPoints[(i + 1) % ColliderPoints.Count];
            vertices[2] = centerBase;
            vertices[3] = ColliderPoints[i] - normal * ColliderHeight;
            vertices[4] = ColliderPoints[(i + 1) % ColliderPoints.Count] - normal * ColliderHeight;
            vertices[5] = centerTop;

            int[] triangles = new int[]
            {
                0, 1, 2, // base triangle
                3, 4, 5, // top triangle
                0, 3, 4, 0, 4, 1, // side triangles
                1, 4, 5, 1, 5, 2 // closing shape
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.name = "CustomCollider";

            meshCollider.sharedMesh = mesh;
            _meshObjects.Add(childObject);
        }

        return meshObject;
    }

    // Creates mesh colliders based on the given points and normal
    private MeshCollider CreateMeshColliders(Vector3 centerBase, Vector3 centerTop, Vector3 normal)
    {
        if(_currentMeshObject == null)
        {
            _currentMeshObject = new GameObject("Room");
            _currentMeshObject.AddComponent<RB_Room>();

            _currentMeshObject.transform.parent = transform;
        }

        if(_currentColliderObject == null)
        {
            _currentColliderObject = new GameObject("Colliders");
            _currentColliderObject.transform.parent = _currentMeshObject.transform;
            _currentColliderObject.AddComponent<RB_EntityDetector>();
            _currentColliderObject.layer = LayerMask.NameToLayer("Room");
        }

        MeshCollider meshCollider = _currentColliderObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = IsCollidersTrigger;
        Mesh mesh = new();
        
        Vector3[] vertices = new Vector3[(ColliderPoints.Count + 1) * 2];
        int[] faces = new int[ColliderPoints.Count * 12];

        //Generate top and base faces
        int verticesOffset = ColliderPoints.Count+1;
        vertices[0] = centerBase;
        vertices[verticesOffset] = centerBase+normal*ColliderHeight;
        for(int i = 1; i <= ColliderPoints.Count; i++)
        {
            vertices[i] = ColliderPoints[i-1];
            vertices[i + verticesOffset] = vertices[i] + normal * ColliderHeight;
        }
        int faceArrayIndex = 0;
        int faceOffset = ColliderPoints.Count * 3;
        int n;
        for (int i = 1; i <= ColliderPoints.Count; i++)
        {
            n = (i == ColliderPoints.Count) ? 1 : i + 1;
            //BaseFace

            faceArrayIndex = i * 3;
            faces[faceArrayIndex] = 0;
            faces[faceArrayIndex + 1] = i;
            faces[faceArrayIndex + 2] = n;

            //TopFace
            faces[faceArrayIndex + faceOffset] = verticesOffset;
            faces[faceArrayIndex + faceOffset + 1] = i + verticesOffset;
            faces[faceArrayIndex + faceOffset + 2] = n + verticesOffset;
        }

        int sideOffset = ColliderPoints.Count * 6;
        print(sideOffset);

        for (int i = 1; i <= ColliderPoints.Count; i++)
        {
            n = (i == ColliderPoints.Count) ? 1 : i + 1;
            faceArrayIndex = sideOffset;
            faces[faceArrayIndex] = i;
            faces[faceArrayIndex + 1] = n;
            faces[faceArrayIndex + 2] = i + verticesOffset;

            print(faceArrayIndex + sideOffset);
            
            faces[faceArrayIndex] = i + verticesOffset;
            faces[faceArrayIndex + 1] = n + verticesOffset;
            faces[faceArrayIndex + 2] = n;
            sideOffset += 6;
        }

        mesh.vertices = vertices;
        mesh.triangles = faces;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.name = "CustomCollider";

        meshCollider.sharedMesh = mesh;

        EditorUtility.SetDirty(meshCollider);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        return meshCollider;
    }

    // Creates a visual representation of the collider mesh
    private void CreateMeshVisual(Vector3 centerBase, Vector3 centerTop, Vector3 normal, Transform parent)
    {
        GameObject visuals = new GameObject("Visuals");
        visuals.transform.parent = parent;
        if (ShouldDrawRenderer)
        {
            if (ColliderMaterial == null || ColliderPoints.Count < 3) return;

            GameObject visualObject = new GameObject("VisualMesh");
            visualObject.transform.parent = visuals.transform;
            visualObject.transform.localPosition = Vector3.zero;

            MeshFilter meshFilter = visualObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = visualObject.AddComponent<MeshRenderer>();
            meshRenderer.material = ColliderMaterial;

            Mesh visualMesh = new Mesh();

            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();

            for (int i = 0; i < ColliderPoints.Count; i++)
            {
                verticesList.Add(ColliderPoints[i]);
                verticesList.Add(ColliderPoints[i] - normal * ColliderHeight);
            }
            verticesList.Add(centerBase);
            verticesList.Add(centerTop);

            for (int i = 0; i < ColliderPoints.Count; i++)
            {
                int next = (i + 1) % ColliderPoints.Count;
                trianglesList.Add(i * 2);
                trianglesList.Add(next * 2);
                trianglesList.Add(verticesList.Count - 2);
            }

            for (int i = 0; i < ColliderPoints.Count; i++)
            {
                int next = (i + 1) % ColliderPoints.Count;
                trianglesList.Add(i * 2 + 1);
                trianglesList.Add(verticesList.Count - 1);
                trianglesList.Add(next * 2 + 1);
            }

            for (int i = 0; i < ColliderPoints.Count; i++)
            {
                int next = (i + 1) % ColliderPoints.Count;

                trianglesList.Add(i * 2);
                trianglesList.Add(i * 2 + 1);
                trianglesList.Add(next * 2);

                trianglesList.Add(next * 2);
                trianglesList.Add(i * 2 + 1);
                trianglesList.Add(next * 2 + 1);
            }

            visualMesh.vertices = verticesList.ToArray();
            visualMesh.triangles = trianglesList.ToArray();
            visualMesh.RecalculateNormals();
            visualMesh.RecalculateBounds();
            visualMesh.name = "VisualCollider";

            meshFilter.mesh = visualMesh;
        }

    }

    // Calculates the centroid of a given set of points
    Vector3 CalculateCentroid(List<Vector3> points)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 point in points)
        {
            sum += point;
        }
        return sum / points.Count;
    }

    // Calculates the normal vector of the plane defined by the first three points
    public static Vector3 CalculateNormal(List<Vector3> points)
    {
        Vector3 v1 = points[1] - points[0];
        Vector3 v2 = points[2] - points[0];
        Vector3 normal = Vector3.Cross(v1, v2).normalized;

        // Ensure the normal is oriented towards Vector3.up
        if (Vector3.Dot(normal, Vector3.up) > 0)
        {
            normal = -normal;
        }

        return normal;
    }

    // Clears the mesh objects and children from the transform
    public void ClearMesh()
    {
        while (transform.childCount != 0)
        {
            foreach (GameObject mesh in _meshObjects.ToList())
            {
                DestroyImmediate(mesh);
            }
            foreach (Transform colliders in transform)
            {
                DestroyImmediate(colliders.gameObject);
            }
        }
        _meshObjects.Clear();
    }

    // Function to calculate the area of a triangle given its vertices
    float TriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return 0.5f * ((p2.x - p1.x) * (p3.y - p1.y) - (p3.x - p1.x) * (p2.y - p1.y));
    }

    // Draws gizmos in the editor for visualization purposes
    void OnDrawGizmos()
    {
        if (!ShouldDrawCollider)
            return;

        Gizmos.color = Color.red;
        if (ColliderPoints.Count > 1)
        {
            for (int i = 0; i < ColliderPoints.Count; i++)
            {
                if (i > 0)
                    Gizmos.DrawLine(ColliderPoints[i - 1], ColliderPoints[i]);
            }
            Gizmos.DrawLine(ColliderPoints[ColliderPoints.Count - 1], ColliderPoints[0]);
        }
    }
}
