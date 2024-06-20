using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RB_Dialogue_Scriptable", order = 1)]
public class RB_Dialogue_Scriptable : ScriptableObject
{
    [TextArea(15, 20)]  public string Paragraph;
    public Sprite Character_Sprite;
}
