using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;



public class RB_Dialogue : MonoBehaviour
{

    //Component
    [SerializeField] private TextMeshProUGUI _dialogueBox;
    [SerializeField] private List<RB_Dialogue_Scriptable> _scriptableDialogues = new List<RB_Dialogue_Scriptable>();
    [SerializeField] private Animator _dialogueAnimator;
    [SerializeField] private RB_RobertAnim _robertAnim;
    [SerializeField] private TMP_InputField _playerNameInputField;

    //Click
    private int _clickIndex = 0;

    //Text properties
    private int _currentDialogueIndex;
    private bool _currentDialogueFinished;
    private bool _dialogueStarted;
    private bool _shouldWriteText;
    private string _currentParagraph;
    private char _currentLetter;
    private int _currentLetterIndex;
    private float _writingLetterTime;
    private float _currentWritingDelay;
    [SerializeField] private float _writingDelay;
    [SerializeField] private float _writingSpeedingDelay;
    [HideInInspector] public bool DialogueOpened = false;

    //Events
    [HideInInspector] public UnityEvent EventOnDialogueStarted;
    [HideInInspector] public UnityEvent EventOnDialogueStopped;

    //Action
    private UnityAction ActionEventOnPlayerEnterLetterName;

    private void Start()
    {
        RB_InputManager.Instance..AddListener(Click);
        _currentDialogueIndex = 0;
    }

    private void Update()
    {
        DrawText();
    }

    public void StartDialogue() //Start the dialogue system
    {
        DialogueOpened = true;
        PlayOpenAnim();
        StartCoroutine(StartDialogueAfterOpenAnim());
        EventOnDialogueStarted?.Invoke();
        if (_scriptableDialogues[_currentDialogueIndex].IsNameInput)
        {
            _playerNameInputField.Select();
            _playerNameInputField.onValueChanged.AddListener(OnPlayerEnterLetterName);
            _playerNameInputField.onEndEdit.AddListener(OnPlayerFinishedEnterName);
            RB_InputManager.Instance.MoveEnabled = false;
            RB_InputManager.Instance.DashEnabled = false;
        }
        if (_scriptableDialogues[_currentDialogueIndex].Clickable)
        {
            RB_InputManager.Instance.AttackEnabled = false;
        }
    }

    public IEnumerator StartDialogueAfterOpenAnim() //Initialize the dialogue system just after the animation started
    {
        _currentParagraph = _scriptableDialogues[0].Paragraph;
        _robertAnim.StartIdle(_scriptableDialogues[0].CurrentAnimation);
        _dialogueBox.enableAutoSizing = true;
        _dialogueBox.text = _currentParagraph;

        yield return new WaitForEndOfFrame();

        float currentSize = _dialogueBox.fontSize;
        _dialogueBox.enableAutoSizing = false;
        _dialogueBox.fontSize = currentSize;
        _dialogueBox.text = "";

        yield return new WaitForSecondsRealtime(_dialogueAnimator.GetCurrentAnimatorClipInfo(0).Length);

        _currentDialogueIndex = 0;
        _currentDialogueFinished = false;
        _clickIndex = 0;
        _dialogueStarted = true;
        StartDrawText(0);
        _currentWritingDelay = _writingDelay; //Set the current delay to the normal one
        _dialogueBox.text = "";
    }

    private void StopDrawText()
    {
        _shouldWriteText = false; //Stop the drawing of the text
        _currentDialogueFinished = true; //The current dialogue is finished
        _robertAnim.StopTalk(); //Stop the talking of robert
        if (_scriptableDialogues[_currentDialogueIndex].CloseAfterTime)
        {
            Invoke(nameof(StopDialogue), _scriptableDialogues[_currentDialogueIndex].TimeAfterClose);
        }
    }

    private void StartDrawText(int DialogueIndex)
    {
        _dialogueBox.text = "";
        _robertAnim.StartTalk(_scriptableDialogues[DialogueIndex].CurrentAnimation); //Start the talking of robert
        _shouldWriteText = true; //Start the drawing of the text
        _writingLetterTime = Time.unscaledTime; //Delay of the drawing
        _currentParagraph = _scriptableDialogues[DialogueIndex].Paragraph; //Current paragraph
        _currentLetterIndex = 0;
    }

    private void DrawText()
    {
        if (_shouldWriteText && Time.unscaledTime > _writingLetterTime + _currentWritingDelay)
        {
            if (_currentLetterIndex < _currentParagraph.Length)
            {
                _writingLetterTime = Time.unscaledTime; //Delay of the drawing 
                _currentLetter = _currentParagraph[_currentLetterIndex];
                if (_currentLetter == '[')
                {
                    ReadTextAction();
                    _currentLetter = _currentParagraph[_currentLetterIndex];
                }
                _dialogueBox.text += _currentLetter; //Drawing of the text
                _currentLetterIndex++;
                {
                    switch (_currentAnimation)
                    {
                        case RB_RobertAnim.CurrentAnimation.AngryNeutral:
                        case RB_RobertAnim.CurrentAnimation.EvilSmile:
                        case RB_RobertAnim.CurrentAnimation.Angry:
                            if (RB_AudioManager.Instance.ClipPlayingCount("miAngry") == 0) RB_AudioManager.Instance.PlaySFX("miAngry", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                            break;
                        case RB_RobertAnim.CurrentAnimation.CloseEyesSad:
                        case RB_RobertAnim.CurrentAnimation.SadNeutral:
                        case RB_RobertAnim.CurrentAnimation.SadSmile:
                        case RB_RobertAnim.CurrentAnimation.Sad:
                            if (RB_AudioManager.Instance.ClipPlayingCount("miSad") == 0) RB_AudioManager.Instance.PlaySFX("miSad", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                            break;
                        case RB_RobertAnim.CurrentAnimation.Bruh:
                        case RB_RobertAnim.CurrentAnimation.BruhAnnoyed:
                        case RB_RobertAnim.CurrentAnimation.CloseEyes:
                            if (RB_AudioManager.Instance.ClipPlayingCount("miAnnoyed") == 0) RB_AudioManager.Instance.PlaySFX("miAnnoyed", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                            break;
                        case RB_RobertAnim.CurrentAnimation.CloseEyesSmile:
                        case RB_RobertAnim.CurrentAnimation.Smile:
                        case RB_RobertAnim.CurrentAnimation.Happy:
                            if (RB_AudioManager.Instance.ClipPlayingCount("miHappy") == 0) RB_AudioManager.Instance.PlaySFX("miHappy", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                            break;
                        default:
                            if (RB_AudioManager.Instance.ClipPlayingCount("mi") == 0) RB_AudioManager.Instance.PlaySFX("mi", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                            break;
                    }
                    
                }
                
            }
            else
            {
                StopDrawText(); //If the text is finished, stop the drawing of the text
            }
        }
        
    }

    private void OnPlayerEnterLetterName(string playerName)
    {
        RB_SaveManager.Instance.SaveObject.PlayerName = _playerNameInputField.text;
    }

    private void OnPlayerFinishedEnterName(string playerName)
    {
        print("player finished");
        if (_playerNameInputField.text != "")
        {
            NextDialogue();
        }
    }

    private void ReadTextAction()
    {
        string textAction = "";
        char currentLetterCheck;
        int startIndex = _currentLetterIndex;
        for (int i = _currentLetterIndex + 1; i < _currentParagraph.Length; i++)
        {
            currentLetterCheck = _currentParagraph[i];
            if (currentLetterCheck == ']')
            {
                DoTextAction(textAction, startIndex, i);
                break;
            }
            else
            {
                textAction += currentLetterCheck;
            }
        }
    }

    private void DoTextAction(string textAction, int startIndex, int endIndex)
    {
        string remplacement = string.Empty;
        
        switch (textAction)
        {
            case "Entrez votre nom":
                if (!string.IsNullOrEmpty(RB_SaveManager.Instance.SaveObject.PlayerName))
                {
                    remplacement = RB_SaveManager.Instance.SaveObject.PlayerName;
                }
                break;
            case "PLAYERNAME":
                remplacement = RB_SaveManager.Instance.SaveObject.PlayerName;
                break;
            
        }

        if (!string.IsNullOrEmpty(remplacement))
        {
            _currentParagraph = _currentParagraph.Remove(startIndex, (endIndex - startIndex) + 1);
            _currentParagraph = _currentParagraph.Insert(startIndex, remplacement);
        }
        
    }

    private void PlayOpenAnim() //Play the open animation of the dialogue box
    {
        _dialogueAnimator.SetBool("open", true);
    }

    private void PlayCloseAnim() //Play the close animation of the dialogue box
    {
        _dialogueAnimator.SetBool("open", false);
    }

    private void Click()
    {
        if (_dialogueStarted && _scriptableDialogues[_currentDialogueIndex].Clickable)
        {
            if (!_currentDialogueFinished)
            {
                if (_clickIndex == 0) //If it's the first click, set the current delay to the speed one
                {
                    _currentWritingDelay = _writingSpeedingDelay;
                }
                else if (_clickIndex == 1) //If it's the second click, show all the dialogue
                {
                    bool shouldShowDialogue = !_scriptableDialogues[_currentDialogueIndex].IsNameInput || !string.IsNullOrEmpty(_playerNameInputField.text);

                    if (shouldShowDialogue)
                    {
                        ShowAllCurrentDialogue();
                    }
                }
                _clickIndex++;
            }
            else//If the current dialogue is finished show the next dialogue
            {
                NextDialogue();
            }
        }
    }

    private void ShowAllCurrentDialogue() //Show all of the current dialogue
    {
        _shouldWriteText = false; //Stop the writing of the text
        _dialogueBox.text = _scriptableDialogues[_currentDialogueIndex].Paragraph; //Set all of the dialogue to the dialogue box
        _currentDialogueFinished = true; //Finish the current dialogue
        _robertAnim.StopTalk();
    }

    public void NextDialogue()
    {
        if (_scriptableDialogues[_currentDialogueIndex].IsNameInput)
        {
            _playerNameInputField.onValueChanged.RemoveListener(OnPlayerEnterLetterName);
            RB_InputManager.Instance.MoveEnabled = true;
            RB_InputManager.Instance.DashEnabled = true;
        }
        if (_scriptableDialogues[_currentDialogueIndex].Clickable)
        {
            RB_InputManager.Instance.AttackEnabled = true;
        }
        _currentDialogueIndex++; //Set to the next dialogue
        if(_currentDialogueIndex >= _scriptableDialogues.Count) //If there's no more dialogues stop the dialogue
        {
            StopDialogue();
            return;
        }
        //Otherwise show the next dialogue
        _currentDialogueFinished = false;
        _currentWritingDelay = _writingDelay;
        _clickIndex = 0;
        _dialogueBox.text = "";
        StartDrawText(_currentDialogueIndex);

    }

    public void StopDialogue() //Stop the dialogue system
    {
        _currentDialogueFinished = true;
        _dialogueStarted = false;
        PlayCloseAnim();
        EventOnDialogueStopped?.Invoke();
        DialogueOpened = false;
        
        
    }
}
