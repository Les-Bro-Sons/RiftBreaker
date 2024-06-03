using AYellowpaper.SerializedCollections;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }

    private void Start()
    {
        TransitionCanvas.worldCamera = Camera.main;
        //_virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;

        //NewTransition(FadeType.ToString());
    }

    /*
        public void SwitchSceneTransition(string nameScene, float duration)
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            if (_fadeInCoroutine != null)
                StopCoroutine(_fadeInCoroutine);
            if (_fadeOutCoroutine != null)
                StopCoroutine(_fadeOutCoroutine);

            _fadeCoroutine = StartCoroutine(Fade(nameScene, duration));
        }
    */

    public void SetImageAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }



    public void NewTransition(string nameTransition)
    {
        TransitionCanvas.worldCamera = Camera.main;
        if (CurrentTransition == null)
            CurrentTransition = Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{nameTransition}"), transform).GetComponent<RB_Transition>();
    }

    public void NewScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void NewScene(int idScene)
    {
        //SceneManager.LoadScene(idScene);
    }
}