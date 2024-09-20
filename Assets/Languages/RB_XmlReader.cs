using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class RB_XmlReader : MonoBehaviour
{
    public TextAsset Dictionary;

    public string DefaultLanguage = "francais";
    public string LanguageName;
    public int CurrentLanguage;

    public TMP_Dropdown SelectDropdown;

    private List<Dictionary<string, string>> _languages = new();
    private Dictionary<string, string> _obj;

    private TMP_Text[] _allTextElements;
    private string[] _originalKeys;


    void Awake()
    {
        Reader();
        _allTextElements = FindObjectsOfType<TMP_Text>();
        
        _originalKeys = new string[_allTextElements.Length]; // Initialiser le tableau des clés originales
        StoreOriginalKeys(); // Stocke les clés originales
        
        _languages[CurrentLanguage].TryGetValue(DefaultLanguage, out LanguageName);
        ValueChangeCheck();
    }

    private void Reader()
    {
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(this.Dictionary.text);
        XmlNodeList languageList = xmlDoc.GetElementsByTagName("language"); // separation des languages

        foreach (XmlNode languageValue in languageList) // pour chaque language
        {
            XmlNodeList languageContent = languageValue.ChildNodes; // recupere toutes les balises
            _obj = new();

            foreach (XmlNode value in languageContent)
            {
                _obj.Add(value.Name, value.InnerText);
            }

            _languages.Add(_obj);
        }
    }

    private void StoreOriginalKeys()
    {
        // Stocke les clés originales à partir du texte initial dans chaque TMP_Text
        for (int i = 0; i < _allTextElements.Length; i++)
            _originalKeys[i] = _allTextElements[i].text; // Sauvegarde la clé originale
    }

    private void UpdateAllTexts()
    {
        for (int i = 0; i < _allTextElements.Length; i++)
            UpdateText(_allTextElements[i], _originalKeys[i]);
    }

    private void UpdateText(TMP_Text tmpText, string originalKey)
    {
        if (_languages[CurrentLanguage].TryGetValue(originalKey, out string translateValue))
            tmpText.text = translateValue; // remplace par la valeur traduite
        else
            Debug.Log($"Key '{originalKey}' not found in the dictionary for the language {LanguageName}");
    }


    public void ValueChangeCheck()
    {
        CurrentLanguage = SelectDropdown.value;
        //_languages[CurrentLanguage].TryGetValue("Name", out LanguageName);

        UpdateAllTexts();
    }
}