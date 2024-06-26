using System.Collections.Generic;
using UnityEngine;

public class RB_Room : MonoBehaviour
{
    //Properties
    [Header("Properties")]
    public bool IsClosedRoom;
    private bool _isRoomClosed;

    //In room
    [Header("In Room")]
    public List<RB_Health> DetectedEnemies = new();
    public List<RB_Health> DetectedAllies = new();
    public List<RB_Door> Doors = new();
    public bool IsPlayerInRoom;

    private void Start()
    {
        SetLayerToAllChildren(LayerMask.NameToLayer("Room"), transform);
    }

    private void Update()
    {
        if((RB_LevelManager.Instance.CurrentPhase == PHASES.Combat || RB_LevelManager.Instance.CurrentPhase == PHASES.Boss) && IsClosedRoom && IsPlayerInRoom && !_isRoomClosed && DetectedEnemies.Count >= 0 && !IsAllEnemyDied())
        {
            CloseRoomByRoom();
        }else if (_isRoomClosed && (IsAllEnemyDied() || !IsPlayerInRoom))
        {
            OpenRoomByRoom();
        }
    }

    private void SetLayerToAllChildren(int layer, Transform obj)
    {
        foreach (Transform child in obj)
        {
            child.gameObject.layer = layer;
            SetLayerToAllChildren(layer, child);
        }
    }

    public void OpenRoomByRoom() //Open the room. Action made by the room itself
    {
        _isRoomClosed = false;
        foreach (RB_Door door in Doors)
        {
            if (door.IsControledByRoom)
                door.Open();
        }
    }

    public void CloseRoomByRoom() //Close the room. Action made by the room itself
    {
        _isRoomClosed = true;
        foreach (RB_Door door in Doors)
        {
            if (door.IsControledByRoom)
                door.Close();
        }
    }

    public void CloseRoom()
    {
        _isRoomClosed = true;
        foreach (RB_Door door in Doors)
        {
            door.Close();
        }
    }

    public void OpenRoom()
    {
        _isRoomClosed = false;
        foreach (RB_Door door in Doors)
        {
            door.Open();
        }
    }

    public void AddDetectedEnemy(RB_Health detectedEnemy) //Add the detected enemies to the list of detected enemies
    {
        if (!DetectedEnemies.Contains(detectedEnemy))
        {
            if (DetectedAllies.Contains(detectedEnemy)) DetectedAllies.Remove(detectedEnemy);
            DetectedEnemies.Add(detectedEnemy);
        }
    }

    public void RemoveDetectedEnemy(RB_Health lostEnemy) //Remove the detected enemies from the list of detected enemies
    {
        if (DetectedEnemies.Contains(lostEnemy))
        {
            DetectedEnemies.Remove(lostEnemy);
        }
    }

    public void AddDectedAlly(RB_Health detectedAlly) //Add the detected allies to the list of detected enemies
    {
        if (!DetectedAllies.Contains(detectedAlly))
        {
            if (DetectedEnemies.Contains(detectedAlly)) DetectedEnemies.Remove(detectedAlly);
            DetectedAllies.Add(detectedAlly);
        }
    }

    public void RemoveDectedAlly(RB_Health lostAlly) //Remove the detected allies from the list of detected enemies
    {
        if (DetectedAllies.Contains(lostAlly))
        {
            DetectedAllies.Remove(lostAlly);
        }
    }

    public bool IsAllEnemyDied() //If all the enemies are dead
    {
        int enemyDead = 0;
        foreach(RB_Health enemyHealth in DetectedEnemies)
        {
            if (enemyHealth.Dead)
            {
                enemyDead++;
            }
        }

        return enemyDead == DetectedEnemies.Count;
    }
}
