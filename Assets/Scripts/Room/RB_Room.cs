using System.Collections.Generic;
using UnityEngine;

public class RB_Room : MonoBehaviour
{
    public List<GameObject> DetectedEnemies = new();
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
}
