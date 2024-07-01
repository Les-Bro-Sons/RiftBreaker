using UnityEngine;
using UnityEngine.Rendering.UI;

public class RB_SeeThroughWalls : MonoBehaviour
{
    private new Transform transform;

    public float LastTimeWallTouched = 0;
    private float _lastTimeWallTouchedOld = 0;
    private bool _isTouchingWall = false;
    private float _lastUntouchedTime = 0;
    private float _timeToNormal = 1;

    [SerializeField] private float _raycastDelay = 0.2f;
    private float _lastRaycastTime = 0;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _lastRaycastTime += Random.Range(0, _raycastDelay);
        RB_SeeThroughWallsManager.Instance.Entities.Add(transform);
    }

    private void Update()
    {
        bool canShootRaycast = Time.time > _lastRaycastTime + _raycastDelay;
        if (canShootRaycast) _lastRaycastTime = Time.time;
        if ((canShootRaycast && Physics.Raycast(transform.position, Vector3.back, 1.5f, (1 << 3))) || (!canShootRaycast && _isTouchingWall))
        {
            if (!_isTouchingWall)
            {
                LastTimeWallTouched = Time.time;
                _isTouchingWall = true;
            }
        }
        else
        {
            if (_isTouchingWall)
            {
                _lastTimeWallTouchedOld = LastTimeWallTouched;
                _lastUntouchedTime = Time.time;
                //LastTimeWallTouched = Time.time;
                _isTouchingWall = false;
            }
            LastTimeWallTouched = Mathf.Lerp(_lastTimeWallTouchedOld, Time.time, Mathf.Clamp((Time.time - _lastUntouchedTime) / (RB_SeeThroughWallsManager.Instance.LerpTime), 0, 1));
        }
    }
}
