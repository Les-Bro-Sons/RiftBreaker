using UnityEditor;
using UnityEngine;

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

    //Time
    private bool _shouldDoTuto = false;
    private bool _shouldSlowTime = false;
    private bool _shouldSpeedUpTime = false;
    private bool _shouldSlowTimeByEnemyDistance = false;
    private float _startEnemyDistance;
    private Vector3 _targetEnemyPos;

    //Properties
    public float ChangeTimeSpeed;

    //Components
    [SerializeField] private RB_AI_BTTree EnemyToSlowDownTimeByDistance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        IsTuto = !RB_SaveManager.Instance.SaveObject.TutoDone;
        EnemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(StartSlowDownTimeByDistance);
        RB_TimeManager.Instance.EventStartRewinding.AddListener(SetNormalTime);
    }

    private void Update()
    {
        SlowTime();
        SpeedUpTime();
        SlowTimeByEnemyDistance();
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
            print("slow down time started");
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
            Time.timeScale += Time.unscaledDeltaTime * ChangeTimeSpeed;
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
            Time.timeScale -= Time.unscaledDeltaTime * ChangeTimeSpeed;
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
