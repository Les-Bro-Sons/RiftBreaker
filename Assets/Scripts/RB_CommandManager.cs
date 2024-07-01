using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_CommandManager : MonoBehaviour
{
    //Instance
    public static RB_CommandManager Instance; //An instance of this object

    //Components
    public TMP_InputField CommandInput; //The input field that will be used to collect the commands
    public CanvasGroup CommandCanvasGroup; //A canvas group to make the commandInput appear and dispaear

    //Commands
    private Dictionary<string, MethodInfo> _commands = new(); //All the commands available (Filled later in the code)

    //Bools
    private bool _opened = false; //If the console is opened or not

    //Events
    [HideInInspector] public UnityEvent OpenConsoleEvent; //Event called when the console is opened
    [HideInInspector] public UnityEvent CloseConsoleEvent; //Event called when the console is closed

    //Awake
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    //Start
    private void Start()
    {
        InitCommands();
    }

    /// <summary>
    /// This function initializes all of the commands available in the script of type "Type"
    /// </summary>
    public void InitCommands()
    {
        Type type = typeof(RB_Commands); //Get the type of the script that contains the commands (to change with the name of your commands container)
        foreach (var method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)) //Get every methods of the RB_Commands class with the flags nonpublic instance public and static
        {
            if (method.Name.StartsWith("Cmd")) //If the method start with cmd it means it's a command function
            {
                _commands.Add(method.Name.Substring(3).ToLower(), method); //Add it to the commands list with command that you have to enter into the console and method to call
            }
        }
    }

    /// <summary>
    /// This function opens the console if it's closed and close it if it's opened
    /// </summary>
    public void ToggleConsole()
    {
        if (RB_PauseMenu.Instance.IsPaused) return;
        CommandInput.text = string.Empty;
        if (_opened)
            CloseConsole();
        else
            OpenConsole();
    }

    /// <summary>
    /// This function opens the console by displaying, selecting and activating it
    /// </summary>
    public void OpenConsole()
    {
        _opened = true;
        CommandInput.text = string.Empty; //Reset the text
        CommandCanvasGroup.alpha = 1; //Display the console
        CommandInput.Select(); //Select the console so the player doesn't have to
        CommandInput.ActivateInputField(); //Active the input field so the player type in
        CommandInput.onSubmit.AddListener(OnSubmit);
        OpenConsoleEvent?.Invoke(); //Call the the open console event for any other script
        
    }



    public void OnSubmit(string command) //When the player submit something
    {
        SubmitCommand();
    }

    /// <summary>
    /// This function process the command that the player wrote and close the console
    /// </summary>
    public void SubmitCommand()
    {
        CommandInput.onSubmit.RemoveListener(OnSubmit);
        ProcessCommand(CommandInput.text); //Process the command entered by the player that is stocked in the CommandInput text
        CloseConsole(); //Close the console after processing it
    }

    /// <summary>
    /// This function closes the console by undisplaying, releasing and deactivating it
    /// </summary>
    public void CloseConsole()
    {
        _opened = false;
        CommandCanvasGroup.alpha = 0; //Undisplay the console
        CommandInput.ReleaseSelection(); //Release the commande so the navigation is no longer locked
        CommandInput.DeactivateInputField(); //Deactivate the input field so the navigation is no longer locked
        CommandInput.text = string.Empty; //Emptying the text
        CloseConsoleEvent?.Invoke();
    }

    /// <summary>
    /// This function process the command entered by the player by invoking the right function stored in another script
    /// </summary>
    /// <param name="input"> The command eneter by the player </param>
    public void ProcessCommand(string input)
    {
        string[] parts = input.Split(' '); //Get all the parts of the command entered
        string command = parts[0]; //Get the actual command
        string[] args = parts.Skip(1).ToArray(); //And its argumentes

        if (_commands.TryGetValue(command, out MethodInfo method)) //Try to get a method with the command entered, if the method exist get in a variable called method
        {
            method.Invoke(this, new object[] { args }); //invoke that method with arguments got before
        }
        else //if the method doesn't exit, return an "error message"
            Debug.Log("Commande non reconnue");
    }

    
}