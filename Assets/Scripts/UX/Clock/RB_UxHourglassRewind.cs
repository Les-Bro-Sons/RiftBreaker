using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_UxHourglass : MonoBehaviour
{
    public static RB_UxHourglass Instance;

    public GameObject _prefabHourglass;
    public GameObject _hourglassHolster;

    [SerializeField] private float _rotationDuration = 1f;
    [SerializeField] private float _alphaDuration = 1f;
    [SerializeField] private float _targetUsedRotation = 180f;
    [SerializeField] private float _targetNotUsedRotation = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void CreateMaxNumberOfHourglass()
    {
        if (RB_TimeManager.Instance.HourglassList.Count > 0)
        {
            for (int i = 0; i < RB_TimeManager.Instance.HourglassList.Count; i++)
            {
                Destroy(RB_TimeManager.Instance.HourglassList[i].gameObject);
            }
            RB_TimeManager.Instance.HourglassList.Clear();
        }

        StartCoroutine(EventStartCreateHourglass(intervalTime: 0.5f));
    }

    public void CreateOneHourglass()
    {
        GameObject obj = Instantiate(_prefabHourglass, _hourglassHolster.transform);
        RB_TimeManager.Instance.HourglassList.Add(obj);
    }

    public void StartUseHourglassUx()
    {
        GameObject lastHourglass = RB_TimeManager.Instance.HourglassList[RB_PlayerAction.Instance.RewindLeft - 1];

        CanvasGroup canvasGroup = lastHourglass.GetComponent<CanvasGroup>();
        if (canvasGroup != null && canvasGroup.alpha != 0f)
        {
            StartCoroutine(StartHourglassUx(lastHourglass, canvasGroup));
        }
        else
        {
            Debug.LogWarning("Le dernier sablier a une valeur d'alpha égale à 0 !");
        }
    }

    public void StopUseHourglassUx()
    {
        StopAllCoroutines();
        StartCoroutine(StopHourglassUx());
    }

    private IEnumerator StartHourglassUx(GameObject hourglass, CanvasGroup canvasGroup)
    {
        float currentRotation = 0f;
        float elapsedTime = 0f;

        while (currentRotation < _targetUsedRotation)
        {
            float rotationThisFrame = (_targetUsedRotation / _rotationDuration) * Time.deltaTime;
            hourglass.transform.localRotation *= Quaternion.Euler(0f, 0f, rotationThisFrame); // rotation delta
            currentRotation += rotationThisFrame;
            yield return null;
        }

        //yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        while (elapsedTime < _alphaDuration)
        {
            canvasGroup.alpha -= Time.deltaTime / _alphaDuration;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //if (RB_TimeManager.Instance.NumberOfRewind <= 0)
            //joue animation de fin
    }

    private IEnumerator EventStartCreateHourglass(float intervalTime)
    {
        for (int i = 0; i < RB_PlayerAction.Instance.RewindLeft; i++)
        {
            StartCoroutine(StartCreateHourglass(i));
            yield return new WaitForSeconds(intervalTime);
        }
    }

    private IEnumerator StartCreateHourglass(int indexHourglass)
    {
        GameObject obj = Instantiate(_prefabHourglass, _hourglassHolster.transform);
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        obj.name = $"Hourglass {indexHourglass}";
        obj.transform.localRotation = Quaternion.Euler(0f, 0f, _targetUsedRotation);

        float currentRotation = _targetUsedRotation;
        float elapsedTime = 0f;

        if (canvasGroup.alpha > 0f)
            canvasGroup.alpha = 0f;


        while (elapsedTime < _alphaDuration || currentRotation > _targetNotUsedRotation)
        {
            if (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime / _alphaDuration;
            }

            if (currentRotation > _targetNotUsedRotation)
            {
                float rotationThisFrame = (_targetUsedRotation / _rotationDuration) * Time.deltaTime;
                obj.transform.localRotation *= Quaternion.Euler(0f, 0f, -rotationThisFrame);
                currentRotation -= rotationThisFrame;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        obj.transform.localRotation = Quaternion.Euler(0f, 0f, _targetNotUsedRotation);
        RB_TimeManager.Instance.HourglassList.Add(obj);
    }

    private IEnumerator StopHourglassUx()
    {
        yield return null;
    }
}