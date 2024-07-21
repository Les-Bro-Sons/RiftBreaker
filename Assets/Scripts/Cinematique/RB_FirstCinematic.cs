using MANAGERS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_FirstCinematic : MonoBehaviour
{
    [SerializeField] private RB_Dialogue _dialogue;   // Reference to the first dialogue sequence
    [SerializeField] private RB_Dialogue _2ndDialogue;   // Reference to the second dialogue sequence
    [SerializeField] private GameObject _exclamationMark;   // Prefab of the exclamation mark object

    private Animator _animator;   // Reference to the Animator component

    private void Awake()
    {
        _animator = GetComponent<Animator>();   // Get the Animator component on Awake
        _dialogue.EventOnDialogueStopped.AddListener(RobertStoppedTalking);   // Listen to dialogue stop event
    }

    private void Start()
    {
        RB_LevelManager.Instance.CurrentScene = SCENENAMES.FirstCinematic;   // Set the current scene in the level manager
        StartCinematic();   // Start the initial cinematic sequence
        RB_AudioManager.Instance.PlayMusic("Birds_Sound");   // Play ambient sound
        RB_SaveManager.Instance.ResetSave();   // Reset game save data
    }

    /// <summary>
    /// Initiates the start cinematic animation.
    /// </summary>
    public void StartCinematic()
    {
        _animator.Play("1-FirstPart");   // Play the first part of the cinematic animation
    }

    /// <summary>
    /// Spawns an exclamation mark above the player for portal opening.
    /// </summary>
    public void PlayerSpotMark()
    {
        Transform mark = Instantiate(_exclamationMark, RB_PlayerController.Instance.transform).transform;   // Instantiate the exclamation mark
        mark.rotation = Quaternion.identity;   // Set rotation to default
        RB_AudioManager.Instance.PlaySFX("rift_closing", false, false, 0f, 1f);   // Play rift closing sound effect
    }

    /// <summary>
    /// Initiates Robert's first dialogue sequence.
    /// </summary>
    public void FirstRobertDialogue()
    {
        _dialogue.StartDialogue();   // Start Robert's dialogue
        RB_AudioManager.Instance.PlayMusic("Theme_Robert");   // Play Robert's theme music
    }

    /// <summary>
    /// Handles actions after Robert stops talking, including animation and starting the second dialogue.
    /// </summary>
    public void RobertStoppedTalking()
    {
        _animator.Play("1-AfterRobbertTalk");   // Play animation after Robert finishes talking
        _2ndDialogue.StartDialogue();   // Start the second dialogue sequence
        Invoke("NextScene", 5);   // Invoke next scene transition after delay
    }

    /// <summary>
    /// Initiates transition to the next scene. (here the tutorial)
    /// </summary>
    public void NextScene()
    {
        RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, SceneManager.GetActiveScene().buildIndex + 1);   // Transition to the next scene
    }
}
