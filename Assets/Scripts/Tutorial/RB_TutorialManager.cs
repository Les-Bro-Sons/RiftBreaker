using System.Collections;
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
    enum Attacks { normal, charge, special};

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
    private bool _shouldFadeIn = false;
    private bool _shouldFadeOut = false;
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
    public RB_Room TutorialRoom;
    public RB_Room RewindTutoRoom;
    public RB_Room StartTutoRoom;
    public RB_CollisionDetection RoomExit;
    private List<RB_FadeEffect> _fadeEffects = new List<RB_FadeEffect>();
    private List<ButtonEffect> _buttonEffects = new List<ButtonEffect>();
    private bool _rewindTutoSuccess = false;
    private bool _shouldFadeOutButton = false;
    private bool _shouldShrinkButton = false;

    //Components
    [Header("Components")]
    [SerializeField] private RB_AI_BTTree _enemyToSlowDownTimeByDistance;
    [SerializeField] private RB_Dialogue _robertLeNecRewindDialogue;
    [SerializeField] private RB_Dialogue _robertLeNecMovementDialogue;
    [SerializeField] private RB_Dialogue _robertLeNecAttackDialogue;
    [SerializeField] private RB_Dialogue _robertLeNecDeathAfterAttack;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private CanvasGroup _rewindTutoImage;
    [SerializeField] private Image _roberLeNec;
    [SerializeField] private List<CanvasGroup> _movementButtons;
    [SerializeField] private List<CanvasGroup> _attackButtons;
    [SerializeField] private RB_Katana _tutoKatana;

    //movements
    private List<Movements> _movementsPerformed = new();

    //Attacks
    private List<Attacks> _attackPerformed = new();

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        IsTuto = !RB_SaveManager.Instance.SaveObject.TutoDone;
        InitializeRewindTuto();
        InitializeMovementTuto();
        InititializeImages();
        RewindTutoRoom.CloseRoom();
    }

    private void Update()
    {
        SlowTime();
        SpeedUpTime();
        SlowTimeByEnemyDistance();
        DarkenBackground();
        BrightenBackground();
        FadeEffect();
        ButtonEffect();
    }

    #region Attack tuto

    private void InitializeAttackTuto() //Initialize the attack tutorial
    {
        if (IsTuto)
        {
            _robertLeNecAttackDialogue.StartDialogue();
            _tutoKatana.EventOnItemGathered.AddListener(OnKatanaGathered);
            RB_PlayerAction.Instance.EventBasicAttack.AddListener(delegate { OnAttackPerformed(Attacks.normal); });
            RB_PlayerAction.Instance.EventChargedAttack.AddListener(delegate { OnAttackPerformed(Attacks.charge); });
            RB_PlayerAction.Instance.EventSpecialAttack.AddListener(delegate { OnAttackPerformed(Attacks.special); });
        }
    }

    private void OnPlayerDeathAfterAttack()
    {
        print("dead");
        RewindTutoRoom.OpenRoom();
        _robertLeNecAttackDialogue.StopDialogue();
        _robertLeNecDeathAfterAttack.StartDialogue();
    }

    private void OnAttackPerformed(Attacks attackPerformed) //When an attack is performed
    {
        if (ValidateButton(_attackPerformed, attackPerformed, _attackButtons[(int)attackPerformed], 3))
        {
            AchieveAttackTuto();
        }
    }

    private void OnKatanaGathered()
    {

        RB_LevelManager.Instance.EventPlayerLost.AddListener(OnPlayerDeathAfterAttack);
        RewindTutoRoom.CloseRoom(); //Close the room
        DisplayAttackTuto();
        _robertLeNecDeathAfterAttack.StopDialogue();
    }

    private void OnKatanaGatheredAfterTutoFinished()
    {
        _robertLeNecDeathAfterAttack.StopDialogue();
    }

    private void DisplayAttackTuto() //Display the attack tutorial
    {
        foreach (CanvasGroup button in _attackButtons)
        {
            StartFadeIn(button, ChangeSpeed);
        }
    }

    private void HideAttackTuto() //Hide the attack tutorial
    {
        foreach (CanvasGroup button in _attackButtons)
        {
            StartFadeOut(button, ChangeSpeed);
        }
    }

    private void AchieveAttackTuto() //Finish the attack tutorial
    {
        HideAttackTuto();
        _robertLeNecAttackDialogue.NextDialogue();
        RB_PlayerAction.Instance.EventBasicAttack.RemoveAllListeners();
        RB_PlayerAction.Instance.EventChargedAttack.RemoveAllListeners();
        RB_PlayerAction.Instance.EventSpecialAttack.RemoveAllListeners();
        RewindTutoRoom.OpenRoom();
        TutorialRoom.OpenRoom();
        _tutoKatana.EventOnItemGathered.RemoveListener(OnKatanaGathered);
        _tutoKatana.EventOnItemGathered.AddListener(OnKatanaGatheredAfterTutoFinished);
        //Invoke(nameof(AchieveTuto), 3);
    }

    #endregion

    #region Movement tuto

    private void InitializeMovementTuto() //Initialize movement tutorial
    {
        if (IsTuto)
        {
            RB_InputManager.Instance.EventMovePerformed.AddListener(OnMovementPerformed);
            RB_InputManager.Instance.EventDashStarted.AddListener(OnDashPerformed);
            TutorialRoom.CloseRoom();
            _robertLeNecMovementDialogue.StartDialogue();
            DisplayMovementButtons();
        }
    }

    private void DisplayMovementButtons() //Display the movement tutorial buttons on the left
    {
        foreach(CanvasGroup button in _movementButtons)
        {
            StartFadeIn(button, ChangeSpeed);
        }
    }

    private void OnDashPerformed() //When the dash is performed
    {
        if(ValidateButton(_movementsPerformed, Movements.dash, _movementButtons[4], 5))
            AchieveMovementTuto();
    }

    private void OnMovementPerformed() //When a movement is performed
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

        if(ValidateButton(_movementsPerformed, movementPerformed, _movementButtons[movementPerformedIndex], 5))
            AchieveMovementTuto();
    }

    private void AchieveMovementTuto() //finish the movement tutorial
    {
        RB_InputManager.Instance.EventMovePerformed.RemoveListener(OnMovementPerformed);
        RB_InputManager.Instance.EventDashStarted.RemoveListener(OnDashPerformed);
        TutorialRoom.OpenRoom();
        foreach(CanvasGroup _movementButton in _movementButtons)
        {
            StartFadeOut(_movementButton, ChangeSpeed);
        }
        _robertLeNecMovementDialogue.StopDialogue();
    }

    #endregion

    #region Rewind tuto
    private void InitializeRewindTuto() //Initialize the rewind tutorial
    {
        _enemyToSlowDownTimeByDistance.EventOnSpotted.AddListener(InitializeTuto); //When the enemy spot the player
        RB_TimeManager.Instance.EventStartNormalRewind.AddListener(OnRewindStarted); //When the rewind is stopped
        RB_TimeManager.Instance.EventStopNormalRewind.AddListener(OnRewindStopped); //When the rewind is stopped
        RB_TimeManager.Instance.EventStopFullRewind.AddListener(OnDeathRewindFinished); //When the death rewind is finished
        RoomExit.EventOnObjectEntered.AddListener(OnPlayerExitRewindRoom); //When the player exit the rewind room
        RB_InputManager.Instance.RewindEnabled = false;
    }

    private void OnPlayerExitRewindRoom() //When the player exit the rewind room
    {
        if (RoomExit.GetDetectedObjects().Contains(RB_PlayerMovement.Instance.gameObject))
        {
            print("level exit");
            RoomExit.EventOnObjectEntered.RemoveListener(OnPlayerExitRewindRoom); //Remove listener
            InitializeAttackTuto();
        }
    }

    private void AchieveRewindTuto() //Finish the rewind tutorial
    {
        RB_InputManager.Instance.EventMovePerformed.RemoveListener(AchieveRewindTuto);
        SetNormalTime();
        StartBrightenBackground();
        StartFadeOut(_rewindTutoImage, ChangeSpeed);
        StopAnimateRobert();
        _robertLeNecRewindDialogue.StopDialogue();

    }

    private void OnRewindStoppedAfterTuto()
    {
        if (RewindTutoRoom.IsPlayerInRoom)
        {
            RewindTutoRoom.OpenRoom();
            RoomExit.EventOnObjectEntered.AddListener(delegate { RewindTutoRoom.CloseRoom(); RoomExit.EventOnObjectEntered.RemoveAllListeners(); });
        }
    }

    private void OnDeathRewindFinished() //When the rewind when the player dies is finished
    {
        AchieveRewindTuto();
    }

    private void OnRewindTutoFailed() //If the player doesn't do the rewind tutorial properly
    {
        RB_PlayerAction.Instance.RewindLeft = 3;
        _robertLeNecRewindDialogue.NextDialogue();
        TutorialRoom.OpenRoom();
    }

    private void OnRewindTutoSuccess() //When the player rewind long enough
    {
        if (!_rewindTutoSuccess)
        {
            print("rewind tuto achieved");
            _enemyToSlowDownTimeByDistance.EventOnSpotted.RemoveAllListeners(); //Stop the rewind tutorial
            RB_TimeManager.Instance.EventStartRewinding.RemoveListener(OnRewindStarted);
            RB_TimeManager.Instance.EventStopRewinding.RemoveListener(OnRewindStopped);
            AchieveRewindTuto();
            RewindTutoRoom.OpenRoom();
            _rewindTutoSuccess = true;
            _enemyToSlowDownTimeByDistance.IsStatic = false;
            RB_TimeManager.Instance.EventStopNormalRewind.AddListener(OnRewindStoppedAfterTuto);
            TutorialRoom.OpenRoom();
        }
        
    }

    public void OnRewindStarted() //When the rewind is started
    {
        StopSlowTimeByEnemyDistance();
        StopSlowtime();
        SetNormalTime();
    }

    public void OnRewindStopped() //When the player release the rewind button
    {
        StartCoroutine(OnRewindStoppedCoroutine());
    }

    private IEnumerator OnRewindStoppedCoroutine()
    {
        yield return new WaitForEndOfFrame();
        if (!_enemyToSlowDownTimeByDistance.GetBool("IsTargetSpotted")) //If the player pressed it long enough
        {
            OnRewindTutoSuccess();
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
            Time.timeScale = Vector3.Distance(RB_PlayerMovement.Instance.transform.position, _enemyToSlowDownTimeByDistance.transform.position) / _startEnemyDistance;
            if (Time.timeScale <= .5f)
            {
                StartSlowTime();
            }
        }
    }

    public void StopSlowTimeByEnemyDistance() //stop the slow down by the distance of enemy
    {
        _shouldSlowTimeByEnemyDistance = false;
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

    public void StopSlowtime() //Stop the slowing time
    {
        _shouldSlowTime = false;
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
    #endregion

    #region general tuto
    private void InititializeImages() //Initialize all the images
    {
        _backgroundImage.color = new Color(_backgroundImage.color.r, _backgroundImage.color.g, _backgroundImage.color.b, 0);
        _rewindTutoImage.alpha = 0;
    }

    private void InitializeTuto() //Initialize everything for the tutorial
    {
        StartSlowDownTimeByDistance();
        StartDarkenBackground();
        StartAnimateRobert();
        _robertLeNecRewindDialogue.StartDialogue();
        StartFadeIn(_rewindTutoImage, ChangeSpeed);
        RB_InputManager.Instance.RewindEnabled = true;
    }

    private void StartValidateButton(CanvasGroup button) //Start the animation of the validation of the buttons
    {
        var buttonEffect = GetOrCreateButtonEffect(button);
        buttonEffect.StartFadeOutAndShrink(ChangeSpeed);
    }

    private bool ValidateButton<T>(List<T> buttonValidated, T movementPerformed, CanvasGroup button, int count) //Validate the button clicked by shrinking it and fading out
    {
        bool allButtonValidated = false;
        StartValidateButton(button);

        if (!buttonValidated.Contains(movementPerformed))
            buttonValidated.Add(movementPerformed);

        if (buttonValidated.Count >= count)
        {
            allButtonValidated = true;
        }
        return allButtonValidated;

    }

    private void AchieveTuto() //Finish the tutorial 
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 1);
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

    public void StartFadeOut(CanvasGroup objectToFade, float changeSpeed) //Start the fading out
    {
        if (IsTuto)
        {
            RB_FadeEffect fadeEffect = new RB_FadeEffect(objectToFade, changeSpeed, FadeType.Out);
            _fadeEffects.Add(fadeEffect);
        }
    }

    public void FadeEffect() //Updating fading out and in effect
    {
        for (int i = _fadeEffects.Count - 1; i >= 0; i--)
        {
            _fadeEffects[i].UpdateFade();
            if (_fadeEffects[i].IsComplete)
            {
                _fadeEffects.RemoveAt(i);
            }
        }
    }

    public void StartFadeIn(CanvasGroup objectToFade, float changeSpeed) //Start the fading in
    {
        if (IsTuto)
        {
            RB_FadeEffect fadeEffect = new RB_FadeEffect(objectToFade, changeSpeed, FadeType.In);
            _fadeEffects.Add(fadeEffect);
        }
    }

    public void ButtonEffect() //Effect on the button (shrink and fade out)
    {
        for (int i = _buttonEffects.Count - 1; i >= 0; i--)
        {
            _buttonEffects[i].UpdateEffect();
            if (_buttonEffects[i].IsComplete)
            {
                _buttonEffects.RemoveAt(i);
            }
        }
    }

    private ButtonEffect GetOrCreateButtonEffect(CanvasGroup button) //Get or create the effect on the button
    {
        foreach (var effect in _buttonEffects)
        {
            if (effect.Button == button)
            {
                return effect;
            }
        }

        var newEffect = new ButtonEffect(button);
        _buttonEffects.Add(newEffect);
        return newEffect;
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

    public void StartTuto() //Start the tutorial
    {
        if (IsTuto)
        {
            _shouldDoTuto = true;
        }
    }

    public void StartSlowTime() //Start the slowing of the time
    {
        if (IsTuto)
        {
            _shouldSpeedUpTime = false;
            _shouldSlowTimeByEnemyDistance = false;
            _shouldSlowTime = true;
        }
    }
    #endregion


}

public enum FadeType
{
    In,
    Out
}




public class ButtonEffect
{
    public CanvasGroup Button { get; private set; }
    private float shrinkSpeed;
    private float fadeOutSpeed;
    private float duration;
    private float elapsedTime;
    private bool shouldFadeOutAndShrink;
    public bool IsComplete { get; private set; }

    public ButtonEffect(CanvasGroup button)
    {
        Button = button;
        IsComplete = false;
    }

    public void StartFadeOutAndShrink(float shrinkSpeed) //Start the fade out and the shrink of the object
    {
        this.shrinkSpeed = shrinkSpeed;
        elapsedTime = 0;
        shouldFadeOutAndShrink = true;
        Button.alpha = 1;
        Button.GetComponent<RectTransform>().localScale = Vector3.one;

        // Calculate duration based on the shrink speed and target scale (0.7)
        duration = (1 - 0.7f) / shrinkSpeed;

        // Calculate the fade out speed to match the duration of the shrink
        fadeOutSpeed = (1 - 0.2f) / duration;
    }

    public void UpdateEffect() //Update constantely the effect
    {
        if (shouldFadeOutAndShrink)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // Update alpha for fade out
            Button.alpha = Mathf.Lerp(1, 0.2f, progress);

            // Update scale for shrink
            var rectTransform = Button.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.7f, progress);

            if (progress >= 1.0f)
            {
                shouldFadeOutAndShrink = false;
                IsComplete = true;
            }
        }
    }
}