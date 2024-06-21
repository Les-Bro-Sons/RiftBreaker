using System.Collections.Generic;
using UnityEngine;

public class RB_PlayerWeaponDetecter : MonoBehaviour
{
    //Components
    [SerializeField] private CanvasGroup _pressToGather;

    //Properties
    private RB_Items _currentItem;


    private List<RB_FadeEffect> _fadeEffects = new List<RB_FadeEffect>();

    private void Start()
    {
        _pressToGather.alpha = 0;
    }

    private void Update()
    {
        FadeEffect();
    }

    public void FadeEffect() //Updating fading out and in effect
    {
        for (int i = _fadeEffects.Count - 1; i >= 0; i--)
        {
            _fadeEffects[i].UpdateFade();
            if (_fadeEffects[i].IsComplete)
            {
                _fadeEffects.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent(other.gameObject, out RB_Items item)) //if a weapon enter the weapon detecter
        {
            _currentItem = item;
            OnWeaponEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent(other.gameObject, out RB_Items item) && item == _currentItem) //if the exited weapon is the same as the current one
        {
            OnWeaponExit();
            _currentItem = null;
        }
    }

    private void OnWeaponEnter() //When a weapon enter the trigger
    {
        RB_FadeEffect fadeEffect = new RB_FadeEffect(_pressToGather, 5, FadeType.In);
        _fadeEffects.Add(fadeEffect); //Start fade in effect
        _currentItem.EventOnItemGathered.AddListener(OnWeaponExit); //Add listener to the gathering item
    }

    private void OnWeaponExit() //When a weapon exit the trigger
    {
        RB_FadeEffect fadeEffect = new RB_FadeEffect(_pressToGather, 5, FadeType.Out);
        _fadeEffects.Add(fadeEffect); //Start the fade out effect
        _currentItem.EventOnItemGathered.RemoveListener(OnWeaponEnter); //Remove the listener from the gathering item
    }
}
