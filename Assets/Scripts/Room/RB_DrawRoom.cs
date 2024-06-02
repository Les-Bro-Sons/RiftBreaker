using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
[CustomEditor(typeof(RB_RoomCreator))]
public class RB_DrawRoom : Editor
{
    //Properties
    private int _intervalIndex = 0;
    private bool _isDrawing = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RB_RoomCreator roomCreator = (RB_RoomCreator)target;


        EditorGUILayout.LabelField("");
        //Create buttons for the collider creations
        if (GUILayout.Button("AddCollider"))
        {
            roomCreator.UpdateCollider();
            roomCreator.ColliderPoints.Clear();
        }

        if (GUILayout.Button("Remove Points"))
        {
            roomCreator.ColliderPoints.Clear();
        }

        if (GUILayout.Button("Clear Mesh"))
        {
            roomCreator.ClearMesh();
            roomCreator.UpdateCollider();
            roomCreator.ClearMesh();
        }

        if (GUILayout.Button("Create Rooms"))
        {
            roomCreator.CreateRoom();
        }
    }
    public Vector3? GetMousePos(RB_RoomCreator detectionZone)
    {
        // Get the position of the mouse on the object with the selected layer
        Vector3 mousePosition = Event.current.mousePosition;
        Vector3? raycastMousePos = null;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);

        // Sort hits by distance
        Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

        foreach (var hit in hits)
        {
            if (detectionZone.objectToDrawOn == hit.collider.gameObject)
            {
                raycastMousePos = hit.point;
                break;
            }
        }
        return raycastMousePos;
    }

    private void SetPoint(RB_RoomCreator detectionZone)
    {
        if (GetMousePos(detectionZone) != null)
        {
            detectionZone.ColliderPoints.Add(GetMousePos(detectionZone).Value);
        }
    }

    void OnSceneGUI()
    {
        RB_RoomCreator detectionZone = (RB_RoomCreator)target;
        detectionZone.PointsInterval = Mathf.Clamp(detectionZone.PointsInterval, 1, int.MaxValue);

        Event e = Event.current;

        if (!detectionZone.ShouldDrawCollider)
            return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            _isDrawing = true;
            bool foundPoint = false;
            if (GetMousePos(detectionZone) != null)
            {
                Vector3 mousePosition = GetMousePos(detectionZone).Value;
                //If there's any point nearby the mouse click, remove it
                if ((GetClosestPointWithinDistance(detectionZone.ColliderPoints, mousePosition, 1) != null))
                {
                    detectionZone.ColliderPoints.Remove(GetClosestPointWithinDistance(detectionZone.ColliderPoints, mousePosition, 1).Value);
                    foundPoint = true;
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
#endif
