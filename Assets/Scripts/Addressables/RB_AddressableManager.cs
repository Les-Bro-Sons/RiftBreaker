using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid):base(guid){}
}
public class RB_AddressableManager : MonoBehaviour
{
    public AssetReferenceAudioClip RB_musicAssetReference;
    
    // Start is called before the first frame update
    void Start()
    {
        Addressables.InitializeAsync().Completed += AddressableManager_Completed;
    }

    private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        RB_musicAssetReference.LoadAsset<AudioClip>().Completed += (clip) =>
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip.Result;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.Play();
        };
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
