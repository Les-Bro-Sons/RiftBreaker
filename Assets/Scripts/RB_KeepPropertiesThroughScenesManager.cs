using System;
using System.Collections.Generic;
using UnityEngine;

public class RB_KeepPropertiesThroughScenesManager : MonoBehaviour
{

    /// <summary>
    /// How to use Manager : First save the properties with an id before loading a new scene, the load the scene, after that load the property back. Exemple : int a = LoadSavedProperties("a")
    /// </summary>

    //Instance
    public static RB_KeepPropertiesThroughScenesManager Instance;

    //Properties saved
    private Dictionary<string, object> _propertiesToKeepThroughScene = new();

    //Awake
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// This function saves an object to keep it through scenes
    /// </summary>
    /// <param name="propertyToSave"> Object to save through scenes </param>
    /// <param name="idOfTheProperty"> The id of the property saved </param>
    public void SaveProperties(string idOfTheProperty, object propertyToSave)
    {
        if (propertyToSave == null) throw new ArgumentNullException("The object you're trying to save is null"); //If the user wants to save a null object
        _propertiesToKeepThroughScene.Add(idOfTheProperty, propertyToSave); //Save the object to the properties list
        print("Saved successfully");
    }

    /// <summary>
    /// This function saves a list of an object to keep through scenes
    /// </summary>
    /// <param name="propertiesToSave"> List of objects to keep through scenes </param>
    public void SaveProperties(Dictionary<string, object> propertiesToSave) //Override
    {
        foreach(var property in propertiesToSave)
        {
            if (property.Value == null) throw new ArgumentNullException("The object you're trying to save is null"); //If the user wants to save a null object
            _propertiesToKeepThroughScene.Add(property.Key, property.Value); //Save every object to the properties list (replacement for AddRange)
            print("Saved successfully");
        }
    }

    /// <summary>
    /// This function saves a bunch of object to keep through scenes. Use : SaveProperties(id1, object1; id2, object2...)
    /// </summary>
    /// <param name="propertiesToSave"> The parameters to save and their ids</param>
    public void SaveProperties(params object[] propertiesToSave) //Override
    {
        if(propertiesToSave.Length % 2 != 0)
            throw new ArgumentException("The number of arguments must be even (key-value pairs)."); //If there's not each time one id and one object
        
        for(int i = 0; i < propertiesToSave.Length; i+=2)
        {
            if (propertiesToSave[i] is string key)
            {
                _propertiesToKeepThroughScene[key] = propertiesToSave[i + 1]; //Foreach object save it with his id to the list of saved properties
                print("Saved successfully");
            }
            else
                throw new ArgumentException($"Argument at position {i} must be a string representing the key."); //If the user didn't enter a string as an id
        }
    }

    /// <summary>
    /// This function gets the saved properties
    /// </summary>
    /// <returns> The saved properties previously </returns>
    public Dictionary<string, object> LoadSavedProperties()
    {
        print("Loaded successfully");
        return _propertiesToKeepThroughScene;
    }

    /// <summary>
    /// This function gets the saved property by his id
    /// </summary>
    /// <typeparam name="T"> The type of the property returned</typeparam>
    /// <param name="id"> The id of the object that you want</param>
    /// <returns> The object wanted with the type wanted</returns>
    public T LoadSavedProperties<T>(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id is null");
        if (!_propertiesToKeepThroughScene.ContainsKey(id)) throw new InvalidOperationException($"The saved properties does not contain {id}");
        if (_propertiesToKeepThroughScene[id] is null) throw new ArgumentNullException("The object you are trying to load is null");
        if (_propertiesToKeepThroughScene[id] is not T) throw new InvalidOperationException($"The object you are trying to get is not of type {typeof(T).Name}");
        return (T)_propertiesToKeepThroughScene[id];
    }

    /// <summary>
    /// This function removes a property if it's in the list of saved properties and return the new saved properties
    /// </summary>
    /// <param name="propertyToRemove"> The object you want to remove from the saved properties </param>
    /// <returns> The list of saved object after the remove of the property </returns>
    public Dictionary<string, object> RemoveFromSavedProperties(object propertyToRemove)
    {
        if (_propertiesToKeepThroughScene == null) throw new ArgumentNullException("There's no properties saved"); //If the user wants to remove an object from the saved properties but the list is empty
        if (propertyToRemove == null) throw new ArgumentNullException("The object you are trying to remove is null"); //If the user wants to remove a null object
        if (_propertiesToKeepThroughScene.ContainsValue(propertyToRemove)) //If the object is in the saved properties
        {
            foreach(var property in _propertiesToKeepThroughScene)
            {
                if(property.Value == propertyToRemove)
                {
                    _propertiesToKeepThroughScene.Remove(property.Key); //Remove the object from the saved properties
                }
            }
        }
        else
            throw new ArgumentException("The object you are trying to remove is not in the saved list"); //If the user wants to remove an object that is not in the saved object

        return _propertiesToKeepThroughScene;
    }

    /// <summary>
    /// This function removes a property if it's in the list of saved properties and return the new saved properties
    /// </summary>
    /// <param name="propertyToRemove"> The id of the object you want to remove </param>
    /// <returns> The list of saved object after the remove of the property </returns>
    public Dictionary<string, object> RemoveFromSavedProperties(string propertyIdToRemove) //Override
    {
        if (_propertiesToKeepThroughScene == null) throw new ArgumentNullException("There's no properties saved");
        if (string.IsNullOrEmpty(propertyIdToRemove)) throw new ArgumentNullException("The object you are trying to remove is null"); //If the user didn't entered any id
        if (_propertiesToKeepThroughScene.ContainsKey(propertyIdToRemove)) //If the object by id is in the saved object
        {
            _propertiesToKeepThroughScene.Remove(propertyIdToRemove); //Remove the object from the saved object
        }
        else
            throw new ArgumentException("The object you are trying to remove is not in the saved list");

        return _propertiesToKeepThroughScene;
    }

    /// <summary>
    /// This function removes a list of properties if it's in the list of saved properties and return the new saved properties
    /// </summary>
    /// <param name="propertiesToRemove"> A list of object to remove </param>
    /// <returns> The list of saved object after the remove of the properties </returns>
    public Dictionary<string, object> RemoveFromSavedProperties(List<object> propertiesToRemove) //Override
    {
        if (_propertiesToKeepThroughScene == null) throw new ArgumentNullException("There's no properties saved");
        if (propertiesToRemove is null) throw new ArgumentNullException("The list of object you want to remove is null");
        if (propertiesToRemove.Count == 0) throw new ArgumentException("The list of object you want to remove is empty");
        for(int i = 0; i < propertiesToRemove.Count; i++)
        {
            if (propertiesToRemove[i] is not null)
            {
                if (_propertiesToKeepThroughScene.ContainsValue(propertiesToRemove[i]))
                {
                    foreach(var property in _propertiesToKeepThroughScene)
                    {
                        if (property.Value == propertiesToRemove[i])
                        {
                            _propertiesToKeepThroughScene.Remove(property.Key); //Remove every object that is in the list from the saved properties
                        }
                    }
                }
                else
                    throw new ArgumentException($"{propertiesToRemove[i]} is not in the saved properties");
            }
            else
                throw new ArgumentNullException($"The object {propertiesToRemove[i]} is null");
        }

        return _propertiesToKeepThroughScene;
    }

    /// <summary>
    /// This function removes objects by a list of id from the saved properties
    /// </summary>
    /// <param name="propertiesIdToRemove"> The list of ids you want to remove </param>
    /// <returns> The list of saved object after the remove of the properties </returns>
    public Dictionary<string, object> RemoveFromSavedProperties(List<string> propertiesIdToRemove) //Override
    {
        if (_propertiesToKeepThroughScene == null) throw new ArgumentNullException("There's no properties saved");
        if (propertiesIdToRemove is null) throw new ArgumentNullException("The list of ids you want to remove is null");
        if (propertiesIdToRemove.Count == 0) throw new ArgumentException("The list of ids you want to remove is empty");
        for (int i = 0; i < propertiesIdToRemove.Count; i++)
        {
            if (!string.IsNullOrEmpty(propertiesIdToRemove[i]))
            {
                if (_propertiesToKeepThroughScene.ContainsKey(propertiesIdToRemove[i]))
                {
                    _propertiesToKeepThroughScene.Remove(propertiesIdToRemove[i]); //Remove every object by id that is in the list from the saved properties
                }
                else
                    throw new ArgumentException($"{propertiesIdToRemove[i]} is not in the saved properties");
            }
            else
                throw new ArgumentNullException($"{propertiesIdToRemove[i]} is null");
        }

        return _propertiesToKeepThroughScene;
    }

    /// <summary>
    /// This function removes a bunch of object from the saved properties
    /// </summary>
    /// <param name="propertiesToRemove"> The objects you want to remove </param>
    /// <returns> The list of saved object after the remove of the properties </returns>
    public Dictionary<string, object> RemoveFromSavedProperties(params object[] propertiesToRemove) //Override
    {
        if (_propertiesToKeepThroughScene == null) throw new ArgumentNullException("There's no properties saved");
        if (propertiesToRemove is null) throw new ArgumentNullException("The list of object you want to remove is null");
        if (propertiesToRemove.Length == 0) throw new ArgumentException("The list of object you want to remove is empty");
        for (int i = 0; i < propertiesToRemove.Length; i++)
        {
            if (propertiesToRemove[i] is not null)
            {
                if (_propertiesToKeepThroughScene.ContainsValue(propertiesToRemove[i]))
                {
                    foreach (var property in _propertiesToKeepThroughScene)
                    {
                        if (property.Value == propertiesToRemove[i])
                        {
                            _propertiesToKeepThroughScene.Remove(property.Key);
                        }
                    }
                }
                else
                    throw new ArgumentException($"{propertiesToRemove[i]} is not in the saved properties");
            }
            else
                throw new ArgumentNullException($"The object {propertiesToRemove[i]} is null");
        }

        return _propertiesToKeepThroughScene;
    }

    /// <summary>
    /// This function removes a bunch object by ids from the saved objects
    /// </summary>
    /// <param name="propertiesIdToRemove"> The bunch of ids you want to remove </param>
    /// <returns> The list of saved object after the remove of the properties </returns>
    public Dictionary<string, object> RemoveFromSavedProperties(params string[] propertiesIdToRemove) //Override
    {
        if (_propertiesToKeepThroughScene == null) throw new ArgumentNullException("There's no properties saved");
        if (propertiesIdToRemove is null) throw new ArgumentNullException("The list of ids you want to remove is null");
        if (propertiesIdToRemove.Length == 0) throw new ArgumentException("The list of ids you want to remove is empty");
        for (int i = 0; i < propertiesIdToRemove.Length; i++)
        {
            if (!string.IsNullOrEmpty(propertiesIdToRemove[i]))
            {
                if (_propertiesToKeepThroughScene.ContainsKey(propertiesIdToRemove[i]))
                {
                    _propertiesToKeepThroughScene.Remove(propertiesIdToRemove[i]);
                }
                else
                    throw new ArgumentException($"{propertiesIdToRemove[i]} is not in the saved properties");
            }
            else
                throw new ArgumentNullException($"{propertiesIdToRemove[i]} is null");
        }

        return _propertiesToKeepThroughScene;
    }
}
