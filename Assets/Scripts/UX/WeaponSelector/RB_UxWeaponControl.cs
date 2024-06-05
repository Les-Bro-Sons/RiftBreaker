using System.Collections.Generic;
using UnityEngine;

public class RB_UxWeaponControl : MonoBehaviour
{
    [SerializeField] private GameObject _cursor;
    [SerializeField] private List<GameObject> _listWeapon = new();
    private int _lastItemsId;

    //Components
    RectTransform _cursorTransform;

    //Properties
    [SerializeField] private float _lerpSpeed;
    private bool _shouldChangeWeapon = false;
    private Quaternion _currentRotation;
    private float _lerpTime;
    private int _deltaWeaponIndex;

    private void Awake()
    {
        _cursorTransform = _cursor.GetComponent<RectTransform>();
    }

    private void Update()
    {
        UxUpdate();
    }

    private void ChangeWeapon()
    {
        _shouldChangeWeapon = true;
        _currentRotation = _cursorTransform.localRotation;
        _lerpTime = 0;
    }

    private void UxUpdate()
    {
        if (_lastItemsId != RB_PlayerAction.Instance.ItemId)
        {
            _deltaWeaponIndex = Mathf.Abs(_lastItemsId - RB_PlayerAction.Instance.ItemId);
            _lastItemsId = RB_PlayerAction.Instance.ItemId;
            ChangeWeapon();
        }
        if (_shouldChangeWeapon)
        {
            _lerpTime += (Time.deltaTime * _lerpSpeed) / _deltaWeaponIndex;
            Vector3 direction = _listWeapon[_lastItemsId].transform.position - _cursorTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            _cursorTransform.localRotation = Quaternion.Slerp(_currentRotation, Quaternion.Euler(0, 0, angle), _lerpTime);
            if (Quaternion.Angle(_cursorTransform.localRotation, Quaternion.Euler(0, 0, angle)) < .1f)
            {
                _shouldChangeWeapon = false;
            }
        }
    }
}