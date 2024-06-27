using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_CommandManager : MonoBehaviour
{
    public TMP_InputField CommandInput; // Assure-toi que tu as attaché le champ de saisie dans l'inspecteur Unity.
    private bool _opened = false;
    [SerializeField] private List<RB_Items> _items = new();

    private float _defaultHpMax;
    private float _defaultHp;

    private struct DefaultItemProperties
    {
        public float DefaultAttackDamage;
        public float DefaultChargedAttackDamage;
        public float DefaultSpecialAttackDamage;
        public float? DefaultAttackCooldown;
        public float DefaultChargeAttackCooldown;
        public float DefaultSpecialAttackChargeTime;
    }

    private List<DefaultItemProperties> _defaultItemProperties = new();

    


    //Player
    private Rigidbody _playerRb;

    private void Start()
    {
        _playerRb = RB_PlayerAction.Instance.GetComponent<Rigidbody>();
        _items.AddRange(FindObjectsOfType<RB_Items>());

        
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
                RB_InputManager.Instance.InputEnabled = false;
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
        RB_InputManager.Instance.InputEnabled = true;
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
            case "/stopcamera":
                RB_Camera.Instance.GetComponentInChildren<CinemachineVirtualCamera>().Follow = null;
                break;
            case "/restartcamera":
                RB_Camera.Instance.GetComponentInChildren<CinemachineVirtualCamera>().Follow = RB_PlayerController.Instance.transform;
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
            foreach (RB_Items item in _items)
            {
                item.AttackDamage *= damageMultiplierInt;
                item.ChargedAttackDamage *= damageMultiplierInt;
                item.SpecialAttackDamage *= damageMultiplierInt;
            }
            
        }
    }

    private void GodMode()
    {
        RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax = float.MaxValue;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = float.MaxValue;
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

    private void Beginning()
    {
        _playerRb.position = RB_LevelManager.Instance.BeginningPos;
    }

    private void Weapon()
    {
        RB_Items foundItem = FindAnyObjectByType<RB_Items>();
        if (foundItem) _playerRb.position = foundItem.transform.position;
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
        RB_PlayerAction.Instance.GetComponent<RB_Health>().HpMax =  _defaultHpMax;
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = _defaultHp;
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
}