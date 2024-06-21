using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
[CustomEditor(typeof(RB_GroundManager))]
public class RB_CustomEditorGroundManager : Editor
{

    bool CurrentlyVisible = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RB_GroundManager groundManager = (RB_GroundManager)target;

        List<GameObject> GetAllGameObjectsInScene()
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            List<GameObject> gameObjectsInScene = new List<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // Check if the object is part of the active scene
                if (obj.hideFlags == HideFlags.None && obj.scene.IsValid())
                {
                    gameObjectsInScene.Add(obj);
                }
            }

            return gameObjectsInScene;
        }

        void ToggleAllDoors(bool isActive)
        {
            List<RB_Door> allDoors = new();
            foreach (GameObject objectFound in GetAllGameObjectsInScene())
            {
                if (objectFound.TryGetComponent<RB_Door>(out RB_Door door))
                {
                    allDoors.Add(door);
                }
            }
            foreach (RB_Door door in allDoors)
            {
                door.gameObject.SetActive(isActive);
                Debug.Log(door.gameObject);
            }
        }

        if (GUILayout.Button("Toggle doors"))
        {
            ToggleAllDoors(!CurrentlyVisible);
            CurrentlyVisible = !CurrentlyVisible;

        }
    }
}
#endif


public class RB_GroundManager : MonoBehaviour
{
    public static RB_GroundManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
