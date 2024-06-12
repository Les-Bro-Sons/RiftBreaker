using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RB_Dialogue_Scriptable", order = 1)]
public class RB_Dialogue_Scriptable : ScriptableObject
{
    public int Index;
    public string Paragraphe;
    public Sprite Character_Sprite;
    public ScriptableObject Child_Paragraphe;
}
