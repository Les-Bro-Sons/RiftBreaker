using UnityEngine;

public class RB_UxClockRewind : MonoBehaviour
{
    [SerializeField] private GameObject _objSecond;
    [SerializeField] private GameObject _objMiliSecond;
    private float _speedToReturn = 2f;

    void Start()
    {
        _objSecond.transform.localRotation= Quaternion.Euler(0, 0, 0);
        _objMiliSecond.transform.localRotation= Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        if (RB_TimeManager.Instance.IsRewinding)
        {
            EnableUx();
        }
        else if (_objSecond.transform.localRotation != Quaternion.Euler(0, 0, 0) ||
        _objMiliSecond.transform.localRotation != Quaternion.Euler(0, 0, 0))
        {
            DisableUx();
        }
    }

    private void EnableUx()
    {
        float remainingTimeSecond = RB_TimeManager.Instance.GetRewindRemainingTime();
        float remainingTimeMilliSecond = RB_TimeManager.Instance.GetRewindRemainingTime();

        _objSecond.transform.localRotation = Quaternion.Euler(0, 0, -360 * (remainingTimeSecond / RB_TimeManager.Instance.DurationRewind));
        _objMiliSecond.transform.localRotation = Quaternion.Euler(0, 0, -3600 * (remainingTimeMilliSecond / RB_TimeManager.Instance.DurationRewind));
    }

    private void DisableUx()
    {
        // Obtenez les angles actuels en degrés
        float angleSecond = _objSecond.transform.localEulerAngles.z;
        float angleMilliSecond = _objMiliSecond.transform.localEulerAngles.z;

        // Calculez l'angle le plus court pour l'interpolation
        angleSecond = (angleSecond > 180) ? angleSecond - 360 : angleSecond;
        angleMilliSecond = (angleMilliSecond > 180) ? angleMilliSecond - 360 : angleMilliSecond;

        // Interpolez vers l'angle le plus proche de 0
        _objSecond.transform.localRotation = Quaternion.RotateTowards(_objSecond.transform.localRotation, Quaternion.Euler(0, 0, angleSecond >= 0 ? 0 : 360), _speedToReturn * 36 * Time.deltaTime);
        _objMiliSecond.transform.localRotation = Quaternion.RotateTowards(_objMiliSecond.transform.localRotation, Quaternion.Euler(0, 0, angleMilliSecond >= 0 ? 0 : 360), _speedToReturn * 360 * Time.deltaTime);
    }
}