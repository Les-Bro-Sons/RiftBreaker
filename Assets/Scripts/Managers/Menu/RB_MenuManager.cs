using UnityEngine;

public class RB_MenuManager : MonoBehaviour {

    public static RB_MenuManager Instance;

    public Animator Animator;

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
    }

    public void CloseOption() { 
        IsOptionOpen = false;
    }

    public void OptionAudio() {
        Animator.SetTrigger("Audio");
    }
    public void OptionVideo() {
        Animator.SetTrigger("Video");
    }
    public void OptionControl() {
        Animator.SetTrigger("Control");
    }

    public void Credits() { 
    
    }

    public void Quit() { 
    
    }

    public void ConfirmQuit() { 
        Application.Quit();
    }

    public void CancelQuit() { 
        
    }

    public void BackMainMenu() {
        Animator.SetBool("IsOptionOpen", false);
    }



}
