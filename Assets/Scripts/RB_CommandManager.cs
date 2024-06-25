using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_CommandManager : MonoBehaviour
{
    public TMP_InputField CommandInput; // Assure-toi que tu as attaché le champ de saisie dans l'inspecteur Unity.
    private bool _opened = false;

    //Player
    private Rigidbody _playerRb;

    private void Start()
    {
        _playerRb = RB_PlayerAction.Instance.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        // Vérifie si la touche "Return" (ou "Enter") est pressée pour exécuter la commande
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            if (!_opened)
            {
                print(_opened);
                _opened = true;
                CommandInput.GetComponent<CanvasGroup>().alpha = 1;
                CommandInput.Select();
                ExecuteCommand();
                RB_InputManager.Instance.MoveEnabled = false;
                RB_InputManager.Instance.AttackEnabled = false;
                RB_InputManager.Instance.DashEnabled = false;
            }
            else
            {
                CommandInput.GetComponent<CanvasGroup>().alpha = 0;
                _opened = false;
                CloseCommand(CommandInput.text);
            }
        }
        
    }

    void ExecuteCommand()
    {
        string inputText = CommandInput.text;

        // Efface le champ de saisie après avoir traité la commande, si nécessaire.
        CommandInput.text = "";
    }

    private void CloseCommand(string command)
    {
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

    private void Life(string lifeAmount)
    {
        if(int.TryParse(lifeAmount, out int lifeAmoutInt))
            RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = lifeAmoutInt;
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
        RB_PlayerAction.Instance.GetComponent<RB_Health>().Hp = float.MaxValue;
        if (RB_PlayerAction.Instance.Item)
        {
            RB_PlayerAction.Instance.Item.AttackDamage = float.MaxValue;
            RB_PlayerAction.Instance.Item.ChargedAttackDamage *= float.MaxValue;
            RB_PlayerAction.Instance.Item.SpecialAttackDamage *= float.MaxValue;
            RB_PlayerAction.Instance.Item.AttackCooldown(0);
            RB_PlayerAction.Instance.Item.ChargeAttackCooldown(0);
            RB_PlayerAction.Instance.Item.SpecialAttackChargeTime = .1f;
        }
        
    }

    private void Beginning()
    {
        _playerRb.position = RB_LevelManager.Instance.BeginningPos;
    }

    private void Weapon()
    {
        _playerRb.position = FindAnyObjectByType<RB_Items>().transform.position;
    }
}