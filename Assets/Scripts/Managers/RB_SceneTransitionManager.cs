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

    public SPEEDTYPES SpeedType = SPEEDTYPES.Linear; // Default speed type set to LINEAR.
    public FADETYPE FadeType = FADETYPE.Fade; // Default fade type set to basic fade.
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

    public void NewTransition(string nameTransition, int nextSceneIndex)
    {
        TransitionCanvas.worldCamera = Camera.main;
        if (CurrentTransition == null)
        {
            CurrentTransition = Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{nameTransition}"), transform).GetComponent<RB_Transition>();
            CurrentTransition.NextSceneID = nextSceneIndex;
        }
    }

    public void NewTransition(string nameTransition, string nextSceneName)
    {
        NewTransition(nameTransition, SceneManager.GetSceneByName(nextSceneName).buildIndex);
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