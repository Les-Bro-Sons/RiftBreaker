using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_Transition : MonoBehaviour
{
    [SerializeField] private const string ROOT_PATH = "Prefabs/Transitions";

    protected float _currentTime;
    public float Duration;
    public SPEEDTYPES SpeedType;
    public int NextSceneID;

    public FADETYPE InTransition;
    public FADETYPE OutTransition;
    public float InDuration = 0.5f;
    public float OutDuration = 0.5f;

    public bool FadeIn = true;

    public bool FinishedTransition = false;
    public static RB_Transition OnTransition(Transform parent, int sceneID, float inDuration, float outDuration, FADETYPE inTransition = FADETYPE.Rift, FADETYPE outTransition = FADETYPE.Rift, SPEEDTYPES inCurve = SPEEDTYPES.Linear, SPEEDTYPES outCurve = SPEEDTYPES.Linear)
    {
        GameObject newObject = new GameObject("Transition: IN=" + inTransition.ToString() + " OUT=" + outTransition.ToString());
        newObject.transform.parent = parent;
        RB_Transition newTransition = newObject.AddComponent<RB_Transition>();

        newTransition.StartCoroutine(newTransition.Transitioning(sceneID, inDuration, outDuration, inTransition, outTransition, inCurve, outCurve));
        return newTransition;
    }

    private IEnumerator Transitioning(int sceneId, float inDuration, float outDuration, FADETYPE inTransition, FADETYPE outTransition, SPEEDTYPES inCurve, SPEEDTYPES outCurve)
    {
        //RB_TimescaleManager.Instance.SetModifier(gameObject, "TransitionSceneTimescale", 0, 900, 4);
        RB_SceneTransitionManager.Instance.TransitionCanvas.worldCamera = Camera.main;

        #region In Transition
        RB_Transition currentTransition = Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{inTransition.ToString()}"), transform.parent).GetComponent<RB_Transition>();
        currentTransition.Duration = inDuration;
        currentTransition.FadeIn = true;
        currentTransition.SpeedType = inCurve;
        while (!currentTransition.FinishedTransition) yield return null;
        #endregion

        #region Loading scene
        Scene currentScene = SceneManager.GetActiveScene();

        AsyncOperation loadingScreenOp = SceneManager.LoadSceneAsync("LOADING", LoadSceneMode.Additive);
        while (!loadingScreenOp.isDone) yield return null;
        yield return StartCoroutine(RB_LoadingScreen.Instance.OpeningLoadingScreen());

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        while (!unloadOp.isDone) yield return null;

        Destroy(currentTransition.gameObject);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;
        while (loadOp.progress < 0.9f) yield return null;

        yield return StartCoroutine(RB_LoadingScreen.Instance.ClosingLoadingScreen());

        loadOp.allowSceneActivation = true;
        while (!loadOp.isDone) yield return null;
        Scene newScene = SceneManager.GetSceneByBuildIndex(sceneId);
        SceneManager.SetActiveScene(newScene);

        Scene loadScene = SceneManager.GetSceneByName("LOADING");
        AsyncOperation unloadLoadingScreenOp = SceneManager.UnloadSceneAsync(loadScene);
        while (!unloadLoadingScreenOp.isDone) yield return null;
        #endregion

        RB_SceneTransitionManager.Instance.TransitionCanvas.worldCamera = Camera.main;

        #region Out Transition
        currentTransition = Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{outTransition.ToString()}"), transform.parent).GetComponent<RB_Transition>();
        currentTransition.Duration = outDuration;
        currentTransition.FadeIn = false;
        currentTransition.SpeedType = outCurve;
        while (!currentTransition.FinishedTransition) yield return null;
        Destroy(currentTransition.gameObject);
        #endregion

        //RB_TimescaleManager.Instance.RemoveModifier("TransitionSceneTimescale");
        Destroy(gameObject);

        yield return null;
    }
}