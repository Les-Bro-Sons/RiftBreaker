using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RB_DetectionZone))]
public class RB_DrawRoom : Editor
{
    //Properties
    private int _intervalIndex = 0;
    private bool _isDrawing = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RB_DetectionZone detectionZone = (RB_DetectionZone)target;


        EditorGUILayout.LabelField("");
        //Create buttons for the collider creations
        if (GUILayout.Button("AddCollider"))
        {
            detectionZone.UpdateCollider();
            detectionZone.ColliderPoints.Clear();
        }

        if (GUILayout.Button("Remove Points"))
        {
            detectionZone.ColliderPoints.Clear();
        }

        if (GUILayout.Button("Clear Mesh"))
        {
            detectionZone.ClearMesh();
            detectionZone.UpdateCollider();
            detectionZone.ClearMesh();
        }

        if (GUILayout.Button("Create Rooms"))
        {
            detectionZone.CreateRoom();
        }
    }

    private void SetPoint(RB_DetectionZone detectionZone)
    {
        //Get the position of the mouse on the object with the selected layer
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);

        foreach (var hit in hits)
        {
            if (detectionZone.DrawColliderLayer == (detectionZone.DrawColliderLayer | (1 << hit.collider.gameObject.layer)))
            {
                detectionZone.ColliderPoints.Add(hit.point);
                break;
            }
        }
    }

    void OnSceneGUI()
    {
        RB_DetectionZone detectionZone = (RB_DetectionZone)target;
        detectionZone.PointsInterval = Mathf.Clamp(detectionZone.PointsInterval, 1, int.MaxValue);

        Event e = Event.current;

        if (!detectionZone.ShouldDrawCollider)
            return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            _isDrawing = true;
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
            bool foundPoint = false;
            for(int i = 0; i < hits.Length; i++)
            {
                //If there's any point nearby the mouse click, remove it
                if (detectionZone.DrawColliderLayer == (detectionZone.DrawColliderLayer | (1 << hits[i].collider.gameObject.layer)) && (GetClosestPointWithinDistance(detectionZone.ColliderPoints, hits[i].point, 1) != null))
                {
                    detectionZone.ColliderPoints.Remove(GetClosestPointWithinDistance(detectionZone.ColliderPoints, hits[i].point, 1).Value);
                    foundPoint = true;
                    break;
                }
            }
            //Otherwise place a new one
            if(!foundPoint)
            {
                SetPoint(detectionZone);
            }

            e.Use();
        }

        if (e.type == EventType.MouseUp && e.button == 0)
        {
            _isDrawing = false;
            e.Use();
        }

        if (_isDrawing && (e.type == EventType.MouseDrag) && e.button == 0)
        {
            //Draw
            if (_intervalIndex % detectionZone.PointsInterval == 0)
            {
                SetPoint(detectionZone);
            }
            _intervalIndex++;
            e.Use();
        }

        Handles.color = Color.red;
        for (int i = 0; i < detectionZone.ColliderPoints.Count; i++)
        {
            //Create the gizmo with the points
            Handles.SphereHandleCap(0, detectionZone.ColliderPoints[i], Quaternion.identity, 0.1f, EventType.Repaint);
            if (i > 0)
                Handles.DrawLine(detectionZone.ColliderPoints[i - 1], detectionZone.ColliderPoints[i]);
        }
    }

    public Vector3? GetClosestPointWithinDistance(List<Vector3> points, Vector3 target, float distance)
    {
        //Get the closest point within the desired distance
        Vector3? closestPoint = null;
        float closestDistance = distance * distance;

        foreach (Vector3 point in points)
        {
            float currentDistance = (point - target).sqrMagnitude;
            if (currentDistance <= closestDistance)
            {
                closestDistance = currentDistance;
                closestPoint = point;
            }
        }

        return closestPoint;
    }
}
