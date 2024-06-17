using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_TransitionRift : RB_Transition
{
    [Header("Parameters")]
    [SerializeField] private Material _material;

    void Start()
    {
        _material.SetFloat("_MaskAmount", -0.1f);
        StartCoroutine(Fade(NextSceneID, Duration, speedType : RB_SceneTransitionManager.Instance.SpeedType));
    }

    public override IEnumerator Fade(int nameScene, float duration, SPEEDTYPES speedType)
    {

        yield return new WaitForSecondsRealtime(0);

        yield return StartCoroutine(FadeMaterial(_material, true, duration * 0.5f, speedType)); // Fade in for half the duration.
        RB_SceneTransitionManager.Instance.NewScene(nameScene);

        yield return new WaitForEndOfFrame(); // Wait for one frame.
        yield return new WaitForEndOfFrame(); // Wait for one frame.

        RB_SceneTransitionManager.Instance.TransitionCanvas.worldCamera = Camera.main;


        //_virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
        //RZ_AudioSettings.Instance.InitAudio();

        yield return StartCoroutine(FadeMaterial(_material, false, duration * 0.5f, speedType)); // Fade out for the remaining duration.

        Destroy(gameObject);
    }

    public override IEnumerator FadeImage(Image image, bool fadeIn, float duration, SPEEDTYPES speedType)
    {
        return base.FadeImage(image, fadeIn, duration, speedType);
    }

    private IEnumerator FadeMaterial(Material material, bool fadeIn, float duration, SPEEDTYPES speedType)
    {
        float targetValue = fadeIn ? 0.7f : -.1f;
        float maskAmount = material.GetFloat("_MaskAmount");
        float startValue = maskAmount;
        float startTime = Time.unscaledTime;

        while (Mathf.Abs(targetValue - maskAmount) > 0.01f)
        {
            float elapsedTime = (Time.unscaledTime - startTime) / duration;
            maskAmount = Mathf.Lerp(startValue, targetValue, RB_SceneTransitionManager.Instance.SpeedCurves[speedType].Evaluate(elapsedTime)); // avec le speed value
            material.SetFloat("_MaskAmount", maskAmount);
            yield return null;
        }

        material.SetFloat("_MaskAmount", targetValue);
    }
}