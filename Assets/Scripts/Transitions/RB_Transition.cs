using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

public class RB_Transition : MonoBehaviour
{
    [SerializeField] private const string ROOT_PATH = "Prefabs/Transitions";

    protected float _currentTime;
    public float FadeInTime;
    public float FadeOutTime;

    // Start is called before the first frame update
    void Start()
    {
        //FadeTransition("SampleScene", 5f, speedType: RB_SceneTransitionManager.Instance.SpeedType);
    }

    public virtual IEnumerator Fade(string nameScene, float duration, SPEEDTYPES speedType)
    {
        yield return StartCoroutine(FadeIn(duration / 2, speedType)); // Fade in for half the duration.
        RB_SceneTransitionManager.Instance.NewScene(nameScene);

        yield return 0; // Wait for one frame.
        yield return 0;

        //_virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
        //RZ_AudioSettings.Instance.InitAudio();

        yield return StartCoroutine(FadeOut(duration / 2, speedType)); // Fade out for the remaining duration.
    }

    public virtual IEnumerator FadeIn(float duration, SPEEDTYPES speedType)
    {
        yield return null;
    }

    public virtual IEnumerator FadeOut(float duration, SPEEDTYPES speedType)
    {
        yield return null;
    }
}