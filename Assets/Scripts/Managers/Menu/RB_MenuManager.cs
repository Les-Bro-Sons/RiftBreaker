using UnityEngine;
using UnityEngine.UI;

public class RB_MenuManager : MonoBehaviour {

    public static RB_MenuManager Instance;

    public Animator Animator;

    enum MENUSTATE { 
        Audio, Video, Control
    }

    MENUSTATE _menuState;

    public bool IsOptionOpen;
    private void Start() {
        if (Instance == null) { Instance = this; }
        Animator = GetComponent<Animator>();
    }

    public void Play() {
    }

    public void Options() {
        Animator.SetBool("IsOptionOpen", true);
        IsOptionOpen = true;
        _menuState = MENUSTATE.Audio;
    }

    public void CloseOption() { 
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
    }

    public void SelectButton(GameObject gameObject) {
        if(gameObject.TryGetComponent(out Button button)) { 
            button.Select();
        }

    }

}
