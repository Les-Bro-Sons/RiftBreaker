using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RB_UxWeaponControl : MonoBehaviour
{
    [SerializeField] private GameObject _cursor;
    [SerializeField] private List<GameObject> _listWeapon = new();
    private int _lastItemsId;


    private void Update()
    {
        UxUpdate();
    }

    private void UxUpdate()
    {
        if (_lastItemsId != RB_PlayerAction.Instance.ItemId)
        {
            _lastItemsId = RB_PlayerAction.Instance.ItemId;    //item selectionne

            Vector3 directionToTarget = _listWeapon[_lastItemsId].transform.position - _cursor.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
            _cursor.transform.rotation = targetRotation;
        }
    }
}