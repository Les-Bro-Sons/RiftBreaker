using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_DialogueWritingObsolete : MonoBehaviour
{
    public static RB_DialogueWritingObsolete Instance;
    
    [SerializeField] private float _writingSpeed;
    [SerializeField] private TextMeshProUGUI _dialogueBox;
    [SerializeField] private Image _characterImage;
    [SerializeField] private Animator _dialogueAnimator;
    
    [SerializeField] private List<RB_Dialogue_Scriptable> _scriptableDialogues = new List<RB_Dialogue_Scriptable>();
    
    private IEnumerator coroutine;
    
    private bool _dialogueBoxOpen = false;
    private bool _allTextWritten = false;
    private bool _anyKeyPressed = false;
    
    RB_Dialogue_Scriptable instanceScriptable = null;

    private float _timer = 0;
    
    private int _clickIndex = 0;

    private string _completeText;

    private int CurrentDialogueIndex = 0;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
        
    }

    private void Start() 
    {
        RB_MenuInputManager.Instance.EventAnyStarted.AddListener(AnyKeyPressed);
        RB_MenuInputManager.Instance.EventAnyCanceled.AddListener(AnyKeyCanceled);
    }

    public void OpenDialogue()
    {
        PlayOpenAnim();
        _dialogueBoxOpen = true;
        ShowDialogue(CurrentDialogueIndex);
    }

    public void WriteText(Sprite characterI,string textToWrite) 
    {
        _allTextWritten = false;
        if(characterI != null)
        {
            _characterImage.sprite = characterI;
        }
        else
        {
            _characterImage.color = new Color(1, 1, 1, 0);
        }
        _completeText = textToWrite;
        StartCoroutine(DelayWriting(textToWrite));
    }

    private void Update() {
        BoxGestion();
    }

    //To Close the box when Pressing buttons
    private void BoxGestion() 
    {
        if (_dialogueBoxOpen)
        {
            _timer -= Time.deltaTime;
            
            // Key Pressed Once
            if (_anyKeyPressed && _clickIndex == 0)
            {
                _writingSpeed = 0;
                _timer = 0.2f;
            }

            if (_timer <= 0 && _writingSpeed == 0 && _clickIndex == 0)
            {
                _clickIndex++;
            }
            
            // Key Pressed Twice
            if (_anyKeyPressed && _clickIndex == 1)
            {
                _dialogueBox.text = _completeText;
                _clickIndex++;
            }
            
            // Key Pressed Thrice
            if (_anyKeyPressed && _clickIndex >= 2 || _anyKeyPressed && _allTextWritten)
            {
                _clickIndex = 0;
                _writingSpeed = 0.03f;
                if (CurrentDialogueIndex >= _scriptableDialogues.Count-1)
                {
                    CloseDialogue();
                }
                else
                {
                    ShowDialogue(CurrentDialogueIndex);
                }
                CurrentDialogueIndex++;
            }
        }
    }
    
    private void AnyKeyPressed() 
    {
        _anyKeyPressed = true;
    }

    private void AnyKeyCanceled() 
    {
        _anyKeyPressed = false;
    }

    //To Pop Up the dialogue by their Index
    public void ShowDialogue(int indexOfScriptable) 
    {
        instanceScriptable = _scriptableDialogues[indexOfScriptable];
        if (instanceScriptable != null)
        {
            _characterImage.sprite = null;
            _dialogueBox.text = "";
            StartCoroutine(WaitDialogueAnim());
        }
        else
        {
            Console.WriteLine("instanceScriptable = null");
        }
    }

    public void CloseDialogue() 
    {
        PlayCloseAnim();
        _dialogueBoxOpen = false;
    }
    
    private void ButtonCloseDialogue() 
    {
        PlayCloseAnim();
    }
    
    private void PlayOpenAnim() 
    {
        _dialogueAnimator.SetBool("open", true);
        _dialogueAnimator.SetBool("close", false);
    }

    private void PlayCloseAnim() 
    {
        _dialogueAnimator.SetBool("close", true);
        _dialogueAnimator.SetBool("open", false);
    }
    
    public IEnumerator DelayWriting(string text)
    {
        foreach (var lettre in text)
        {
            if (!_allTextWritten)
            {
                _dialogueBox.text += lettre;
                yield return new WaitForSeconds(_writingSpeed);
            }
            else
            {
                yield break;
            }
        }
        _allTextWritten = true;
        yield return null;
    }

    public IEnumerator WaitDialogueAnim() 
    {
        yield return new WaitForSeconds(_dialogueAnimator.GetCurrentAnimatorStateInfo(0).length  );
        
        WriteText(instanceScriptable.Character_Sprite, instanceScriptable.Paragraph);
    }
}
