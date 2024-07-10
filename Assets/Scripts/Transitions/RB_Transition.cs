using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RB_Transition : MonoBehaviour
{
    [SerializeField] private const string ROOT_PATH = "Prefabs/Transitions";

    protected float _currentTime;
    public float Duration;

    public int NextSceneID;

    public FADETYPE InTransition;
    public FADETYPE OutTransition;
    public SPEEDTYPES SpeedType;

    public bool FadeIn = true;

    public bool FinishedTransition = false;
    public static RB_Transition OnTransition(Transform parent, int sceneID, float duration, FADETYPE inTransition = FADETYPE.Rift, FADETYPE outTransition = FADETYPE.Rift, SPEEDTYPES speedType = SPEEDTYPES.Linear)
    {
        GameObject newObject = new GameObject("Transition: IN=" + inTransition.ToString() + " OUT=" + outTransition.ToString());
        newObject.transform.parent = parent;
        RB_Transition newTransition = newObject.AddComponent<RB_Transition>();

        newTransition.NextSceneID = sceneID;
        newTransition.Duration = duration;
        newTransition.InTransition = inTransition;
        newTransition.OutTransition = outTransition;
        newTransition.SpeedType = speedType;

        newTransition.StartCoroutine(newTransition.Transitioning(sceneID, duration, inTransition, outTransition, speedType));
        return newTransition;
    }

    private IEnumerator Transitioning(int sceneId, float duration, FADETYPE inTransition, FADETYPE outTransition, SPEEDTYPES speedType)
    {
        RB_TimescaleManager.Instance.SetModifier(gameObject, "TransitionSceneTimescale", 0, 900, 4);
        RB_SceneTransitionManager.Instance.TransitionCanvas.worldCamera = Camera.main;

        RB_Transition currentTransition = Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{inTransition.ToString()}"), transform.parent).GetComponent<RB_Transition>();
        currentTransition.FadeIn = true;
        while (!currentTransition.FinishedTransition) yield return null;
        
        RB_SceneTransitionManager.Instance.NewScene(sceneId);

        yield return new WaitForEndOfFrame(); // Wait for one frame.
        yield return new WaitForEndOfFrame(); // Wait for one frame.

        RB_SceneTransitionManager.Instance.TransitionCanvas.worldCamera = Camera.main;

        Destroy(currentTransition.gameObject);

        currentTransition = Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{outTransition.ToString()}"), transform.parent).GetComponent<RB_Transition>();
        currentTransition.FadeIn = false;
        while (!currentTransition.FinishedTransition) yield return null;
        Destroy(currentTransition.gameObject);
        RB_TimescaleManager.Instance.RemoveModifier("TransitionSceneTimescale");
        Destroy(gameObject);

        yield return null;
    }
}