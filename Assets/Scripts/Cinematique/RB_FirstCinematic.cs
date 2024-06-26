using MANAGERS;
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

    private void Start()
    {
        RB_LevelManager.Instance.CurrentScene = SCENENAMES.FirstCinematic;
        StartCinematic();
        RB_AudioManager.Instance.PlayMusic("Birds_Sound");
        RB_SaveManager.Instance.ResetSave();
    }

    public void StartCinematic() //after player has chosen their name
    {
        _animator.Play("1-FirstPart");
    }

    public void PlayerSpotMark() //for portal oppening
    {
        Transform mark = Instantiate(_exclamationMark, RB_PlayerController.Instance.transform).transform;
        mark.rotation = Quaternion.identity;
        RB_AudioManager.Instance.PlaySFX("rift_closing", false, false, 0f, 1f);
    }

    public void FirstRobertDialogue() //robert talk
    {
        _dialogue.StartDialogue();
        RB_AudioManager.Instance.PlayMusic("Theme_Robert");
    }

    public void RobertStoppedTalking() //for walk animation after dialogue
    {
        _animator.Play("1-AfterRobbertTalk");
        _2ndDialogue.StartDialogue();
        Invoke("NextScene", 5);
    }

    public void NextScene() //go to tutorial
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), SceneManager.GetActiveScene().buildIndex + 1);
    }
}
