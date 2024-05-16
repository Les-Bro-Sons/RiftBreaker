using AYellowpaper.SerializedCollections;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Cinemachine;

public class RB_SceneTransitionManager : MonoBehaviour
{
    public static RB_SceneTransitionManager Instance; // Singleton instance of the manager.

    //private CinemachineVirtualCamera _virtualCamera;

    [Header("Management")]
    [SerializeField] private const string ROOT_PATH = "Prefabs/Transitions";

    public SPEEDTYPES SpeedType = SPEEDTYPES.Linear; // Default speed type set to LINEAR.
    public FADETYPE FadeType = FADETYPE.Fade; // Default fade type set to basic fade.
    public RB_Transition CurrentTransition;
    

    [Header("Curves")]
    [SerializedDictionary("ID", "Zoom Curve")]
    public SerializedDictionary<ZOOMTYPES, AnimationCurve> AnimationCurves = new();
    
    [SerializedDictionary("ID", "Speed Curve")]
    public SerializedDictionary<SPEEDTYPES, AnimationCurve> SpeedCurves = new();




    private void Awake()
    {
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
        //_virtualCamera = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;

        /*if (_fadeImage.color.a != 0) // Check if the fade image is not fully transparent.
        {
            Color newColor = _fadeImage.color;
            newColor.a = 0;
            _fadeImage.color = newColor;
        }*/

        NewTransition(FadeType.ToString());
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

    public void NewTransition(string nameTransition)
    {
        Instantiate(Resources.Load<GameObject>($"{ROOT_PATH}/{nameTransition}"), transform);
    }

    public void NewScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);

/*        if (SceneManager.GetSceneByName(nameScene).buildIndex < 4 || SceneManager.GetSceneByName("EndMenu").buildIndex >= 18)
            RZ_GameManager.Instance.hud.SetActive(false);
        else
            RZ_GameManager.Instance.hud.SetActive(true);*/
    }
}