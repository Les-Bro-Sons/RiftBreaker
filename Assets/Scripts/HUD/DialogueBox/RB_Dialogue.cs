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
    private float _writingLetterTimer;
    private float _currentWritingDelay;
    [SerializeField] private float _writingDelay;
    [SerializeField] private float _writingSpeedingDelay;
    [HideInInspector] public bool DialogueOpened = false;
    private bool _isListening = false;
    private bool _alreadyClosedAfterTime = false;

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
        _writingLetterTimer += Time.deltaTime;
    }

    public void StartDialogue() //Start the dialogue system
    {
        DialogueOpened = true;
        PlayOpenAnim();
        StartCoroutine(StartDialogueAfterOpenAnim(0));
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
        _alreadyClosedAfterTime = false;
    }

    public void StartDialogue(int dialogueIndex)
    {
        _currentDialogueIndex = dialogueIndex;
        DialogueOpened = true;
        PlayOpenAnim();
        StartCoroutine(StartDialogueAfterOpenAnim(dialogueIndex));
        EventOnDialogueStarted?.Invoke();
        if (_scriptableDialogues[dialogueIndex].IsNameInput)
        {
            RB_InputManager.Instance.MoveEnabled = false;
            RB_InputManager.Instance.DashEnabled = false;
        }
        if (_scriptableDialogues[dialogueIndex].Clickable)
        {
            RB_InputManager.Instance.AttackEnabled = false;
        }
        _alreadyClosedAfterTime = false;
    }

    public IEnumerator StartDialogueAfterOpenAnim(int dialogueIndex) //Initialize the dialogue system just after the animation started
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(_dialogueAnimator.GetCurrentAnimatorClipInfo(0).Length);
        _currentDialogueIndex = dialogueIndex;
        _currentDialogueFinished = false;
        _clickIndex = 0;
        _dialogueStarted = true;
        StartDrawText(dialogueIndex);
        _currentWritingDelay = _writingDelay; //Set the current delay to the normal one
    }

    private void StopDrawText()
    {
        int dialogueToTest = _currentDialogueIndex;
        if(dialogueToTest >= _scriptableDialogues.Count)
        {
            dialogueToTest--;
        }
        if (!_isListening && _scriptableDialogues[dialogueToTest].IsNameInput)
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
        {
            if (!_alreadyClosedAfterTime && _scriptableDialogues[dialogueToTest].CloseAfterTime)
            {
                _alreadyClosedAfterTime = true;
                Invoke(nameof(StopDialogue), _scriptableDialogues[dialogueToTest].TimeAfterClose);
            }
            else if ((!_scriptableDialogues[dialogueToTest].IsNameInput && _scriptableDialogues[dialogueToTest].Clickable) || (_scriptableDialogues[dialogueToTest].IsNameInput && !string.IsNullOrEmpty(_playerNameInputField.text) && _scriptableDialogues[dialogueToTest].Clickable))
            {
                _nextDialogueArrow.SetBool("Open", true);
            }

        }
        
    }

    private void StartDrawText(int DialogueIndex)
    {
        _currentParagraph = _scriptableDialogues[DialogueIndex].Paragraph; //Current paragraph
        StartCoroutine(StartDrawTextCoroutine(DialogueIndex));
    }

    private IEnumerator StartDrawTextCoroutine(int DialogueIndex)
    {
        Color defaultColor = _dialogueBox.color;
        Color invisibleColor = new Color(_dialogueBox.color.r, _dialogueBox.color.g, _dialogueBox.color.b, 0);

        _dialogueBox.color = invisibleColor;
        _dialogueBox.enableAutoSizing = true;
        _dialogueBox.text = _scriptableDialogues[DialogueIndex].Paragraph;
        
        yield return new WaitForEndOfFrame();

        float currentSize = _dialogueBox.fontSize;
        _dialogueBox.enableAutoSizing = false;
        _dialogueBox.fontSize = currentSize;
        _dialogueBox.text = "";
        _dialogueBox.color = defaultColor;
        _shouldWriteText = true; //Start the drawing of the text
        _writingLetterTimer = 0; //Delay of the drawing
        _currentLetterIndex = 0;
        _robertAnim.StartTalk(_scriptableDialogues[DialogueIndex].CurrentAnimation); //Start the talking of robert
    }

    private void DrawText()
    {
        if (_shouldWriteText && _writingLetterTimer >= _currentWritingDelay)
        {
            if (_currentLetterIndex < _currentParagraph.Length)
            {
                DrawChar(true);
            }
            else
            {
                StopDrawText(); //If the text is finished, stop the drawing of the text
            }
        }
        
    }

    private void DrawChar(bool checkTimerAgain = false)
    {
        _writingLetterTimer = Mathf.Clamp((_writingLetterTimer - _currentWritingDelay), 0, int.MaxValue); //Delay of the drawing 
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

        if (checkTimerAgain) DrawText();
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
        _nextDialogueArrow.SetBool("Open", false);
        Invoke(nameof(ReinitializeAfterCloseAnim), _dialogueAnimator.GetCurrentAnimatorClipInfo(0).Length);
    }

    private void ReinitializeAfterCloseAnim()
    {
        _dialogueBox.text = "";
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
                if (_scriptableDialogues[_currentDialogueIndex].CloseByClick)
                    StopDialogue();
                else
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
        int indexToTest = _currentDialogueIndex;
        if(_scriptableDialogues.Count >= indexToTest)
            indexToTest = _scriptableDialogues.Count-1;
        if (_isListening && !(_scriptableDialogues[indexToTest].IsNameInput))
        {
            _playerNameInputField.onValueChanged.RemoveListener(OnPlayerEnterLetterName);
            RB_InputManager.Instance.MoveEnabled = true;
            RB_InputManager.Instance.DashEnabled = true;
        }
        if (_scriptableDialogues[indexToTest].Clickable || !(_scriptableDialogues[indexToTest].Clickable))
        {
            RB_InputManager.Instance.AttackEnabled = true;
        }
        StopDrawText();
        _currentDialogueFinished = true;
        _dialogueStarted = false;
        PlayCloseAnim();
        EventOnDialogueStopped?.Invoke();
        DialogueOpened = false;
        
        
    }
}
