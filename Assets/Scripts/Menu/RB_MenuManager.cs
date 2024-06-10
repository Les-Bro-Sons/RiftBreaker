 using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RB_MenuManager : MonoBehaviour {

    public static RB_MenuManager Instance;

    public bool IsOptionOpen;

    public Animator Animator;

    enum MENUSTATE { 
        Audio, Video, Control
    }

    MENUSTATE _menuState;


    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        Animator = GetComponent<Animator>();
        Animator.updateMode = AnimatorUpdateMode.UnscaledTime;

    }
     
    public void Play() {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString());
    }

    public void MainMenu() {
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString());
    }

    public void Options() {
        Animator.SetBool("IsOptionOpen", true);
        IsOptionOpen = true;
        _menuState = MENUSTATE.Audio;
    }

    public void CloseOption() {
        Debug.Log("AAAAAA");
        IsOptionOpen = false;
    }

    public void OptionAudio() {
        if(_menuState != MENUSTATE.Audio) { 
            Animator.SetTrigger("Audio");
            _menuState = MENUSTATE.Audio;
        }
    }
    public void OptionVideo() {
        if (_menuState != MENUSTATE.Video) { 
            Animator.SetTrigger("Video");
            _menuState= MENUSTATE.Video;
        }

    }
    public void OptionControl() {
        if (_menuState != MENUSTATE.Control) { 
            Animator.SetTrigger("Control");
            _menuState = MENUSTATE.Control;
        }
    }

    public void Credits() { 
    
    }

    public void Quit() {
        Animator.SetBool("IsQuitOpen", true);
    }
    public void ConfirmQuit() { 
        Application.Quit();
    }
    public void CancelQuit() {
        Animator.SetBool("IsQuitOpen", false);
    }
    public void BackMainMenu() {
        Animator.SetBool("IsOptionOpen", false);
        CloseOption();
    }

    public void PauseAnim(){
        Animator.SetBool("IsPaused",true);
    }

    public void UnPauseAnim(){
        Animator.SetBool("IsPaused",false);
    }
    public void SelectButton(GameObject gameObject) {
        if(gameObject.TryGetComponent(out Button button)) { 
            button.Select();
        }

    }
}
