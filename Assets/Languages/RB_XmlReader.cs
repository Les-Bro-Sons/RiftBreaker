using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class RB_XmlReader : MonoBehaviour
{
    public TextAsset Dictionary;

    [Tooltip("Choisissez la langue par défaut dans la liste")]
    public LANGUAGES DefaultLanguage = LANGUAGES.Francais;
    public string LanguageName;
    public int CurrentLanguage;

    public TMP_Dropdown SelectDropdown;

    private Dictionary<string, string> _obj;
    private List<Dictionary<string, string>> _languages = new();

    private TMP_Text[] _allTextElements;
    private string[] _originalKeys;


    void Awake()
    {
        Reader();
        _allTextElements = FindObjectsOfType<TMP_Text>();
        
        _originalKeys = new string[_allTextElements.Length]; // Initialiser le tableau des clés originales
        StoreOriginalKeys(); // Stocke les clés originales

        LoadDefaultLanguage();
        PopulateDropdown();

        SelectDropdown.value = CurrentLanguage;
        UpdateAllTexts();
        
        SelectDropdown.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    private void PopulateDropdown()
    {
        SelectDropdown.ClearOptions(); // Efface les options existantes
        List<string> options = new();

        foreach (var language in _languages)
        {
            if (language.TryGetValue("Name", out string langName))
            {
                options.Add(langName); // Ajoute le nom de la langue à la liste des options
            }
        }

        SelectDropdown.AddOptions(options); // Ajoute les options au dropdown
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

    private void LoadDefaultLanguage()
    {
        // Recherche la langue par défaut dans _languages


        CurrentLanguage = (int)DefaultLanguage;

        if (_languages.Count > CurrentLanguage)
        {
            _languages[CurrentLanguage].TryGetValue("Name", out LanguageName);
        }

        /*// sans l'enum

        for (int i = 0; i < _languages.Count; i++)
        {
            if (_languages[i].TryGetValue("Name", out string language) && language.ToLower() == DefaultLanguage.ToLower())
            {
                CurrentLanguage = i; // Définit l'index de la langue par défaut
                LanguageName = language; // Sauvegarde le nom de la langue
                break;
            }
        }*/
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
        /*else
            Debug.Log($"Key '{originalKey}' not found in the dictionary for the language {LanguageName}");*/
    }

    public void ValueChangeCheck()
    {
        CurrentLanguage = SelectDropdown.value;
        //_languages[CurrentLanguage].TryGetValue("Name", out LanguageName);

        UpdateAllTexts();
    }
}