using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_Cinematique : MonoBehaviour
{
    public GameObject Portal;  // Reference to the portal GameObject
    public GameObject PNJ;  // Reference to the PNJ (non-player character) GameObject
    public Transform Player;  // Reference to the player's Transform
    public Rigidbody PlayerRigidBody;  // Reference to the player's Rigidbody component
    public Animator _anim;  // Reference to the Animator component for animation control
    public GameObject Dial;  // Reference to the dialogue GameObject (not currently used)
    public Transform Wp;  // Waypoint towards which the player walks
    public float alphaPnj = 0f;  // Alpha value for fading in the PNJ
    private float _openGateSpeed = 0f;  // Speed at which the gate opens
    private bool _cinematiqueStart = false;  // Flag to check if the cinematic has started
    [SerializeField] private RB_Dialogue _dialogue;  // Reference to the RB_Dialogue script for dialogue handling
    public float WalkSpeed = 5;  // Speed at which the player walks

    // Start is called before the first frame update
    void Start()
    {
        Portal.SetActive(false);  // Deactivate the Portal GameObject
        PNJ.SetActive(false);  // Deactivate the PNJ GameObject
    }

    // Update is called once per frame
    void Update()
    {
        if (!_cinematiqueStart)
            StartCinematique();  // Start the cinematic sequence if it hasn't started yet
    }

    /// <summary>
    /// Method used to Start the cinematic
    /// </summary>
    void StartCinematique()
    {
        // Calculate the direction and distance to the waypoint
        Vector3 walkDirection = Wp.position - Player.position;
        float walkDistance = walkDirection.magnitude;
        walkDirection = walkDirection.normalized;

        // Move the player towards the waypoint
        PlayerRigidBody.MovePosition(Player.position + (walkDirection * WalkSpeed * Time.deltaTime));

        // Play the walk animation if the player is walking towards the waypoint
        if (walkDistance > 3)
            _anim.Play("Walk_Cinematique");
        else
        {
            _anim.Play("Player_Idle_Down");  // Play the idle animation once the player reaches the waypoint

            Portal.SetActive(true);  // Activate the Portal GameObject

            // Open the portal gate gradually
            if (_openGateSpeed < 2f)
            {
                _openGateSpeed += Time.deltaTime;
                Portal.transform.localScale = new Vector3(_openGateSpeed, 3, 1);
            }
            else
            {
                _openGateSpeed = 2f;
                Portal.transform.localScale = new Vector3(_openGateSpeed, 3, 1);
                PNJ.SetActive(true);  // Activate the PNJ GameObject once the gate is fully open
            }

            // Fade in the PNJ
            if (PNJ.activeSelf && alphaPnj < 1)
            {
                alphaPnj += Time.deltaTime;
                PNJ.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, alphaPnj);
            }

            // Start dialogue and mark the cinematic as started once the PNJ is fully visible
            if (PNJ.activeSelf && alphaPnj > 1)
            {
                PNJ.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);  // Ensure full opacity
                _dialogue.StartDialogue();  // Start the dialogue sequence
                _cinematiqueStart = true;  // Mark the cinematic as started
            }
        }
    }

    /// <summary>
    /// Method to quit the cinematic
    /// </summary>
    public void QuitCinematique()
    {
        // Transition to the next scene using a fade transition
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);
    }
}
