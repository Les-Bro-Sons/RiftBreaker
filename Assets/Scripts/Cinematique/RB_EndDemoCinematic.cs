using UnityEngine;

public class RB_EndDemoCinematic : MonoBehaviour
{
    [SerializeField] private RB_Dialogue _dialogue;

    private void Start()
    {
        _dialogue.StartDialogue();
        _dialogue.EventOnDialogueStopped.AddListener(NextScene);
    }

    private void NextScene()
    {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 0);
        RB_SaveManager.Instance.ResetSave();
    }
}
