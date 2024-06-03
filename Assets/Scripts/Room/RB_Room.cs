using System.Collections.Generic;
using UnityEngine;

public class RB_Room : MonoBehaviour
{
    //Properties
    [Header("Properties")]
    public bool IsClosedRoom;
    private bool isRoomClosed;

    //In room
    [Header("In Room")]
    public List<GameObject> DetectedEnemies = new();
    public List<GameObject> DetectedAllies = new();
    public List<GameObject> Doors = new();
    public bool IsPlayerInRoom;

    private void Update()
    {
        if(IsClosedRoom && IsPlayerInRoom && !isRoomClosed)
        {
            

        }
    }

    public void CreateDoors()
    {
        isRoomClosed = true;
        foreach(GameObject door in Doors)
        {
            
        }
    }

    public void CloseRoom()
    {

    }

    public void AddDetectedEnemy(GameObject detectedEnemy)
    {
        if (!DetectedEnemies.Contains(detectedEnemy))
        {
            DetectedEnemies.Add(detectedEnemy);
        }
    }

    public void RemoveDetectedEnemy(GameObject lostEnemy)
    {
        if (DetectedEnemies.Contains(lostEnemy))
        {
            DetectedEnemies.Remove(lostEnemy);
        }
    }

    public void AddDectedAlly(GameObject detectedAlly)
    {
        if (!DetectedAllies.Contains(detectedAlly))
        {
            DetectedAllies.Add(detectedAlly);
        }
    }

    public void RemoveDectedAlly(GameObject lostAlly)
    {
        if (DetectedAllies.Contains(lostAlly))
        {
            DetectedAllies.Remove(lostAlly);
        }
    }
}
