using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_LoadingScreen : MonoBehaviour
{
    public static RB_LoadingScreen Instance;

    [SerializeField] private Image _foregroundImage;

    private void Awake()
    {
        Instance = this;
        _foregroundImage.color = Color.black;
    }

    public IEnumerator OpeningLoadingScreen()
    {
        _foregroundImage.color = Color.black;
        float startTime = Time.unscaledTime;
        float duration = 0.5f;
        while (_foregroundImage.color.a > 0)
        {
            Color newColor = _foregroundImage.color;
            float elapsedTime = Time.unscaledTime - startTime;
            newColor.a = Mathf.Lerp(1, 0, elapsedTime / duration);
            _foregroundImage.color = newColor;
            yield return null;
        }
        _foregroundImage.color = Color.clear;
    }

    public IEnumerator ClosingLoadingScreen()
    {
        StopCoroutine(OpeningLoadingScreen());
        float startTime = Time.unscaledTime;
        float duration = 0.5f;
        while (_foregroundImage.color.a < 1)
        {
            Color newColor = _foregroundImage.color;
            float elapsedTime = Time.unscaledTime - startTime;
            newColor.a = Mathf.Lerp(0, 1, elapsedTime / duration);
            _foregroundImage.color = newColor;
            yield return null;
        }
        _foregroundImage.color = Color.black;
    }
}
