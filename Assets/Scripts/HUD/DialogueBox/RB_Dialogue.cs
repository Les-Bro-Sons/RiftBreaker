using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RB_Dialogue : MonoBehaviour
{

    //Component
    [SerializeField] private TextMeshProUGUI _dialogueBox;
    [SerializeField] private List<RB_Dialogue_Scriptable> _scriptableDialogues = new List<RB_Dialogue_Scriptable>();
    [SerializeField] private Animator _dialogueAnimator;

    //Click
    private int _clickIndex = 0;
    [SerializeField] private bool _clickable = true;

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

    private void Start()
    {
        RB_MenuInputManager.Instance.EventAnyStarted.AddListener(Click);
        _currentDialogueIndex = 0;
    }

    private void Update()
    {
        DrawText();
    }

    public void StartDialogue() //Start the dialogue system
    {
        PlayOpenAnim();
        StartCoroutine(StartDialogueAfterOpenAnim());
    }

    public IEnumerator StartDialogueAfterOpenAnim() //Initialize the dialogue system just after the animation started
    {
        _dialogueBox.text = "";
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(_dialogueAnimator.GetCurrentAnimatorClipInfo(0).Length);
        _currentDialogueIndex = 0;
        _currentDialogueFinished = false;
        _clickIndex = 0;
        _dialogueStarted = true;
        _currentParagraph = _scriptableDialogues[0].Paragraph;
        StartDrawText(0);
        _currentWritingDelay = _writingDelay; //Set the current delay to the normal one
    }

    private void StopDrawText()
    {
        _shouldWriteText = false; //Stop the drawing of the text
        _currentDialogueFinished = true; //The current dialogue is finished
    }

    private void StartDrawText(int DialogueIndex)
    {
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
                _dialogueBox.text += _currentLetter; //Drawing of the text
                _currentLetterIndex++;
            }
            else
            {
                StopDrawText(); //If the text is finished, stop the drawing of the text
            }
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
        if (_dialogueStarted)
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
            else if(_clickable)//If the current dialogue is finished show the next dialogue
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
    }

    public void NextDialogue()
    {
        _currentDialogueIndex++; //Set to the next dialogue
        print("next dialogue 1");
        if(_currentDialogueIndex >= _scriptableDialogues.Count) //If there's no more dialogues stop the dialogue
        {
            StopDialogue();
            return;
        }
        print("next dialogue 2");
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
    }
}
