using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Test))]
public class CustomEditorTest : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Test test = (Test)target;
        if(GUILayout.Button("Save properties"))
        {
            test.SaveProperties();
        }

        if (GUILayout.Button("Load properties"))
        {
            test.LoadProperties();
        }
    }
}

#endif

public class Test : MonoBehaviour
{
    public int testInt1;
    public int testInt2;
    public string testString;
    public GameObject testGameObject;

    public List<string> ids;

    public void SaveProperties()
    {
        List<object> objects = new List<object>() { testInt1, testInt2, testString, testGameObject };
        Dictionary<string, object> properties = new();
        for(int i = 0; i < objects.Count; i++)
        {
            properties[ids[i]] = objects[i];
        }
        RB_KeepPropertiesThroughScenesManager.Instance.SaveProperties(properties);
        //RB_KeepPropertiesThroughScenesManager.Instance.SaveProperties(ids[0], testInt1, ids[1], testInt2, ids[2], testString, ids[3], testGameObject);
    }

    delegate Dictionary<string, object> LoadPropertiesDelegate();

    public void LoadProperties()
    {
        Func<string, int> loadInt = RB_KeepPropertiesThroughScenesManager.Instance.LoadSavedProperties<int>;
        Func<string, string> loadString = RB_KeepPropertiesThroughScenesManager.Instance.LoadSavedProperties<string>;
        Func<string, GameObject> loadGameObject = RB_KeepPropertiesThroughScenesManager.Instance.LoadSavedProperties<GameObject>;
        testInt1 = loadInt(ids[0]);
        testInt2 = loadInt(ids[1]);
        testString = loadString(ids[2]);
        testGameObject = loadGameObject(ids[3]);
    }
}
