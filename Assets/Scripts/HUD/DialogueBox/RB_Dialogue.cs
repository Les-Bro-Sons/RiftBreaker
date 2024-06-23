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
    [SerializeField] private Animator _nextDialogueArrow;

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
    private bool _isListening = false;

    //Events
    [HideInInspector] public UnityEvent EventOnDialogueStarted;
    [HideInInspector] public UnityEvent EventOnDialogueStopped;

    //Action
    private UnityAction ActionEventOnPlayerEnterLetterName;

    private void Start()
    {
        RB_InputManager.Instance.EventAttackStartedEvenIfDisabled.AddListener(Click);
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
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(_dialogueAnimator.GetCurrentAnimatorClipInfo(0).Length);
        _currentDialogueIndex = 0;
        _currentDialogueFinished = false;
        _clickIndex = 0;
        _dialogueStarted = true;
        StartDrawText(0);
        _currentWritingDelay = _writingDelay; //Set the current delay to the normal one
    }

    private void StopDrawText()
    {
        if (!_isListening && _scriptableDialogues[_currentDialogueIndex].IsNameInput)
        {
            _isListening = true;
            _playerNameInputField.Select();
            _playerNameInputField.characterLimit = 12;
            _playerNameInputField.onValueChanged.AddListener(OnPlayerEnterLetterName);
            _playerNameInputField.onEndEdit.AddListener(OnPlayerFinishedEnterName);
        }
        _shouldWriteText = false; //Stop the drawing of the text
        _currentDialogueFinished = true; //The current dialogue is finished
        _robertAnim.StopTalk(); //Stop the talking of robert
        if (_scriptableDialogues[_currentDialogueIndex].CloseAfterTime)
        {
            Invoke(nameof(StopDialogue), _scriptableDialogues[_currentDialogueIndex].TimeAfterClose);
        }
        else if((!_scriptableDialogues[_currentDialogueIndex].IsNameInput && _scriptableDialogues[_currentDialogueIndex].Clickable) || (_scriptableDialogues[_currentDialogueIndex].IsNameInput && !string.IsNullOrEmpty(_playerNameInputField.text) && _scriptableDialogues[_currentDialogueIndex].Clickable))
        {
            _nextDialogueArrow.SetBool("Open", true);
        }
    }

    private void StartDrawText(int DialogueIndex)
    {
        _robertAnim.StartTalk(_scriptableDialogues[DialogueIndex].CurrentAnimation); //Start the talking of robert
        _currentParagraph = _scriptableDialogues[DialogueIndex].Paragraph; //Current paragraph
        StartCoroutine(StartDrawTextCoroutine());
    }

    private IEnumerator StartDrawTextCoroutine()
    {
        Color defaultColor = _dialogueBox.color;
        Color invisibleColor = new Color(_dialogueBox.color.r, _dialogueBox.color.g, _dialogueBox.color.b, 0);

        _dialogueBox.color = invisibleColor;
        _dialogueBox.enableAutoSizing = true;
        _dialogueBox.text = _scriptableDialogues[_currentDialogueIndex].Paragraph;
        
        yield return new WaitForEndOfFrame();

        float currentSize = _dialogueBox.fontSize;
        _dialogueBox.enableAutoSizing = false;
        _dialogueBox.fontSize = currentSize;
        _dialogueBox.text = "";
        _dialogueBox.color = defaultColor;
        _shouldWriteText = true; //Start the drawing of the text
        _writingLetterTime = Time.unscaledTime; //Delay of the drawing
        _currentLetterIndex = 0;
    }

    private void DrawText()
    {
        if (_shouldWriteText && Time.unscaledTime > _writingLetterTime + _currentWritingDelay)
        {
            if (_currentLetterIndex < _currentParagraph.Length)
            {
                DrawChar();
            }
            else
            {
                StopDrawText(); //If the text is finished, stop the drawing of the text
            }
        }
        
    }

    private void DrawChar()
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
        if(_currentDialogueIndex <= _scriptableDialogues.Count)
        {
            switch (_scriptableDialogues[_currentDialogueIndex].CurrentAnimation)
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
                    if (RB_AudioManager.Instance.ClipPlayingCount("miAnnoyed") == 0) RB_AudioManager.Instance.PlaySFX("miAnnoyed", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                    break;
                case RB_RobertAnim.CurrentAnimation.CloseEyesSmile:
                case RB_RobertAnim.CurrentAnimation.Smile:
                case RB_RobertAnim.CurrentAnimation.Happy:
                    if (RB_AudioManager.Instance.ClipPlayingCount("miHappy") == 0) RB_AudioManager.Instance.PlaySFX("miHappy", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                    break;
                case RB_RobertAnim.CurrentAnimation.CloseEyes:
                case RB_RobertAnim.CurrentAnimation.Neutral:
                default:
                    if (RB_AudioManager.Instance.ClipPlayingCount("mi") == 0) RB_AudioManager.Instance.PlaySFX("mi", false, false, 0.25f, 1.5f, MIXERNAME.SFX);
                    break;
            }

        }
    }

    private void OnPlayerEnterLetterName(string playerName)
    {
        RB_SaveManager.Instance.SaveObject.PlayerName = _playerNameInputField.text;
        print("letter entered");
        ShowAllCurrentDialogue();
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
                print("entrer votre nom");
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
        if (_dialogueStarted && _scriptableDialogues[_currentDialogueIndex].Clickable && ((_scriptableDialogues[_currentDialogueIndex].IsNameInput && !string.IsNullOrEmpty(_playerNameInputField.text)) || !_scriptableDialogues[_currentDialogueIndex].IsNameInput))
        {
            if (!_currentDialogueFinished)
            {
                if (_clickIndex == 0) //If it's the first click, set the current delay to the speed one
                {
                    _currentWritingDelay = _writingSpeedingDelay;
                }
                else if (_clickIndex == 1) //If it's the second click, show all the dialogue
                {
                    ShowAllCurrentDialogue();
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
        StopDrawText();
        _currentLetterIndex = 0;
        _dialogueBox.text = string.Empty;
        _currentParagraph = _scriptableDialogues[_currentDialogueIndex].Paragraph;
        while (_currentLetterIndex < _currentParagraph.Length)
        {
            DrawChar(); //Set all of the dialogue to the dialogue box
        }
    }

    public void NextDialogue()
    {
        _nextDialogueArrow.SetBool("Open", false);
        if (_isListening &&  !(_scriptableDialogues.Count-1 < _currentDialogueIndex && _scriptableDialogues[_currentDialogueIndex + 1].IsNameInput))
        {
            _playerNameInputField.onValueChanged.RemoveListener(OnPlayerEnterLetterName);
            RB_InputManager.Instance.MoveEnabled = true;
            RB_InputManager.Instance.DashEnabled = true;
        }
        if (_scriptableDialogues[_currentDialogueIndex].Clickable || !(_scriptableDialogues.Count-1 < _currentDialogueIndex && _scriptableDialogues[_currentDialogueIndex+1].Clickable))
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
        StopDrawText();
        _currentDialogueFinished = true;
        _dialogueStarted = false;
        PlayCloseAnim();
        EventOnDialogueStopped?.Invoke();
        DialogueOpened = false;
        
        
    }
}
