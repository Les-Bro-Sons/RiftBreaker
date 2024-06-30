using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_CommandManager : MonoBehaviour
{
    //Instance
    RB_CommandManager Instance;

    //Components
    public TMP_InputField CommandInput; // Assure-toi que tu as attaché le champ de saisie dans l'inspecteur Unity.

    //Items
    private struct DefaultItemProperties
    {
        public float DefaultAttackDamage;
        public float DefaultChargedAttackDamage;
        public float DefaultSpecialAttackDamage;
        public float? DefaultAttackCooldown;
        public float DefaultChargeAttackCooldown;
        public float DefaultSpecialAttackChargeTime;
    }
    private List<RB_Items> _items = new();
    private List<DefaultItemProperties> _defaultItemProperties = new();
    private float _lastDamageMultiplier;

    //Time
    private struct TimeModifier
    {
        public string Id;
        public float TargetTimeScale;
        public int Priority;
    }

    private static TimeModifier _commandManagerTimeModifier = new TimeModifier()
    {
        Id = "consolePause",
        TargetTimeScale = 0f,
        Priority = 1001,
    };
    

    //Commands
    private Dictionary<string, MethodInfo> _commands = new();

    //Player
    private Rigidbody _playerRb;
    private float _defaultHp;
    private float _defaultSpeed;
    private int _defaultRewindAmount;
    private float _lastSpeed;
    private int _lastRewindAmount;
    private float _lastHp;

    //Bools
    private bool _opened = false;
    private bool _isGodMode = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        RB_InputManager.Instance.ConsoleToggledEvent.AddListener(OnConsoleToggled);
        RB_MenuInputManager.Instance.EventPauseStarted.AddListener(OnPauseStarted);

        
        InitProperties();
        SceneManager.activeSceneChanged += OnChangeScene;
    }

    private void InitProperties()
    {
        foreach (var method in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)) //Get every methods with the flags nonpublic instance and public
        {
            if (method.Name.StartsWith("Cmd")) //If the method start with cmd it means it's a command function
            {
                _commands.Add(method.Name.Substring(3).ToLower(), method); //Add it to the commands list with command that you have to enter into the console and method to call
            }
        }
        _defaultHp = _lastHp = RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax;
        _defaultSpeed = _lastSpeed = RB_PlayerMovement.Instance.MovementMaxSpeed;
        _defaultRewindAmount = _lastRewindAmount = RB_PlayerAction.Instance.RewindLeft;
        _lastDamageMultiplier = 1;
        KeepStateThroughScenes();
    }

    private void OnChangeScene(Scene currentScene, Scene nextScene)
    {
        KeepStateThroughScenes();
        print("init properties");
    }

    private void KeepStateThroughScenes()
    {
        _playerRb = RB_PlayerAction.Instance.GetComponent<Rigidbody>();
        _items = FindObjectsOfType<RB_Items>().ToList();

        _defaultItemProperties.Clear();
        foreach (RB_Items item in _items)
        {
            DefaultItemProperties properties = new DefaultItemProperties
            {
                DefaultAttackDamage = item.AttackDamage,
                DefaultChargedAttackDamage = item.ChargedAttackDamage,
                DefaultSpecialAttackDamage = item.SpecialAttackDamage,
                DefaultAttackCooldown = item.AttackCooldown(),
                DefaultChargeAttackCooldown = item.ChargeAttackCooldown(),
                DefaultSpecialAttackChargeTime = item.SpecialAttackChargeTime,
            };
            _defaultItemProperties.Add(properties);
        }
        
        if (_isGodMode)
            CmdGodMode();
        else
            CmdNormal();
        CmdSpeed(new string[] { _lastSpeed.ToString() });
        CmdRewind(new string[] { _lastRewindAmount.ToString() });
        CmdLife(new string[] { _lastHp.ToString() });
        CmdDamage(new string[] { _lastDamageMultiplier.ToString()});
    }

    private void OnPauseStarted()
    {
        CloseConsole();
    }

    private void OnConsoleToggled()
    {
        if (RB_PauseMenu.Instance.IsPaused) return;
        if (_opened)
            CloseConsole();
        else
            OpenConsole();
    }

    private void OpenConsole()
    {
        _opened = true;
        CommandInput.GetComponent<CanvasGroup>().alpha = 1;
        CommandInput.Select();
        RB_MenuInputManager.Instance.EventSubmitStarted.AddListener(OnSubmitStarted);
        CommandInput.ActivateInputField();
        CommandInput.text = string.Empty;
        RB_InputManager.Instance.InputEnabled = false;
        RB_TimescaleManager.Instance.SetModifier(gameObject, _commandManagerTimeModifier.Id, _commandManagerTimeModifier.TargetTimeScale, _commandManagerTimeModifier.Priority);
    }

    private void OnSubmitStarted()
    {
        RB_MenuInputManager.Instance.EventSubmitStarted.RemoveListener(OnSubmitStarted);
        ProcessCommand(CommandInput.text);
        CloseConsole();
    }

    private void CloseConsole()
    {
        _opened = false;
        StartCoroutine(DelayedReEnableInput());
        CommandInput.GetComponent<CanvasGroup>().alpha = 0;
        RB_TimescaleManager.Instance.RemoveModifier(_commandManagerTimeModifier.Id);
    }

    private IEnumerator DelayedReEnableInput()
    {
        yield return 0;
        RB_InputManager.Instance.InputEnabled = true;
    }

    public void ProcessCommand(string input)
    {
        string[] parts = input.Split(' ');
        string command = parts[0];
        string[] args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(command, out MethodInfo method)) //Try to get a method with the command entered, if the method exist get in a variable called method
        {
            method.Invoke(this, new object[] { args }); //invoke that method with arguments got before
        }
        else //if the method doesn't exit, return an "error message"
            Debug.Log("Commande non reconnue");
    }

    #region commands

    private void CmdResetLevel(string[] args = null)
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void CmdPreviousLevel(string[] args = null)
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void CmdLife(string[] args = null)
    {
        if (int.TryParse(args[0], out int lifeAmoutInt))
        {
            _lastHp = lifeAmoutInt;
            RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = lifeAmoutInt;
            RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = lifeAmoutInt;

        }
    }

    private void CmdNextLevel(string[] args = null)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void CmdDamage(string[] args = null)
    {
        if (int.TryParse(args[0], out int damageMultiplierInt))
        {
            _lastDamageMultiplier = damageMultiplierInt;
            foreach (RB_Items item in _items)
            {
                item.AttackDamage *= damageMultiplierInt;
                item.ChargedAttackDamage *= damageMultiplierInt;
                item.SpecialAttackDamage *= damageMultiplierInt;
            }
            
        }
    }

    private void CmdGodMode(string[] args = null)
    {
        _isGodMode = true;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = float.MaxValue;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = float.MaxValue;
        _lastHp = float.MaxValue;
        foreach (RB_Items item in _items)
        {
            item.AttackDamage = float.MaxValue;
            item.ChargedAttackDamage *= float.MaxValue;
            item.SpecialAttackDamage *= float.MaxValue;
            item.AttackCooldown(0);
            item.ChargeAttackCooldown(0);
            item.SpecialAttackChargeTime = .1f;
        }
        
    }

    private void CmdBeginning(string[] args = null)
    {
        _playerRb.position = RB_LevelManager.Instance.BeginningPos;
    }

    private void CmdWeapon(string[] args = null)
    {
        RB_Items foundItem = FindAnyObjectByType<RB_Items>();
        if (foundItem) _playerRb.position = foundItem.transform.position;
    }

    private void CmdSpeed(string[] args = null)
    {
        if (int.TryParse(args[0], out int speedInt))
        {
            RB_PlayerMovement.Instance.MovementMaxSpeed = speedInt;
            _lastSpeed = speedInt;
        }
    }

    private void CmdRewind(string[] args = null)
    {
        if (int.TryParse(args[0], out int rewindAmountInt))
        {
            _lastRewindAmount = rewindAmountInt;
            RB_PlayerAction.Instance.RewindLeft = rewindAmountInt;
        }
    }

    private void CmdNormal(string[] args = null)
    {
        _isGodMode = true;
        _lastHp = _defaultHp;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = _defaultHp;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = _defaultHp;
        RB_PlayerMovement.Instance.MovementMaxSpeed = _defaultSpeed;
        RB_PlayerAction.Instance.RewindLeft = _defaultRewindAmount;
        for(int i = 0; i < _items.Count; i++)
        {
            _items[i].AttackDamage = _defaultItemProperties[i].DefaultAttackDamage;
            _items[i].ChargedAttackDamage = _defaultItemProperties[i].DefaultChargedAttackDamage;
            _items[i].SpecialAttackDamage = _defaultItemProperties[i].DefaultSpecialAttackDamage;
            _items[i].AttackCooldown(_defaultItemProperties[i].DefaultAttackCooldown);
            _items[i].ChargeAttackCooldown(_defaultItemProperties[i].DefaultAttackCooldown);
            _items[i].SpecialAttackChargeTime = _defaultItemProperties[i].DefaultSpecialAttackChargeTime;
        }
    }
    #endregion
}