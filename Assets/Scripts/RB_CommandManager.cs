using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_CommandManager : MonoBehaviour
{
    public TMP_InputField CommandInput; // Assure-toi que tu as attaché le champ de saisie dans l'inspecteur Unity.
    private bool _opened = false;
    [SerializeField] private RB_Items _item;

    private float _defaultHpMax;
    private float _defaultHp;
    private float _defaultAttackDamage;
    private float _defaultChargedAttackDamage;
    private float _defaultSpecialAttackDamage;
    private float? _defaultAttackCooldown;
    private float _defaultChargeAttackCooldown;
    private float _defaultSpecialAttackChargeTime;


    //Player
    private Rigidbody _playerRb;

    private void Start()
    {
        _playerRb = RB_PlayerAction.Instance.GetComponent<Rigidbody>();
        if (_item)
        {
            _defaultAttackDamage = _item.AttackDamage;
            _defaultChargedAttackDamage = _item.ChargedAttackDamage;
            _defaultSpecialAttackDamage = _item.SpecialAttackDamage;
            _defaultAttackCooldown = _item.AttackCooldown();
            _defaultChargeAttackCooldown = _item.ChargeAttackCooldown();
            _defaultSpecialAttackChargeTime = _item.SpecialAttackChargeTime;
        }
        
    }
    private void Update()
    {
        // Vérifie si la touche "Return" (ou "Enter") est pressée pour exécuter la commande

        if (UnityEngine.Input.GetKeyDown(KeyCode.Period))
        {
            if (!_opened)
            {

                _opened = true;
                CommandInput.GetComponent<CanvasGroup>().alpha = 1;
                CommandInput.Select();
                CommandInput.onEndEdit.AddListener(CloseCommand);
                CommandInput.ActivateInputField();
                StartCoroutine(DelayStartEnd());
                ExecuteCommand();
                RB_InputManager.Instance.MoveEnabled = false;
                RB_InputManager.Instance.AttackEnabled = false;
                RB_InputManager.Instance.DashEnabled = false;
            }
        }
        
    }

    private IEnumerator DelayStartEnd()
    {
        yield return 0;
        CommandInput.MoveTextEnd(false);
    }

    void ExecuteCommand()
    {
        string inputText = CommandInput.text;

        // Efface le champ de saisie après avoir traité la commande, si nécessaire.
        CommandInput.text = "/";
    }

    private void CloseCommand(string command)
    {
        CommandInput.GetComponent<CanvasGroup>().alpha = 0;
        print("close");
        _opened = false;
        ProcessCommand(command);
        RB_InputManager.Instance.AttackEnabled = true;
        RB_InputManager.Instance.MoveEnabled = true;
        RB_InputManager.Instance.DashEnabled = true;
    }

    public void ProcessCommand(string input)
    {
        // Parser la commande ici
        string[] parts = input.Split(' ');
        string command = parts[0];

        // Interpréter la commande
        switch (command)
        {
            case "/life":
                if (parts.Length >= 2)
                    Life(parts[1]);
                break;
            case "/nextlevel":
                Level();
                break;
            case "/damage":
                if(parts.Length >= 2)
                    Damage(parts[1]);
                break;
            case "/godmode":
                GodMode();
                break;
            case "/beginning":
                Beginning(); break;
            case "/weapon":
                Weapon(); break;
            case "/resetlevel":
                ResetLevel(); break;
            case "/previouslevel":
                PreviousLevel(); break;
            case "/speed":
                if (parts.Length >= 2)
                    Speed(parts[1]);
                break;
            case "/rewind":
                if(parts.Length >= 2)
                    Rewind(parts[1]);
                break;
            case "/normal":
                Normal();
                break;
            // Ajouter d'autres cas selon les besoins
            default:
                Debug.Log("Commande non reconnue");
                break;
        }
    }

    // Méthode d'exemple pour téléporter le joueur
    void TeleportPlayer(string x, string y)
    {
        // Logique pour téléporter le joueur
        float newX = float.Parse(x);
        float newY = float.Parse(y);
        // Implémenter la téléportation du joueur
        Debug.Log("Teleported to (" + newX + ", " + newY + ")");
    }

    private void ResetLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private void PreviousLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
    }

    private void Life(string lifeAmount)
    {
        if(int.TryParse(lifeAmount, out int lifeAmoutInt))
        {
            RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = lifeAmoutInt;
            RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = lifeAmoutInt;
        }
    }

    private void Level()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Damage(string damageMultiplier)
    {
        if(int.TryParse(damageMultiplier, out int damageMultiplierInt))
        {
            RB_PlayerAction.Instance.Item.AttackDamage *= damageMultiplierInt;
            RB_PlayerAction.Instance.Item.ChargedAttackDamage *= damageMultiplierInt;
            RB_PlayerAction.Instance.Item.SpecialAttackDamage *= damageMultiplierInt;
        }
    }

    private void GodMode()
    {
        RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = float.MaxValue;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = float.MaxValue;
        if (_item)
        {
            _item.AttackDamage = float.MaxValue;
            _item.ChargedAttackDamage *= float.MaxValue;
            _item.SpecialAttackDamage *= float.MaxValue;
            _item.AttackCooldown(0);
            _item.ChargeAttackCooldown(0);
            _item.SpecialAttackChargeTime = .1f;
        }
        
    }

    private void Beginning()
    {
        _playerRb.position = RB_LevelManager.Instance.BeginningPos;
    }

    private void Weapon()
    {
        RB_Items foundItem = FindAnyObjectByType<RB_Items>();
        print(foundItem.name);
        _playerRb.position = foundItem.transform.position;
    }

    private void Speed(string speed)
    {
        if(int.TryParse(speed, out int speedInt))
            RB_PlayerMovement.Instance.MovementMaxSpeed = speedInt;
    }

    private void Rewind(string rewindAmount)
    {
        if(int.TryParse(rewindAmount, out int rewindAmountInt))
        {
            RB_PlayerAction.Instance.RewindLeft = rewindAmountInt;
        }
    }

    private void Normal()
    {
        RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = _defaultHpMax;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = _defaultHp;
        if (_item)
        {
            _item.AttackDamage = _defaultAttackDamage;
            _item.ChargedAttackDamage = _defaultChargedAttackDamage;
            _item.SpecialAttackDamage = _defaultSpecialAttackDamage;
            _item.AttackCooldown(_defaultAttackCooldown);
            _item.ChargeAttackCooldown(_defaultAttackCooldown);
            _item.SpecialAttackChargeTime = _defaultSpecialAttackChargeTime;
        }
    }
}