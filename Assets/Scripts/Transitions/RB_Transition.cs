using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_Transition : MonoBehaviour
{
    [SerializeField] private const string ROOT_PATH = "Prefabs/Transitions";

    protected float _currentTime;
    //public float FadeInTime;
    //public float FadeOutTime;
    public float Duration;

    public int NextSceneID;

    // Start is called before the first frame update
    void Start()
    {
        //FadeTransition("SampleScene", 5f, speedType: RB_SceneTransitionManager.Instance.SpeedType);
    }

    public virtual IEnumerator Fade(int nameScene, float duration, SPEEDTYPES speedType)
    {
        yield return null;
    }

    public virtual IEnumerator FadeImage(Image image, bool fadeIn,  float duration, SPEEDTYPES speedType)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = image.color.a;
        float startTime = Time.unscaledTime;

        while (image.color.a < targetAlpha)
        {
            if (fadeIn)
            {
                RB_TimescaleManager.Instance.SetModifier(gameObject, "TransitionSceneTimescale", 0, 900, 4);
            }
            else
            {
                RB_TimescaleManager.Instance.RemoveModifier("TransitionSceneTimescale");
            }
            float elapsedTime = (Time.unscaledTime - startTime) / duration;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, RB_SceneTransitionManager.Instance.SpeedCurves[speedType].Evaluate(elapsedTime));
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
    }
}