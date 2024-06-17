using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(RB_TutorialManager))]
public class RB_CustomEditorTutoManager : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RB_TutorialManager tutoManager = (RB_TutorialManager)target;

        if (GUILayout.Button("Slow time"))
        {
            tutoManager.StartSlowTime();
        }

        if (GUILayout.Button("Speed up time"))
        {
            tutoManager.StartSpeedUpTime();
        }

        if(GUILayout.Button("Slow time by enemy distance"))
        {
            tutoManager.StartSlowDownTimeByDistance();
        }
    }
}
#endif
public class RB_TutorialManager : MonoBehaviour
{
    //Instance
    public static RB_TutorialManager Instance;


    public bool IsTuto = true;

    //bools
    private bool _shouldDoTuto = false;
    private bool _shouldSlowTime = false;
    private bool _shouldSpeedUpTime = false;
    private bool _shouldSlowTimeByEnemyDistance = false;
    private bool _shouldDarkenBackground = false;
    private bool _shouldBrightenBackground = false;
    private bool _shouldFadeInRewindTuto = false;
    private bool _shouldFadeOutRewindTuto = false;
    private bool _shouldAnimateRobertLeNec = false;

    //positions
    private float _startEnemyDistance;
    private Vector3 _targetEnemyPos;
    private Vector3 _startRobertPos;

    //Properties
    public float ChangeSpeed;
    public float RobertMoveHeight;
    public float RobertMoveSpeed;

    //Components
    [SerializeField] private RB_AI_BTTree EnemyToSlowDownTimeByDistance;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _rewindTutoImage;
    [SerializeField] private Image _roberLeNec;
    Transform robertTransform;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        robertTransform = _roberLeNec.GetComponent<RectTransform>();
    }

    private void Start()
    {
        IsTuto = !RB_SaveManager.Instance.SaveObject.TutoDone;
        EnemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(StartSlowDownTimeByDistance);
        EnemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(StartDarkenBackground);
        EnemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(StartFadeIn);
        EnemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(StartAnimateRobert);
        RB_TimeManager.Instance.EventStartRewinding.AddListener(SetNormalTime);
        RB_TimeManager.Instance.EventStartRewinding.AddListener(StartBrightenBackground);
        RB_TimeManager.Instance.EventStartRewinding.AddListener(StartFadeOut);
        RB_TimeManager.Instance.EventStartRewinding.AddListener(StopAnimateRobert);
        InititializeImages();
    }

    private void Update()
    {
        SlowTime();
        SpeedUpTime();
        SlowTimeByEnemyDistance();
        DarkenBackground();
        BrightenBackground();
        FadeIn();
        FadeOut();
        AnimateRobert();
    }

    private void InititializeImages()
    {
        print("initialize");
        _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, 0);
        _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
        _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
    }

    public void StartAnimateRobert()
    {
        if (IsTuto)
        {
            _shouldAnimateRobertLeNec = true;
            _startRobertPos = robertTransform.localPosition;
        }
    }

    public void StopAnimateRobert()
    {
        if (IsTuto)
        {
            _shouldAnimateRobertLeNec = false;
        }
    }

    public void AnimateRobert()
    {
        if(IsTuto && _shouldAnimateRobertLeNec)
        {
            print("animate robert");
            robertTransform.localPosition = new Vector3(robertTransform.localPosition.x, _startRobertPos.y + RobertMoveHeight * Mathf.Sin(Time.unscaledTime * RobertMoveSpeed), robertTransform.localPosition.z);
        }
    }

    public void StartBrightenBackground()
    {
        if (IsTuto)
        {
            _shouldBrightenBackground = true;
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, .6f);
        }
    }

    public void BrightenBackground()
    {
        if(IsTuto && _shouldBrightenBackground)
        {
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, _backgroundImage.color.a - Time.unscaledDeltaTime * ChangeSpeed);
            if (_backgroundImage.color.a <= 0)
            {
                _shouldBrightenBackground = false;
            }
        }
    }

    public void StartFadeOut()
    {
        if (IsTuto)
        {
            _shouldFadeOutRewindTuto = true;
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 1);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 1);
        }
    }

    public void FadeOut()
    {
        if(IsTuto && _shouldFadeOutRewindTuto)
        {
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, _rewindTutoImage.color.a - Time.unscaledDeltaTime * ChangeSpeed);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, _rewindTutoImage.color.a - Time.unscaledDeltaTime * ChangeSpeed);
            if (_rewindTutoImage.color.a <= 0)
            {
                _shouldFadeOutRewindTuto = false;
            }
        }
    }

    public void StartFadeIn()
    {
        if (IsTuto)
        {
            _shouldFadeInRewindTuto = true;
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
        }
    }

    public void FadeIn()
    {
        if (IsTuto && _shouldFadeInRewindTuto)
        {
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, _rewindTutoImage.color.a + Time.unscaledDeltaTime * ChangeSpeed);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, _rewindTutoImage.color.a + Time.unscaledDeltaTime * ChangeSpeed);
            if (_rewindTutoImage.color.a >= 1)
            {
                _shouldFadeInRewindTuto = false;
            }
        }
    }

    public void StartDarkenBackground()
    {
        if (IsTuto)
        {
            _shouldDarkenBackground = true;
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, 0);
        }
    }

    public void DarkenBackground()
    {
        if (IsTuto && _shouldDarkenBackground)
        {
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, _backgroundImage.color.a + Time.unscaledDeltaTime * ChangeSpeed);
            if (_backgroundImage.color.a >= .6f)
            {
                _shouldDarkenBackground = false;
            }
        }
    }

    public void SetNormalTime()
    {
        if (IsTuto)
        {
            Time.timeScale = 1.0f;
        }
    }

    public void StartSlowDownTimeByDistance()
    {
        if (IsTuto)
        {
            _startEnemyDistance = 10;
            _shouldSlowTimeByEnemyDistance = true;
        }
    }

    public void SlowTimeByEnemyDistance()
    {
        if (IsTuto && _shouldSlowTimeByEnemyDistance)
        {
            Time.timeScale = Vector3.Distance(RB_PlayerMovement.Instance.transform.position, EnemyToSlowDownTimeByDistance.transform.position) / _startEnemyDistance;
            if(Time.timeScale <= .5f)
            {
                StartSlowTime();
            }
        }
    }

    public void StartSpeedUpTime()
    {
        if (IsTuto)
        {
            _shouldSlowTime = false;
            _shouldSlowTimeByEnemyDistance = false;
            _shouldSpeedUpTime = true;

        }
    }

    public void SpeedUpTime()
    {
        if (IsTuto && _shouldSpeedUpTime)
        {
            Time.timeScale += Time.unscaledDeltaTime * ChangeSpeed;
            if (Time.timeScale >= .9f)
            {
                print("Time = normal");
                Time.timeScale = 1;
                _shouldSpeedUpTime = false;
            }
        }
    }

    public void SlowTime()
    {
        if (IsTuto && _shouldSlowTime)
        {
            Time.timeScale -= Time.unscaledDeltaTime * ChangeSpeed;
            if (Time.timeScale <= 0.1)
            {
                print("time = slow");
                Time.timeScale = 0;
                _shouldSlowTime = false;
            }
        }
    }

    public void StartTuto()
    {
        if (IsTuto)
        {
            _shouldDoTuto = true;
        }
    }

    public void StartSlowTime()
    {
        if (IsTuto)
        {
            _shouldSpeedUpTime = false;
            _shouldSlowTimeByEnemyDistance = false;
            _shouldSlowTime = true;
        }
    }
}
