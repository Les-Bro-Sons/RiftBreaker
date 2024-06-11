using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_DialogueWriting : MonoBehaviour
{
    public static RB_DialogueWriting Instance;
    
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
    
    private int _index = 0;

    private string _completeText;
    
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

    private void Start() {
        RB_MenuInputManager.Instance.EventAnyStarted.AddListener(AnyKeyPressed);
        RB_MenuInputManager.Instance.EventAnyCanceled.AddListener(AnyKeyCanceled);
    }

    public void WriteText(Sprite characterI,string textToWrite) {
        _allTextWritten = false;
        _characterImage.sprite = characterI;
        _completeText = textToWrite;
        StartCoroutine(DelayWriting(textToWrite));
    }

    private void Update() {
        BoxGestion();
    }

    private void BoxGestion() {
        if (_dialogueBoxOpen)
        {
            _timer -= Time.deltaTime;
            if (_anyKeyPressed && _index == 0)
            {
                _writingSpeed = 0;
                _timer = 0.2f;
            }

            if (_timer <= 0 && _writingSpeed == 0 && _index == 0)
            {
                _index++;
            }

            if (_anyKeyPressed && _index == 1)
            {
                _dialogueBox.text = _completeText;
                _index++;
            }

            if (_anyKeyPressed && _index >= 2 || _anyKeyPressed && _allTextWritten)
            {
                _index = 0;
                _writingSpeed = 0.03f;
                CloseDialogue();
            }
        }
    }
    
    private void AnyKeyPressed() {
        _anyKeyPressed = true;
    }

    private void AnyKeyCanceled() {
        _anyKeyPressed = false;
    }

    public void ShowDialogue(int indexOfScriptable) {
        foreach (var scriptable in _scriptableDialogues)
        {
            if (scriptable.Index == indexOfScriptable)
            {
                instanceScriptable = scriptable;
            }
        }
        if (instanceScriptable != null)
        {
            _characterImage.sprite = null;
            _dialogueBox.text = "";
            PlayOpenAnim();
            StartCoroutine(WaitDialogueAnim());
        }
        else
        {
            Console.WriteLine("instanceScriptable = null");
        }
    }

    public void CloseDialogue() {
        PlayCloseAnim();
        _dialogueBoxOpen = false;
    }
    
    private void ButtonCloseDialogue() {
        PlayCloseAnim();
    }
    
    private void PlayOpenAnim() {
        _dialogueAnimator.SetBool("open", true);
        _dialogueAnimator.SetBool("close", false);
    }

    private void PlayCloseAnim() {
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

    public IEnumerator WaitDialogueAnim() {
        yield return new WaitForSeconds(_dialogueAnimator.GetCurrentAnimatorStateInfo(0).length  );
        
        WriteText(instanceScriptable.Character_Sprite, instanceScriptable.Paragraphe);
        
        _dialogueBoxOpen = true;
    }
}
