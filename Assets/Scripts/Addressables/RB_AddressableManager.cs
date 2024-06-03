// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.AddressableAssets.ResourceLocators;
// using UnityEngine.ResourceManagement.AsyncOperations;
//
// [Serializable]
// public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
// {
//     public AssetReferenceAudioClip(string guid):base(guid){}
// }
//
//
// public class RB_AddressableManager : MonoBehaviour
// {
//     public AssetReferenceAudioClip RB_musicAssetReference;
//     
//     // Start is called before the first frame update
//     void Start()
//     {
//         Addressables.InitializeAsync().Completed += AddressableManager_Completed;
//     }
//
//     private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
//     {
//         RB_musicAssetReference.LoadAsset<AudioClip>().Completed += (clip) =>
//         {
//             var audioSource = gameObject.AddComponent<AudioSource>();
//             audioSource.clip = clip.Result;
//             audioSource.playOnAwake = false;
//             audioSource.loop = false;
//             audioSource.Play();
//         };
//     }
// }

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid) { }
}

public class RB_AddressableManager : MonoBehaviour
{
    public static RB_AddressableManager Instance;
    public Dictionary<string, AssetReferenceAudioClip> AudioAssetReferences;
    public GameObject AudioPrefab; // Reference to your audio prefab

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
        
        
        AudioAssetReferences = new Dictionary<string, AssetReferenceAudioClip>();

        // Add audio references to the dictionary (same as before)
        AudioAssetReferences.Add("MainMenuMusic", new AssetReferenceAudioClip("son_angaros"));
        AudioAssetReferences.Add("LevelCompleteSound", new AssetReferenceAudioClip("SheathingSound"));

        // Initialize Addressables
        Addressables.InitializeAsync().Completed += AddressableManager_Completed;
    }

    private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        // Iterate through audio references and load them
        foreach (var audioReference in AudioAssetReferences.Values)
        {
            audioReference.LoadAsset<AudioClip>().Completed += (clip) =>
            {
                if (clip.Status == AsyncOperationStatus.Succeeded)
                {
                    // Create a new GameObject instance
                   GameObject audioGameObject = Instantiate(AudioPrefab);

                    // Handle successful loading
                   var audioSource = audioGameObject.GetComponent<AudioSource>(); // Assuming the prefab has an AudioSource
                    audioSource.clip = clip.Result;

                    // Configure audio source based on your needs (playOnAwake, loop, etc.)
                    audioSource.playOnAwake = false;
                   // audioSource.loop = false;
                    // ... (other configuration)

                    // Optionally handle specific audio types differently (if needed)
                    // ...
                }
                else
                {
                    Debug.LogError($"Failed to load audio clip: {audioReference}");
                }
            };
        }
    }

    // Public methods (optional)
    public void PlayAudio(string audioName, Vector3 desiredPosition, Quaternion desiredRotation)
    {
        GameObject audioGameObject = Instantiate(AudioPrefab, desiredPosition, desiredRotation);
        if (AudioAssetReferences.TryGetValue(audioName, out var audioReference))
        {
            audioReference.LoadAsset<AudioClip>().Completed += (clip) =>
            {
                if (clip.Status == AsyncOperationStatus.Succeeded)
                {
                    // Create a new GameObject instance at the specified coordinates
                    
                    var audioSource = audioGameObject.GetComponent<AudioSource>();
                    audioSource.clip = clip.Result;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError($"Failed to load audio clip: {audioName}");
                }
            };
        }
        else
        {
            Debug.LogError($"Audio reference not found: {audioName}");
        }
    }
}






// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.AddressableAssets.ResourceLocators;
// using UnityEngine.ResourceManagement.AsyncOperations;
//
// [Serializable]
// public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
// {
//     public AssetReferenceAudioClip(string guid) : base(guid) { }
// }
//
// public class RB_AddressableManager : MonoBehaviour
// {
//     public Dictionary<string, AssetReferenceAudioClip> AudioAssetReferences;
//
//     private void Awake()
//     {
//         AudioAssetReferences = new Dictionary<string, AssetReferenceAudioClip>();
//
//         //Add audio references to the dictionary
//         AudioAssetReferences.Add("MainMenuMusic", new AssetReferenceAudioClip("son_angaros"));
//         AudioAssetReferences.Add("LevelCompleteSound", new AssetReferenceAudioClip("SheathingSound"));
//
//         // Initialize Addressables
//         Addressables.InitializeAsync().Completed += AddressableManager_Completed;
//     }
//
//     // Start is called before the first frame update
//     void Start()
//     {
//        
//        // PlayAudio("MainMenuMusic");
//     }
//
//     private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
//     {
//         // Iterate through audio references and load them
//         foreach (var audioReference in AudioAssetReferences.Values)
//         {
//             audioReference.LoadAsset<AudioClip>().Completed += (clip) =>
//             {
//                 if (clip.Status == AsyncOperationStatus.Succeeded)
//                 {
//                     // Handle successful loading
//                     var audioSource = gameObject.AddComponent<AudioSource>();
//                     audioSource.clip = clip.Result;
//                     // Configure audio source based on your needs (playOnAwake, loop, etc.)
//                     audioSource.playOnAwake = false;
//                     audioSource.loop = false;
//                     // ... (other configuration)
//                 }
//                 else
//                 {
//                     // Handle loading failures
//                     Debug.LogError($"Failed to load audio clip: {audioReference}");
//                 }
//             };
//         }
//     }
//
//     // Public methods to play or configure audio from the dictionary (optional)
//     public void PlayAudio(string audioName)
//     {
//         if (AudioAssetReferences.TryGetValue(audioName, out var audioReference))
//         {
//             audioReference.LoadAsset<AudioClip>().Completed += (clip) =>
//             {
//                 if (clip.Status == AsyncOperationStatus.Succeeded)
//                 {
//                     var audioSource = GetComponent<AudioSource>(); // Assuming there's an AudioSource
//                     audioSource.clip = clip.Result;
//                     audioSource.Play();
//                 }
//                 else
//                 {
//                     Debug.LogError($"Failed to load audio clip: {audioName}");
//                 }
//             };
//         }
//         else
//         {
//             Debug.LogError($"Audio reference not found: {audioName}");
//         }
//     }
// }
