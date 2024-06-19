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
    public List<RB_Health> DetectedEnemies = new();
    public List<RB_Health> DetectedAllies = new();
    public List<RB_Door> Doors = new();
    public bool IsPlayerInRoom;

    private void Update()
    {
        if((RB_LevelManager.Instance.CurrentPhase == PHASES.Combat || RB_LevelManager.Instance.CurrentPhase == PHASES.Boss) && IsClosedRoom && IsPlayerInRoom && !isRoomClosed && DetectedEnemies.Count >= 0 && !IsAllEnemyDied())
        {
            CloseRoom();
        }else if (isRoomClosed && (IsAllEnemyDied() || !IsPlayerInRoom))
        {
            OpenRoom();
        }
    }

    public void CloseRoom()
    {
        isRoomClosed = true;
        foreach (RB_Door door in Doors)
        {
            door.Close();
        }
    }

    public void OpenRoom()
    {
        isRoomClosed = false;
        foreach (RB_Door door in Doors)
        {
            door.Open();
        }
    }

    public void AddDetectedEnemy(RB_Health detectedEnemy)
    {
        if (!DetectedEnemies.Contains(detectedEnemy))
        {
            if (DetectedAllies.Contains(detectedEnemy)) DetectedAllies.Remove(detectedEnemy);
            DetectedEnemies.Add(detectedEnemy);
        }
    }

    public void RemoveDetectedEnemy(RB_Health lostEnemy)
    {
        if (DetectedEnemies.Contains(lostEnemy))
        {
            DetectedEnemies.Remove(lostEnemy);
        }
    }

    public void AddDectedAlly(RB_Health detectedAlly)
    {
        if (!DetectedAllies.Contains(detectedAlly))
        {
            if (DetectedEnemies.Contains(detectedAlly)) DetectedEnemies.Remove(detectedAlly);
            DetectedAllies.Add(detectedAlly);
        }
    }

    public void RemoveDectedAlly(RB_Health lostAlly)
    {
        if (DetectedAllies.Contains(lostAlly))
        {
            DetectedAllies.Remove(lostAlly);
        }
    }

    public bool IsAllEnemyDied()
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
