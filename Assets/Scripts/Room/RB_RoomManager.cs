using System.Collections.Generic;
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

    public List<GameObject> GetDetectedEnemies(int roomIndex)
    {
        return _rooms[roomIndex].DetectedEnemies;
    }

    public List<GameObject> GetDetectedAllies(int roomIndex)
    {
        return _rooms[roomIndex].DetectedAllies;
    }

    public int? GetEntityRoom(TEAMS team, GameObject entity)
    {
        int? roomIndex = null;
        for(int i = 0; i < _rooms.Count; i++)
        {
            if (team == TEAMS.Ai)
            {

                if (GetDetectedEnemies(i).Contains(entity))
                {
                    roomIndex = i;
                    break;
                }
            }
            else
            {
                if (GetDetectedAllies(i).Contains(entity))
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
                    DestroyImmediate(roomScript.gameObject);
                    _rooms.Remove(roomScript);
                }
            }
            UpdateRooms();
            maxIter --;
            if(maxIter <= 0)
                break;
        }
        
    }
}