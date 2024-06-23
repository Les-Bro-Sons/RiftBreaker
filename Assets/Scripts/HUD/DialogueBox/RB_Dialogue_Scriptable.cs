using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(RB_Dialogue_Scriptable))]
public class RB_CustomEditorDialogue : Editor
{
    SerializedProperty _clickableProperty;
    SerializedProperty _timeAfterCloseProperty;
    private void OnEnable()
    {
        _clickableProperty = serializedObject.FindProperty("Clickable");
        _timeAfterCloseProperty = serializedObject.FindProperty("TimeAfterClose");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RB_Dialogue_Scriptable dialogue = (RB_Dialogue_Scriptable)target;
        serializedObject.Update();


        if (dialogue.CloseAfterTime)
        {
            dialogue.Clickable = false;
            EditorGUILayout.PropertyField(_timeAfterCloseProperty);
        }
        else
        {
            EditorGUILayout.PropertyField(_clickableProperty);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RB_Dialogue_Scriptable", order = 1)]
public class RB_Dialogue_Scriptable : ScriptableObject
{
    [TextArea(15, 20)]  public string Paragraph;
    public Sprite Character_Sprite;
    public RB_RobertAnim.CurrentAnimation CurrentAnimation;
    public bool IsNameInput = false;
    public bool CloseAfterTime;
    [HideInInspector] public float TimeAfterClose = 0;
    [HideInInspector] public bool Clickable = true;
}
