﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

public class RB_RoomManager:MonoBehaviour
{
    //Rooms
    private List<RB_Room> _rooms = new();

    //Components
    private Transform _transform;
    public static RB_RoomManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        _transform = transform;
    }

    private void Start()
    {
        InitializeRooms();
    }

    private void InitializeRooms()
    {
        foreach (Transform roomTransform in _transform)
        {
            if (roomTransform.TryGetComponent<RB_Room>(out RB_Room room))
            {
                _rooms.Add(room);
            }
        }
    }

    public List<GameObject> GetDetectedEnemies(int roomIndex)
    {
        return _rooms[roomIndex].DetectedEnemies;
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

    public void ClearRooms()
    {
        foreach (Transform room in transform)
        {
            DestroyImmediate(room.gameObject);
        }
        _rooms.Clear();
    }
}