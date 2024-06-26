using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RB_UxWeaponControl : MonoBehaviour
{
    //Components
    [Header("Components")]
    RectTransform _cursorTransform;
    [SerializeField] private GameObject _cursor;
    [SerializeField] private List<Image> _listWeapon = new();
    [SerializeField] private Image _currentWeapon;
    private int _lastItemsId;

    //Properties
    [Header("Properties")]
    [SerializeField] private float _lerpSpeed;
    private Material _defaultMaterial;
    private bool _shouldChangeWeapon = false;
    private Quaternion _currentRotation;
    private float _lerpTime;
    private int _deltaWeaponIndex;
    private bool _weaponInitialized;

    //Fading
    private bool _shouldFadeIn = false;
    private bool _shouldFadeOut = false;
    [SerializeField] private float _fadeTime;

    //Outline
    [SerializeField] private Material _outlineMaterial;

    private void Awake()
    {
        //Get the rect transform of the cursor
        _cursorTransform = _cursor.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _currentWeapon.sprite = null;
        //Bind to the event called when an item is gathered
        RB_PlayerAction.Instance.EventItemGathered.AddListener(RefreshSprites);
        RB_PlayerAction.Instance.EventItemDropped.AddListener(RefreshSprites);

        //Get the angle out of the direction of the first item slot
        Vector3 direction = _listWeapon[0].transform.position - _cursorTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        //Set that angle to the cursor
        _cursorTransform.localRotation = Quaternion.Euler(0, 0, angle);

        //Get the default material of the weapons
        _defaultMaterial = _listWeapon[0].material;

        //Set the thickness of the outline mat to 1
        _outlineMaterial.SetFloat("_Thickness", 1);

        Invoke(nameof(InitializeWeapons), RB_HUDManager.Instance.AnimatorHud.GetCurrentAnimatorClipInfo(0).Length);
    }

    private void InitializeWeapons()
    {
        RefreshSprites();
        RB_PlayerController.Instance.ChoseItem(0);
        _weaponInitialized = true;
    }

    private void RefreshSprites()
    {
        for (int i = 0; i < _listWeapon.Count; i++)
        {
            AddSprite(i);
        }
    }

    private void StartFade()
    {
        //Start the fade out
        _shouldFadeIn = false;
        _shouldFadeOut = true;
    }

    private void FadeIn()
    {
        //Adds alpha to the color of the currently selectionned weapon 
        _currentWeapon.color += new Color(0, 0, 0, Time.deltaTime/ (_fadeTime / 2));
        
        //Adds thickness to the outline mat
        _outlineMaterial.SetFloat("_Thickness", _outlineMaterial.GetFloat("_Thickness") + Time.deltaTime / (_fadeTime / 2));
        if (_currentWeapon.color.a >= 1)
        {
            //When the alpha is full, stops the fade in
            _shouldFadeIn = false;
        }
    }

    private void FadeOut()
    {
        //Removes alpha to the color of the currently selectionned weapon 
        _currentWeapon.color -= new Color(0, 0, 0, Time.deltaTime / (_fadeTime / 2));

        //Removes thickness to the outline mat
        _outlineMaterial.SetFloat("_Thickness", _outlineMaterial.GetFloat("_Thickness") - Time.deltaTime / (_fadeTime / 2));
        if (_currentWeapon.color.a <= 0)
        {
            //When the alpha is full, set the sprite of the current weapon to the slectionned weapon
            _currentWeapon.sprite = _listWeapon[_lastItemsId].sprite;

            //Set the outline to the currently selectionned weapon
            SetMaterialOnCurrentWeapon(_lastItemsId);

            //Stops the fade in and start the fade out
            _shouldFadeOut = false;
            _shouldFadeIn = true;
        }
    }

    private void SetMaterialOnCurrentWeapon(int weaponId)
    {
        //Set the outline material to the chosed weapon and removes it from all the others
        foreach(Image weapon in  _listWeapon)
        {
            weapon.material = _defaultMaterial;
        }
        _listWeapon[weaponId].material = _outlineMaterial;

    }

    private void Fade()
    {
        if (_shouldFadeIn)
        {
            FadeIn();
        }

        if (_shouldFadeOut)
        {
            FadeOut();
        }
    }

    private void Update()
    {
        UxUpdate();
        Fade();
    }

    private void AddSprite(int id)
    {
        if (RB_PlayerAction.Instance.Items.Count <= id)
        {
            //if weapon is null, remove the sprite
            _listWeapon[id].sprite = null;
            _listWeapon[id].material = null;
            _currentWeapon.color = new Color(0, 0, 0, 0);
            //Same to the current weapon slot
            _listWeapon[id].color = new Color(0, 0, 0, 0);
        }
        else
        {
            //When an item is gathered, add it to the weapon slots
            _listWeapon[id].sprite = RB_PlayerAction.Instance.Items[id].CurrentSprite;
            _listWeapon[id].transform.localScale = Vector3.one * RB_PlayerAction.Instance.Items[id].ScaleSpriteHUD;
            SetMaterialOnCurrentWeapon(_lastItemsId);
            //For safety, put the color to full white
            _currentWeapon.color = Color.white;
            //Same to the current weapon slot
            _listWeapon[id].color = Color.white;
        }

        //Set the current weapon the newly gathered weapon
        _currentWeapon.sprite = _listWeapon[_lastItemsId].sprite;

        

        
    }

    private void ChangeWeapon()
    {
        //Start the lerp rotation of the cursor towards the selectionned weapon slot
        _shouldChangeWeapon = true;

        //Initialize the rotation, the lerp time and start the fade in fade out
        _currentRotation = _cursorTransform.localRotation;
        _lerpTime = 0;
        StartFade();
    }

    private void UxUpdate()
    {
        if (_lastItemsId != RB_PlayerAction.Instance.ItemId && _weaponInitialized)
        {
            //Get the difference between the current weapon id and the last item id to have a constant lerp speed even when the switch is 1 to 3
            _deltaWeaponIndex = Mathf.Abs(_lastItemsId - RB_PlayerAction.Instance.ItemId);
            _lastItemsId = RB_PlayerAction.Instance.ItemId;
            ChangeWeapon();
        }
        if (_shouldChangeWeapon)
        {
            //Make a lerp rotation towards the selectionned weapon
            _lerpTime += (Time.deltaTime * _lerpSpeed) / _deltaWeaponIndex;
            Vector3 direction = _listWeapon[_lastItemsId].transform.position - _cursorTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            _cursorTransform.localRotation = Quaternion.Slerp(_currentRotation, Quaternion.Euler(0, 0, angle), _lerpTime);
            if (Quaternion.Angle(_cursorTransform.localRotation, Quaternion.Euler(0, 0, angle)) < .1f)
            {
                //when the rotation is finished, stop the change weapon
                _shouldChangeWeapon = false;
            }
        }
    }
}