using System.Collections;
using UnityEngine;

public class RB_ButtonShake : MonoBehaviour
{
    public static RB_ButtonShake Instance; // Singleton instance

    private Vector3 _defaultPos = new Vector3(); // Default position of the button

    [SerializeField] GameObject _buttonToShake; // Reference to the button GameObject to shake

    private void Awake()
    {
        // Singleton pattern: ensure only one instance exists
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Destroy duplicate instances
    }

    private void Start()
    {
        _defaultPos = _buttonToShake.transform.localPosition; // Store the default position of the button
    }

    // Coroutine to shake the button for a specified duration and magnitude
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = _buttonToShake.transform.localPosition; // Store the original position of the button

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Generate random offsets within magnitude to create shaking effect
            float x = Random.Range(-1f, 1f) * magnitude + _defaultPos.x;
            float y = Random.Range(-1f, 1f) * magnitude + _defaultPos.y;

            // Apply the new position with shaking effect
            _buttonToShake.transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime; // Update elapsed time based on deltaTime

            yield return null; // Wait for the next frame
        }

        _buttonToShake.transform.localPosition = originalPosition; // Reset button position to original after shaking
    }
}
