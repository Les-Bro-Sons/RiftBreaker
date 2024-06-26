using UnityEngine;

public class RB_SpriteFloating : MonoBehaviour
{
    private Transform _transform;
    private float _baseY;

    [SerializeField] private float _floatHeight = 1;
    [SerializeField] private float _floatSpeed = 1;
    [SerializeField] private bool _floatAbs = false;

    private void Awake()
    {
        _transform = transform;
        _baseY = _transform.localPosition.y;
    }

    private void Update()
    {
        if (_floatAbs)
        {
            _transform.localPosition = (Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * _floatSpeed) * _floatHeight)) + (Vector3.up * _baseY);
        }
        else
        {
            _transform.localPosition = (Vector3.up * Mathf.Sin(Time.time * _floatSpeed) * _floatHeight) + (Vector3.up * _baseY);
        }
    }

    private void OnDisable()
    {
        _transform.localPosition = new Vector3(_transform.localPosition.x, _baseY, _transform.localPosition.z);
    }
}
