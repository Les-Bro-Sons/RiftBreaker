using System.Collections.Generic;
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
    //Enums
    enum Movements { up, left, right, down, dash};

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
    private Vector3 _tutoPos;
    private bool _positionGot = false;

    //Properties
    public float ChangeSpeed;
    public float RobertMoveHeight;
    public float RobertMoveSpeed;
    public float MinimumRewindTime;
    public RB_Room _tutorialRoom;

    //Components
    [SerializeField] private RB_AI_BTTree EnemyToSlowDownTimeByDistance;
    [SerializeField] private RB_Dialogue _robertLeNecRewindDialogue;
    [SerializeField] private RB_Dialogue _robertLeNecMovementDialogue;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _rewindTutoImage;
    [SerializeField] private Image _roberLeNec;
    [SerializeField] private List<CanvasGroup> _movementButtons;
    private Transform robertTransform;

    //Rewind
    private float _startRewindTime;

    //movements
    private List<Movements> _movementsPerformed = new();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        robertTransform = _roberLeNec.GetComponent<RectTransform>();
    }

    private void Start()
    {
        IsTuto = !RB_SaveManager.Instance.SaveObject.TutoDone;
        InitializeRewindTuto();
        InitializeMovementTuto();
        _startRobertPos = robertTransform.localPosition;
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

    #region Movement tuto

    private void InitializeMovementTuto()
    {
        if (IsTuto)
        {
            RB_InputManager.Instance.EventMovePerformed.AddListener(OnMovementPerformed);
            RB_InputManager.Instance.EventDashStarted.AddListener(OnDashPerformed);
            _tutorialRoom.CloseRoom();
            _robertLeNecMovementDialogue.StartDialogue();
        }
    }

    private void OnDashPerformed()
    {
        if (!_movementsPerformed.Contains(Movements.dash))
            _movementsPerformed.Add(Movements.dash);
        _movementButtons[4].alpha = .2f;
        _movementButtons[4].GetComponent<RectTransform>().localScale = Vector3.one * .7f;
        if (_movementsPerformed.Count >= 5)
        {
            AchieveMovementTuto();
        }
    }

    private void OnMovementPerformed()
    {
        Movements movementPerformed = new();
        int movementPerformedIndex = 0;
        if (RB_InputManager.Instance.MoveValue.y > 0)
        {
            movementPerformed = Movements.up;
            movementPerformedIndex = 0;
        }
        else if (RB_InputManager.Instance.MoveValue.y < 0)
        {
            movementPerformed = Movements.down;
            movementPerformedIndex = 1;
        }
        else if (RB_InputManager.Instance.MoveValue.x < 0)
        {
            movementPerformed = Movements.left;
            movementPerformedIndex = 2;
        }
        else if (RB_InputManager.Instance.MoveValue.x > 0)
        {
            movementPerformed = Movements.right;
            movementPerformedIndex = 3;
        }
        

        _movementButtons[movementPerformedIndex].alpha = .2f;
        _movementButtons[movementPerformedIndex].GetComponent<RectTransform>().localScale = Vector3.one * .7f;

        if (!_movementsPerformed.Contains(movementPerformed))
            _movementsPerformed.Add(movementPerformed);
        if(_movementsPerformed.Count >= 5)
        {
            AchieveMovementTuto();
        }
    }

    private void AchieveMovementTuto()
    {
        RB_InputManager.Instance.EventMovePerformed.RemoveListener(OnMovementPerformed);
        _tutorialRoom.OpenRoom();
        foreach(CanvasGroup _movementButton in _movementButtons)
        {
            _movementButton.gameObject.SetActive(false);
        }
        _robertLeNecMovementDialogue.StopDialogue();
    }

    #endregion

    #region Rewind tuto
    private void InitializeRewindTuto() 
    {
        EnemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(InitializeTuto); //When the enemy spot the player
        RB_TimeManager.Instance.EventStartRewinding.AddListener(OnRewindStarted); //When the rewind is stopped
        RB_TimeManager.Instance.EventStopRewinding.AddListener(OnRewindStopped); //When the rewind is stopped
        RB_TimeManager.Instance.EventStopFullRewind.AddListener(OnDeathRewindFinished); //When the death rewind is finished
    }

    private void AchieveRewindTuto()
    {
        RB_InputManager.Instance.EventMovePerformed.RemoveListener(AchieveRewindTuto);
        SetNormalTime();
        StartBrightenBackground();
        StartFadeOut();
        StopAnimateRobert();
        _robertLeNecRewindDialogue.StopDialogue();
    }

    private void OnDeathRewindFinished()
    {
        RB_InputManager.Instance.EventMovePerformed.AddListener(AchieveRewindTuto);
    }

    private void OnRewindTutoFailed() //If the player doesn't do the rewind tutorial properly
    {
        RB_UxHourglass.Instance.CreateMaxNumberOfHourglass(); //Add one rewind to the player
        _robertLeNecRewindDialogue.NextDialogue();
    }

    public void OnRewindStarted()
    {
        _startRewindTime = Time.unscaledTime; //Set the time when the rewind started
    }

    public void OnRewindStopped() //When the player release the rewind button
    {
        if (_startRewindTime + MinimumRewindTime < Time.unscaledTime) //If the player pressed it long enough
        {
            print("rewind tuto achieved");
            EnemyToSlowDownTimeByDistance.EventOnSpotted.RemoveListener(InitializeTuto); //Stop the rewind tutorial
            RB_TimeManager.Instance.EventStartRewinding.RemoveListener(OnRewindStarted);
            RB_TimeManager.Instance.EventStopRewinding.RemoveListener(OnRewindStopped);
        }
        else //If the player didn't press long enough
        {
            OnRewindTutoFailed();
        }
    }

    private void GetRewindFirstPos() //Get the position of the player when he started the rewind
    {
        if (!_positionGot)
        {
            _tutoPos = RB_PlayerMovement.Instance.transform.position;
            _positionGot = true;
        }
    }
    #endregion

    #region time managing
    public void SetNormalTime() //Set the time scale back to the normal one
    {
        if (IsTuto)
        {
            Time.timeScale = 1.0f;
        }
    }

    private void StopTime() //Set the time scale to 0
    {
        if (IsTuto)
        {
            Time.timeScale = 0f;
        }
    }

    public void StartSlowDownTimeByDistance() //Start the slow down of the time by the tuto enemy
    {
        if (IsTuto)
        {
            _startEnemyDistance = 10;
            _shouldSlowTimeByEnemyDistance = true;
        }
    }

    public void SlowTimeByEnemyDistance() //Slow down time by the distance of the tutorial enemy and the player
    {
        if (IsTuto && _shouldSlowTimeByEnemyDistance)
        {
            Time.timeScale = Vector3.Distance(RB_PlayerMovement.Instance.transform.position, EnemyToSlowDownTimeByDistance.transform.position) / _startEnemyDistance;
            if (Time.timeScale <= .5f)
            {
                StartSlowTime();
            }
        }
    }

    public void StartSpeedUpTime() //Start speeding up time to the normal one
    {
        if (IsTuto)
        {
            _shouldSlowTime = false;
            _shouldSlowTimeByEnemyDistance = false;
            _shouldSpeedUpTime = true;

        }
    }

    public void SpeedUpTime() //Speed up time to the normal one
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

    public void SlowTime() //Slow the time untill it reaches 0
    {
        if (IsTuto && _shouldSlowTime)
        {
            Time.timeScale -= Time.unscaledDeltaTime * ChangeSpeed;
            if (Time.timeScale <= 0.1)
            {
                Time.timeScale = 0;
                _shouldSlowTime = false;
                GetRewindFirstPos();
            }
        }
    }
    #endregion

    #region Robert
    public void StartAnimateRobert() //Start the animation of robert
    {
        if (IsTuto)
        {
            _shouldAnimateRobertLeNec = true;
        }
    }

    public void StopAnimateRobert() //Stop the animation of robert
    {
        if (IsTuto)
        {
            _shouldAnimateRobertLeNec = false;
        }
    }

    public void AnimateRobert() //Make robert go up then down with a sin
    {
        if (IsTuto && _shouldAnimateRobertLeNec)
        {
            print("animate robert");
            robertTransform.localPosition = new Vector3(robertTransform.localPosition.x, _startRobertPos.y + RobertMoveHeight * Mathf.Sin(Time.unscaledTime * RobertMoveSpeed), robertTransform.localPosition.z);
        }
    }
    #endregion

    #region general tuto
    private void InititializeImages() //Initialize all the images
    {
        _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, 0);
        _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
        _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
    }

    private void InitializeTuto() //Initialize everything for the tutorial
    {
        StartSlowDownTimeByDistance();
        StartDarkenBackground();
        StartFadeIn();
        StartAnimateRobert();
        _robertLeNecRewindDialogue.StartDialogue();
    }

    private void AchieveTuto() //Finish the tutorial 
    {
        AchieveRewindTuto();
    }

    public void StartBrightenBackground() //start the background brightning
    {
        if (IsTuto)
        {
            _shouldBrightenBackground = true;
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, .6f);
        }
    }

    public void BrightenBackground() //Brighten the background
    {
        if (IsTuto && _shouldBrightenBackground)
        {
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, _backgroundImage.color.a - Time.unscaledDeltaTime * ChangeSpeed);
            if (_backgroundImage.color.a <= 0)
            {
                _shouldBrightenBackground = false;
            }
        }
    }

    public void StartFadeOut() //Start the fade out of everything that have to be faded out
    {
        if (IsTuto)
        {
            _shouldFadeOutRewindTuto = true;
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 1);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 1);
        }
    }

    public void FadeOut() //Fade out of everything that have to be faded out
    {
        if (IsTuto && _shouldFadeOutRewindTuto)
        {
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, _rewindTutoImage.color.a - Time.unscaledDeltaTime * ChangeSpeed);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, _rewindTutoImage.color.a - Time.unscaledDeltaTime * ChangeSpeed);
            if (_rewindTutoImage.color.a <= 0)
            {
                _shouldFadeOutRewindTuto = false;
            }
        }
    }

    public void StartFadeIn() //Start fade in of everything that have to be faded out
    {
        if (IsTuto)
        {
            _shouldFadeInRewindTuto = true;
            _rewindTutoImage.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
            _roberLeNec.color = new Color(_rewindTutoImage.color.r, _rewindTutoImage.color.g, _rewindTutoImage.color.b, 0);
        }
    }

    public void FadeIn() //Fade in of everything that have to be faded out
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

    public void StartDarkenBackground() //Start the darkening of the background
    {
        if (IsTuto)
        {
            _shouldDarkenBackground = true;
            _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, 0);
        }
    }

    public void DarkenBackground() //Darken the background
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
    #endregion



































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
