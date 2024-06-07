using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_DialogueWriting : MonoBehaviour
{
    public static RB_DialogueWriting Instance;
    [SerializeField] private float writingSpeed;
    [SerializeField] private TextMeshProUGUI dialogueBox;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private Image characterImage;
    [SerializeField] private List<Sprite> characterSprites = new List<Sprite>();
    [SerializeField] public GameObject lettreContainer;
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

    public void WriteText(Sprite characterI,string Name,string textToWrite) 
    {
        characterImage.sprite = characterI;
        NameText.text = Name;
        dialogueBox.text = "";
        StartCoroutine(DelayWriting(textToWrite));
        
    }

    public void ButtonTestDialogue() 
    {
        WriteText(characterSprites[1],
            "Nek",
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua." +
                  " Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.");
    }

    public IEnumerator DelayWriting(string text)
    {
        foreach (var lettre in text)
        {
            dialogueBox.text += lettre;
            yield return new WaitForSeconds(writingSpeed);
        }
        yield return null;
    }
}
