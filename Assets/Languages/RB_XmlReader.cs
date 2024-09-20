using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TMPro;

public class RB_XmlReader : MonoBehaviour
{
    public TextAsset Dictionary;

    public string LanguageName;
    public int CurrentLanguage;

    private string _bonjour;
    private string _aurevoir;

    // Variables specifique (affichage UI)
    public TMP_Text TextBonjour;
    public TMP_Text TextAurevoir;
    public TMP_Dropdown SelectDropdown;

    private List<Dictionary<string, string>> _languages = new();
    private Dictionary<string, string> _obj;

    void Awake()
    {
        Reader();
        
    }

    void Update()
    {
        _languages[CurrentLanguage].TryGetValue("Name", out LanguageName);
        _languages[CurrentLanguage].TryGetValue("bonjour", out _bonjour);
        _languages[CurrentLanguage].TryGetValue("aurevoir", out _aurevoir);

        TextBonjour.text = _bonjour;
        TextAurevoir.text = _aurevoir;
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

    public void ValueChangeCheck()
    {
        CurrentLanguage = SelectDropdown.value;
    }
}