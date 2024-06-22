using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_FirstCinematic : MonoBehaviour
{
    [SerializeField] private RB_Dialogue _dialogue;
    [SerializeField] private RB_Dialogue _2ndDialogue;

    [SerializeField] private GameObject _exclamationMark;

    private Animator _animator;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _dialogue.EventOnDialogueStopped.AddListener(RobertStoppedTalking);
    }

    public void PlayerSpotMark()
    {
        Transform mark = Instantiate(_exclamationMark, RB_PlayerController.Instance.transform).transform;
        mark.rotation = Quaternion.identity;
    }

    public void FirstRobertDialogue()
    {
        _dialogue.StartDialogue();
    }

    public void RobertStoppedTalking()
    {
        _animator.Play("1-AfterRobbertTalk");
        _2ndDialogue.StartDialogue();
        Invoke("NextScene", 5);
    }

    public void NextScene()
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);
    }
}
