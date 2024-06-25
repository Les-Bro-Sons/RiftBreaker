using UnityEngine;

public class RB_YogCinematic : MonoBehaviour
{
    private float _bloodTimer = 0;
    private float _bloodSpawnTime = 0.5f;

    [SerializeField] private GameObject _yogBlood;

    private void Update()
    {
        _bloodTimer += Time.deltaTime;
        if (_bloodTimer > _bloodSpawnTime)
        {
            _bloodTimer = 0;
        }
    }
}
