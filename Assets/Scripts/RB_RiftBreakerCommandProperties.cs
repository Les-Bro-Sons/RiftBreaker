using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_RiftBreakerCommandProperties : MonoBehaviour
{
    //Instance
    public static RB_RiftBreakerCommandProperties Instance; //Make an instance of this object


    //Time
    private struct TimeModifier //Everything needed to stop and resume time
    {
        public string Id;
        public float TargetTimeScale;
        public int Priority;
    }

    private static TimeModifier _commandManagerTimeModifier = new TimeModifier() //Current time modifier
    {
        Id = "consolePause",
        TargetTimeScale = 0f,
        Priority = 1001,
    };

    //Items
    [HideInInspector]public RB_Items FoundItem; //The item found on the scene
    [HideInInspector]public struct DefaultItemPropertiesStruct //All of the item properties (damage and cooldown)
    {
        public float DefaultAttackDamage;
        public float DefaultChargedAttackDamage;
        public float DefaultSpecialAttackDamage;
        public float? DefaultAttackCooldown;
        public float DefaultChargeAttackCooldown;
        public float DefaultSpecialAttackChargeTime;
    }

    [HideInInspector]public List<RB_Items> ItemsOnScene = new(); //All of the items on the scene
    [HideInInspector]public List<DefaultItemPropertiesStruct> DefaultItemProperties = new(); //The current scene default properties
    [HideInInspector]public float LastDamageMultiplier; //The last damage multiplier
    
    //Player
    [HideInInspector]public Rigidbody PlayerRb; //The player rigidbody
    //All of the default player properties
    [HideInInspector]public float DefaultHp;
    [HideInInspector]public float DefaultSpeed;
    [HideInInspector]public int DefaultRewindAmount;
    [HideInInspector]public float LastSpeed;
    [HideInInspector]public int LastRewindAmount;
    [HideInInspector]public float LastHp;

    //Bools
    [HideInInspector]public bool IsGodMode = false; //If the player is in god mode
    [HideInInspector]public bool IsLoadingNewScene = false; //If the game is currently loading a new scene
    private bool _noPlayerInScene = false;

    //Debug
    [Header("Debug")]
    public GameObject GodModeActivatedFeedbackDebugImage; //God mode debug feedback

    //Awake
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //Start
    private void Start()
    {
        InitProperties();
        //All of the event needed
        RB_InputManager.Instance.ConsoleToggleInputEvent.AddListener(OnConsoleToggleInput);
        RB_MenuInputManager.Instance.EventPauseStarted.AddListener(OnPauseStarted);
        SceneManager.activeSceneChanged += OnChangeScene;
        RB_CommandManager.Instance.OpenConsoleEvent.AddListener(OnOpenconsole);
        RB_CommandManager.Instance.CloseConsoleEvent.AddListener(OnCloseConsole);
    }

    private void OnDestroy()
    {
        //Unsuscribe every events
        RB_InputManager.Instance.ConsoleToggleInputEvent.RemoveListener(OnConsoleToggleInput);
        RB_MenuInputManager.Instance.EventPauseStarted.RemoveListener(OnPauseStarted);
        SceneManager.activeSceneChanged -= OnChangeScene;
        RB_CommandManager.Instance.OpenConsoleEvent.RemoveListener(OnOpenconsole);
        RB_CommandManager.Instance.CloseConsoleEvent.RemoveListener(OnCloseConsole);
    }

    /// <summary>
    /// This function re enable the input of the player after a frame
    /// </summary>
    public IEnumerator DelayedReEnableInput()
    {
        yield return 0;
        RB_InputManager.Instance.InputEnabled = true; //Re enable the player inputs
    }

    /// <summary>
    /// This function is called when the input of the console input is used and toggle the console
    /// </summary>
    public void OnConsoleToggleInput() 
    {
        RB_CommandManager.Instance.ToggleConsole();
    }

    /// <summary>
    /// This function is called when the pause is started and close the console
    /// </summary>
    public void OnPauseStarted() 
    {
        RB_CommandManager.Instance.CloseConsole();
    }

    /// <summary>
    /// This function is called when the console is closed and resume the time and re enable the player input after a delay
    /// </summary>
    private void OnCloseConsole() 
    {
        ResumeTime();
        StartCoroutine(DelayedReEnableInput());
    }

    /// <summary>
    /// This function resume the time
    /// </summary>
    private void ResumeTime()
    {
        RB_TimescaleManager.Instance.RemoveModifier(_commandManagerTimeModifier.Id); //Remove the time modifier so it gets back to the previous one
    }

    /// <summary>
    /// This function is called when the console is opened
    /// </summary>
    private void OnOpenconsole() 
    {
        StopInputsAndTime();
    }

    /// <summary>
    /// This function stop the time and disable the player inputs
    /// </summary>
    private void StopInputsAndTime()
    {
        RB_InputManager.Instance.InputEnabled = false; //Disable the player inputs
        RB_TimescaleManager.Instance.SetModifier(gameObject, _commandManagerTimeModifier.Id, _commandManagerTimeModifier.TargetTimeScale, _commandManagerTimeModifier.Priority); //Set a time scale modifier
    }

    /// <summary>
    /// This function is called when the scene is changed
    /// </summary>
    /// <param name="currentScene"> The previous scene </param>
    /// <param name="nextScene"> The next scene </param>
    private void OnChangeScene(Scene currentScene, Scene nextScene)
    {
        IsLoadingNewScene = false; //Finish the loading of the scene
        KeepStateThroughScenes();
    }

    /// <summary>
    /// This function initialize the properties
    /// </summary>
    /// <returns> If the properties are initialized </returns>
    public bool InitProperties()
    {
        if (RB_PlayerAction.Instance == null)//If there's no player on the scene (exemple : on the main menu)
        {
            _noPlayerInScene = true;
            return false; 
        }
        _noPlayerInScene = false;
        FoundItem = FindAnyObjectByType<RB_Items>(); //Set the found item
        GodModeActivatedFeedbackDebugImage.SetActive(false); //Debug feedback god mode
        DefaultHp = LastHp = RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax; //Set the default hp and last hp to the real default
        DefaultSpeed = LastSpeed = RB_PlayerMovement.Instance.MovementMaxSpeed; //Set the default speed and last speed to the real default
        DefaultRewindAmount = LastRewindAmount = RB_PlayerAction.Instance.RewindLeft; //Set the default rewind amount and last rewind amount to the real default
        LastDamageMultiplier = 1; //Set last damage multiplier to one (default)
        IsGodMode = false; //Set the player to not god mode
        return true;
    }

    /// <summary>
    /// This function keeps the states like god modes or any stats of the player through levels
    /// </summary>
    private void KeepStateThroughScenes()
    {
        if (_noPlayerInScene && !InitProperties()) return;  //If the game started on a scene with no player, while there's no player in scene retry to get a player
        PlayerRb = RB_PlayerAction.Instance.GetComponent<Rigidbody>(); //Get the rigidbody of the player
        ItemsOnScene = FindObjectsOfType<RB_Items>().ToList(); //Get the Items on the scene


        DefaultItemProperties.Clear(); //Clear the default properties so they don't duplicate
        foreach (RB_Items item in ItemsOnScene) //For each item on the scene
        {
            DefaultItemPropertiesStruct properties = new DefaultItemPropertiesStruct //Set the default properties (damage and cooldown)
            {
                DefaultAttackDamage = item.AttackDamage,
                DefaultChargedAttackDamage = item.ChargedAttackDamage,
                DefaultSpecialAttackDamage = item.SpecialAttackDamage,
                DefaultAttackCooldown = item.AttackCooldown(),
                DefaultChargeAttackCooldown = item.ChargeAttackCooldown(),
                DefaultSpecialAttackChargeTime = item.SpecialAttackChargeTime,
            };
            DefaultItemProperties.Add(properties); //Add it to the default item properties list
        }
        if (IsGodMode) //If the player was on god mode set it back
            RB_Commands.CmdGodMode();
        else //Otherwise set it back to normal
            RB_Commands.CmdNormal();
        //Set the player the last stats
        RB_Commands.CmdSpeed(new string[] { LastSpeed.ToString() });
        RB_Commands.CmdRewind(new string[] { LastRewindAmount.ToString() });
        RB_Commands.CmdLife(new string[] { LastHp.ToString() });
        RB_Commands.CmdDamage(new string[] { LastDamageMultiplier.ToString() });
    }

    
}
