using AYellowpaper.SerializedCollections;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using Cinemachine;

public class RB_SceneTransitionManager : MonoBehaviour
{
    public static RB_SceneTransitionManager Instance; // Singleton instance of the manager.

    //private CinemachineVirtualCamera _virtualCamera;
    [HideInInspector] public Canvas TransitionCanvas;

    [Header("Management")]
    [SerializeField] private const string ROOT_PATH = "Prefabs/Transitions";

    [HideInInspector] public RB_Transition CurrentTransition;

    [Header("Name Scene")]
    public string PlayButton = "Niveau1-MVP";

    [Header("Curves")]
    [SerializedDictionary("ID", "Zoom Curve")]
    public SerializedDictionary<ZOOMTYPES, AnimationCurve> AnimationCurves = new();
    
    [SerializedDictionary("ID", "Speed Curve")]
    public SerializedDictionary<SPEEDTYPES, AnimationCurve> SpeedCurves = new();




    private void Awake()
    {
        TransitionCanvas = GetComponent<Canvas>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
        TransitionCanvas.worldCamera = Camera.main;
        //_virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
    }



    public void SwitchSceneTransition()
    {
        StartCoroutine(SwitchSceneTransitionCoroutine());
    }

    private IEnumerator SwitchSceneTransitionCoroutine()
    {
        yield return null;
    }

    public void SetImageAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    public void NewTransition(FADETYPE inTransition, int nextSceneIndex, FADETYPE? outTransition = null, float inDuration = 2, float outDuration = 2, SPEEDTYPES inCurve = SPEEDTYPES.Linear, SPEEDTYPES? outCurve = null)
    {
        if (outTransition == null) outTransition = inTransition;
        if (outCurve == null) outCurve = inCurve;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings) nextSceneIndex = 0;
        TransitionCanvas.worldCamera = Camera.main;
        if (CurrentTransition == null)
        {
            CurrentTransition = RB_Transition.OnTransition(transform, nextSceneIndex, inDuration, outDuration, inTransition, outTransition.Value, inCurve, outCurve.Value);
        }
    }

    public void NewTransition(FADETYPE inTransition, string nextSceneName, FADETYPE? outTransition = null)
    {
        NewTransition(inTransition, SceneManager.GetSceneByName(nextSceneName).buildIndex, outTransition);
    }

    public void NewScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void NewScene(int idScene)
    {
        SceneManager.LoadScene(idScene);
    }
}