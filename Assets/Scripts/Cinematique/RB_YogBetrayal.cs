using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_YogBetrayal : MonoBehaviour
{
    [SerializeField] private RB_Dialogue _1stDialogue;
    [SerializeField] private RB_Dialogue _2ndDialogue;
    [SerializeField] private RB_Dialogue _3rdDialogue;

    [SerializeField] private GameObject _exclamationMark;

    private Animator _animator;

    private bool _1stDialogueDone = false;
    private bool _2ndDialogueDone = false;
    private bool _3rdDialogueDone = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.Play("2-RobertTalking");
    }

    public void PlayerSpotMark() //for yog transformation
    {
        Transform mark = Instantiate(_exclamationMark, RB_PlayerController.Instance.transform).transform;
        mark.rotation = Quaternion.identity;
    }

    public void StartFirstDialogue()
    {
        if (!_1stDialogueDone)
        {
            _1stDialogue.StartDialogue();
            _1stDialogue.EventOnDialogueStopped.AddListener(FinishedFirstDialogue);
            _1stDialogueDone = true;
        }
    }

    public void StartSecondDialogue()
    {
        if (!_2ndDialogueDone)
        {
            _2ndDialogue.StartDialogue();
            _2ndDialogue.EventOnDialogueStopped.AddListener(FinishedSecondDialogue);
            _2ndDialogueDone = true;
        }
    }

    public void StartThirdDialogue()
    {
        if (!_3rdDialogueDone)
        {
            _3rdDialogue.StartDialogue();
            _3rdDialogue.EventOnDialogueStopped.AddListener(FinishedThirdDialogue);
            _3rdDialogueDone = true;
        }
    }

    public void FinishedFirstDialogue()
    {
        _animator.SetTrigger("FinishedFirstDialogue");
    }

    public void FinishedSecondDialogue()
    {
        _animator.SetTrigger("FinishedSecondDialogue");
    }

    public void FinishedThirdDialogue()
    {
        _animator.SetTrigger("FinishedThirdDialogue");
    }

    public void NextScene() //go to tutorial
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);
    }
}
