using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_CommandManager : MonoBehaviour
{
    public TMP_InputField CommandInput; // Assure-toi que tu as attach� le champ de saisie dans l'inspecteur Unity.


    private void Start()
    {
        CommandInput.gameObject.SetActive(false);
        CommandInput.onDeselect.AddListener(CloseCommand);
        CommandInput.onEndEdit.AddListener(CloseCommand);
    }
    private void Update()
    {
        // V�rifie si la touche "Return" (ou "Enter") est press�e pour ex�cuter la commande.
        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            ExecuteCommand();
            RB_InputManager.Instance.MoveEnabled = false;
            RB_InputManager.Instance.AttackEnabled = false;
        }
    }

    void ExecuteCommand()
    {
        CommandInput.gameObject.SetActive(true);
        string inputText = CommandInput.text;
        CommandInput.Select();

        // Efface le champ de saisie apr�s avoir trait� la commande, si n�cessaire.
        CommandInput.text = "";
    }

    private void CloseCommand(string command)
    {
        ProcessCommand(command);
        CommandInput.gameObject.SetActive(false);
        RB_InputManager.Instance.AttackEnabled = true;
        RB_InputManager.Instance.MoveEnabled = true;
    }

    public void ProcessCommand(string input)
    {
        // Parser la commande ici
        string[] parts = input.Split(' ');
        string command = parts[0];

        // Interpr�ter la commande
        switch (command)
        {

            case "/teleport":
                TeleportPlayer(parts[1], parts[2]); // M�thode � impl�menter
                break;
            // Ajouter d'autres cas selon les besoins
            default:
                Debug.Log("Commande non reconnue");
                break;
        }
    }

    // M�thode d'exemple pour t�l�porter le joueur
    void TeleportPlayer(string x, string y)
    {
        // Logique pour t�l�porter le joueur
        float newX = float.Parse(x);
        float newY = float.Parse(y);
        // Impl�menter la t�l�portation du joueur
        Debug.Log("Teleported to (" + newX + ", " + newY + ")");
    }
}