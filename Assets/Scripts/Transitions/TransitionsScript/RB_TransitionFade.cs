using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_TransitionFade : RB_Transition
{
    [Header("Parameters")]
    [SerializeField] private Image _fadeImage;
    //private float fadeInDuration = FadeInTime > 0 ? FadeInTime : _duration;
    

    void Awake()
    {
        if (_fadeImage.color.a != 0)
        {
            Color newColor = _fadeImage.color;
            newColor.a = 0;
            _fadeImage.color = newColor;
        }
    }

    void Start()
    {
        StartCoroutine(Fade(NextSceneID, Duration, speedType : RB_SceneTransitionManager.Instance.SpeedType));
        //RB_SaveManager.Instance.SaveObject.CurrentLevel
    }

    public override IEnumerator Fade(int nameScene, float duration, SPEEDTYPES speedType)
    {
        yield return StartCoroutine(FadeImage(_fadeImage, true, duration * 0.5f, speedType)); // Fade in for half the duration.
        RB_SceneTransitionManager.Instance.NewScene(nameScene);

        yield return new WaitForEndOfFrame(); // Wait for one frame.
        yield return new WaitForEndOfFrame(); // Wait for one frame.

        //_virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
        //RZ_AudioSettings.Instance.InitAudio();

        yield return StartCoroutine(FadeImage(_fadeImage, false, duration * 0.5f, speedType)); // Fade out for the remaining duration.
        yield return new WaitForEndOfFrame(); // Wait for one frame.
        Destroy(gameObject);
    }

    public override IEnumerator FadeImage(Image image, bool fadeIn, float duration, SPEEDTYPES speedType)
    {
        return base.FadeImage(image, fadeIn, duration, speedType);
    }
}