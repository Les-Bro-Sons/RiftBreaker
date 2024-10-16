﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if (UNITY_EDITOR)
[CustomEditor(typeof(RB_RoomManager))]
public class RB_CustomEditorRoomManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RB_RoomManager roomManager = (RB_RoomManager)target;

        if(GUILayout.Button("Clear Rooms"))
        {
            roomManager.ClearRooms();
        }
    }
}
#endif

public class RB_RoomManager:MonoBehaviour
{
    //Rooms
    private List<RB_Room> _rooms = new();

    //Components
    public static RB_RoomManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        InitializeRooms();
    }

    private void InitializeRooms()
    {
        _rooms.Clear();
        foreach (Transform roomTransform in transform)
        {
            if (roomTransform.TryGetComponent<RB_Room>(out RB_Room room))
            {
                _rooms.Add(room);
            }
        }
    }

    private void UpdateRooms()
    {
        foreach (Transform roomTransform in transform)
        {
            if (roomTransform.TryGetComponent<RB_Room>(out RB_Room room))
            {
                _rooms.Add(room);
            }
        }
    }

    public List<RB_Health> GetDetectedEnemies(int roomIndex)
    {

        foreach (RB_Health enemy in _rooms[roomIndex].DetectedEnemies.ToList()) 
        {
            if (enemy == null) _rooms[roomIndex].DetectedEnemies.Remove(enemy);
        }
        return _rooms[roomIndex].DetectedEnemies;
    }

    public List<RB_Health> GetDetectedAllies(int roomIndex)
    {
        foreach (RB_Health enemy in _rooms[roomIndex].DetectedAllies.ToList())
        {
            if (enemy == null) _rooms[roomIndex].DetectedAllies.Remove(enemy);
        }
        return _rooms[roomIndex].DetectedAllies;
    }

    public int? GetEntityRoom(TEAMS team, GameObject entity)
    {
        int? roomIndex = null;
        for(int i = 0; i < _rooms.Count; i++)
        {
            if (team == TEAMS.Ai)
            {
                if (GetDetectedEnemies(i).Contains(entity.GetComponent<RB_Health>()))
                {
                    roomIndex = i;
                    break;
                }
            }
            else
            {
                if (GetDetectedAllies(i).Contains(entity.GetComponent<RB_Health>()))
                {
                    roomIndex = i;
                    break;
                }
            }
        }
        

        return roomIndex;
    }

    public int? GetPlayerCurrentRoom()
    {
        int? playerCurrentRoomIndex = null;
        for(int i = 0; i< _rooms.Count; i++)
        {
            if (_rooms[i].IsPlayerInRoom)
            {
                playerCurrentRoomIndex = i;
                break;
            }
        }

        if(playerCurrentRoomIndex == null)
        {
            Debug.LogWarning("The player is not found in any room");
        }

        return playerCurrentRoomIndex;
    }

    public List<RB_Room> GetAllRooms()
    {
        return _rooms;
    }
#if UNITY_EDITOR
    public void ClearRooms()
    {
        UpdateRooms();
        int maxIter = _rooms.Count + 100;
        while (_rooms.Count > 0)
        {
            foreach (Transform room in transform)
            {
                if (room.gameObject.TryGetComponent<RB_Room>(out RB_Room roomScript) && _rooms.Contains(roomScript))
                {
                    EditorUtility.SetDirty(room);
                    DestroyImmediate(roomScript.gameObject);
                    _rooms.Remove(roomScript);
                }
            }
            UpdateRooms();
            maxIter --;
            if(maxIter <= 0)
                break;
        }


        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        
    }
#endif
}
