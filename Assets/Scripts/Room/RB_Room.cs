using System.Collections.Generic;
using UnityEngine;

public class RB_Room : MonoBehaviour
{
    public List<GameObject> DetectedEnemies = new();
    public List<GameObject> DetectedAllies = new();
    public bool IsPlayerInRoom;

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
