using MANAGERS;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_YogBetrayal : MonoBehaviour
{
    [SerializeField] private RB_Dialogue _1stDialogue;   // Reference to the first dialogue sequence
    [SerializeField] private RB_Dialogue _2ndDialogue;   // Reference to the second dialogue sequence
    [SerializeField] private RB_Dialogue _3rdDialogue;   // Reference to the third dialogue sequence

    [SerializeField] private GameObject _exclamationMark;   // Prefab of the exclamation mark object for transformations
    [SerializeField] private GameObject _transformationParticle;   // Particle effect for Robert's transformation

    [SerializeField] private Transform _robertLeNec;   // Transform position for Robert's transformation

    private Animator _animator;   // Reference to the Animator component

    private bool _1stDialogueDone = false;   // Flag indicating if the first dialogue is finished
    private bool _2ndDialogueDone = false;   // Flag indicating if the second dialogue is finished
    private bool _3rdDialogueDone = false;   // Flag indicating if the third dialogue is finished

    [SerializeField] private int _cinematicIndex = 0;   // Index to control animations in Animator

    private void Awake()
    {
        _animator = GetComponent<Animator>();   // Get the Animator component on Awake
        _animator.SetInteger("CinematicIndex", _cinematicIndex);   // Set the cinematic index parameter in Animator
    }

    private void Start()
    {
        _animator.Play("2-RobertTalking");   // Start Robert's talking animation
        RB_AudioManager.Instance.PlayMusic("Theme_Robert");   // Play Robert's theme music
    }

    /// <summary>
    /// Spawns an exclamation mark above the player for yog transformation.
    /// </summary>
    public void PlayerSpotMark()
    {
        Transform mark = Instantiate(_exclamationMark, RB_PlayerController.Instance.transform).transform;   // Instantiate the exclamation mark
        mark.rotation = Quaternion.identity;   // Set rotation to default
    }

    /// <summary>
    /// Initiates Robert's transformation with a particle effect.
    /// </summary>
    public void RobertTransformation()
    {
        Instantiate(_transformationParticle, _robertLeNec.position, Quaternion.identity);   // Instantiate transformation particle effect at Robert's position
    }

    /// <summary>
    /// Starts the first dialogue sequence if not already done.
    /// </summary>
    public void StartFirstDialogue()
    {
        if (!_1stDialogueDone)
        {
            _1stDialogue.StartDialogue();   // Start the first dialogue
            _1stDialogue.EventOnDialogueStopped.AddListener(FinishedFirstDialogue);   // Listen for dialogue completion event
            _1stDialogueDone = true;   // Mark first dialogue as done
        }
    }

    /// <summary>
    /// Starts the second dialogue sequence if not already done, and stops the current music.
    /// </summary>
    public void StartSecondDialogue()
    {
        if (!_2ndDialogueDone)
        {
            _2ndDialogue.StartDialogue();   // Start the second dialogue
            _2ndDialogue.EventOnDialogueStopped.AddListener(FinishedSecondDialogue);   // Listen for dialogue completion event
            RB_AudioManager.Instance.StopMusic();   // Stop playing current music
            _2ndDialogueDone = true;   // Mark second dialogue as done
        }
    }

    /// <summary>
    /// Starts the third dialogue sequence if not already done.
    /// </summary>
    public void StartThirdDialogue()
    {
        if (!_3rdDialogueDone)
        {
            _3rdDialogue.StartDialogue();   // Start the third dialogue
            _3rdDialogue.EventOnDialogueStopped.AddListener(FinishedThirdDialogue);   // Listen for dialogue completion event
            _3rdDialogueDone = true;   // Mark third dialogue as done
        }
    }

    /// <summary>
    /// Sets trigger in Animator for finishing the first dialogue sequence.
    /// </summary>
    public void FinishedFirstDialogue()
    {
        _animator.SetTrigger("FinishedFirstDialogue");   // Trigger animation for finishing first dialogue
    }

    /// <summary>
    /// Sets trigger in Animator for finishing the second dialogue sequence.
    /// </summary>
    public void FinishedSecondDialogue()
    {
        _animator.SetTrigger("FinishedSecondDialogue");   // Trigger animation for finishing second dialogue
    }

    /// <summary>
    /// Sets trigger in Animator for finishing the third dialogue sequence.
    /// </summary>
    public void FinishedThirdDialogue()
    {
        _animator.SetTrigger("FinishedThirdDialogue");   // Trigger animation for finishing third dialogue
    }

    /// <summary>
    /// Initiates transition to the next scene. (The fight with Robert LeNec).
    /// </summary>
    public void NextScene()
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);   // Transition to the next scene
    }
}
