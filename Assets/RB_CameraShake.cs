using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private Vector3 _defaultPos = new();

    [SerializeField] GameObject _buttonToShake;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _defaultPos = _buttonToShake.transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = _buttonToShake.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude + _defaultPos.x;
            float y = Random.Range(-1f, 1f) * magnitude + _defaultPos.y;

            _buttonToShake.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        _buttonToShake.transform.localPosition = originalPosition;
    }
}
