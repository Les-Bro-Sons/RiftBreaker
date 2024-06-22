using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class RB_SaveManager : MonoBehaviour
{
    //Save
    public RB_SaveObject SaveObject = new();

    //Instance
    public static RB_SaveManager Instance;
    public bool IsSaveExist;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        LoadFromJson();
    }

    public void SaveToJson()
    {
        SaveObject.CurrentLevel = SceneManager.GetActiveScene().buildIndex+1; //Save the next level
        if (RB_TutorialManager.Instance) SaveObject.TutoDone = !RB_TutorialManager.Instance.IsTuto;

        //Save everything to json
        string saveObjectData = JsonUtility.ToJson(SaveObject);
        string filePath = Application.persistentDataPath + "/SaveObjectData.json";
        System.IO.File.WriteAllText(filePath, saveObjectData);
        IsSaveExist = true;
        print("Sauvegard effectué");
    }

    public void LoadFromJson()
    {
        
        string filePath = Application.persistentDataPath + "/SaveObjectData.json";
        if (!File.Exists(filePath))
        {
            IsSaveExist = false;
            SaveToJson();
        }
        else { 
            IsSaveExist=true;
        }
        //Load everything from json
        if (SaveObject.CurrentLevel == 0)
            SaveObject.CurrentLevel = 1; //If the level is set to the menu, set it to the first level
        string saveObjectData = System.IO.File.ReadAllText(filePath);
        SaveObject = JsonUtility.FromJson<RB_SaveObject>(saveObjectData);
    }

    public void ResetSave()
    {
        string filePath = Application.persistentDataPath + "/SaveObjectData.json";
        File.Delete(filePath);
    }
}
