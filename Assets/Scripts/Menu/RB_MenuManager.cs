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

    [Header("Sliders")]
    [SerializeField] Slider _sliderGeneral;
    [SerializeField] Slider _sliderMusic;
    [SerializeField] Slider _sliderSFX;

    private void Awake() {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        Animator = GetComponent<Animator>();
        Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void Start(){
        RB_ButtonSelectioner.Instance.SelectMainButton(0);
        Time.timeScale = 1f;
    }

    public void NewGame() {
        RB_ButtonSelectioner.Instance.BlockInteraction();
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 1);
    }

    public void Continue() {
        RB_ButtonSelectioner.Instance.BlockInteraction();
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), RB_SaveManager.Instance.SaveObject.CurrentLevel);
    }

    public void MainMenu() {
        Time.timeScale = 1f;
        RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 0);
    }

    public void Options() {
        Animator.SetBool("IsOptionOpen", true);
        IsOptionOpen = true;
        _menuState = MENUSTATE.Audio;
    }

    public void CloseOption() {
        RB_ButtonSelectioner.Instance.SelectMainButton(1);
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
        Animator.SetTrigger("Credits");
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
        _sliderGeneral.GetComponent<RB_MenuSlider>().ResetInteraction();
        _sliderMusic.GetComponent<RB_MenuSlider>().ResetInteraction();
        _sliderSFX  .GetComponent<RB_MenuSlider>().ResetInteraction();
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
