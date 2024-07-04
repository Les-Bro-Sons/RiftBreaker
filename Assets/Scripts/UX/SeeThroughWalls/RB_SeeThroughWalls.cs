using UnityEngine;
using UnityEngine.Rendering.UI;

public class RB_SeeThroughWalls : MonoBehaviour
{
    private new Transform transform;

    private bool _isTouchingWall = false;
    public float LastTimeWallTouched = 0;
    private float _lastTimeWallTouchedOld = 0;
    private float _lastUntouchedTime = 0;
    private float _timeTarget = 0;

    [SerializeField] private float _raycastLength = 1.5f;
    [SerializeField] private float _raycastDelay = 0.2f;
    private float _lastRaycastTime = 0;

    [SerializeField] private float _unLerpingOffset = 0.1f;

    public Vector3 ShaderPosition;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    private void Start()
    {
        _lastRaycastTime += Random.Range(0, _raycastDelay);
        if (RB_SeeThroughWallsManager.Instance == null) this.enabled = false;
        RB_SeeThroughWallsManager.Instance.AddEntity(this);
    }

    private void Update()
    {
        bool canShootRaycast = Time.time > _lastRaycastTime + _raycastDelay;
        if (canShootRaycast) _lastRaycastTime = Time.time;
        RaycastHit hitInfo;
        if ((canShootRaycast && Physics.Raycast(transform.position, Vector3.back, out hitInfo, _raycastLength, (1 << 3))) || (_isTouchingWall && Physics.Raycast(transform.position, Vector3.back, out hitInfo, _raycastLength, (1 << 3))))
        {
            if (!_isTouchingWall)
            {
                LastTimeWallTouched = Time.time;
                _isTouchingWall = true;
            }
            
            
            ShaderPosition = hitInfo.point;
        }
        else
        {
            if (_isTouchingWall)
            {
                _lastTimeWallTouchedOld = LastTimeWallTouched;
                _lastUntouchedTime = Time.time;
                _timeTarget = Time.time + RB_SeeThroughWallsManager.Instance.LerpTime;
                _isTouchingWall = false;
            }
            float currentValue = (Time.time + _unLerpingOffset - _lastUntouchedTime) / (RB_SeeThroughWallsManager.Instance.LerpTime);
            LastTimeWallTouched = Mathf.Lerp(_lastTimeWallTouchedOld, _timeTarget, Mathf.Clamp(currentValue, 0, 1));
            if (currentValue >= 1)
            {
                ShaderPosition = Vector3.up * 1000;
                _timeTarget = Time.time;
            }
            else
            {
                ShaderPosition = Vector3.Lerp(ShaderPosition, transform.position, 4 * Time.deltaTime);
            }
        }
    }
}
