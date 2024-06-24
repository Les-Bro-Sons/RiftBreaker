using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_UxHourglassManager : MonoBehaviour
{
    public static RB_UxHourglassManager Instance;

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

    private void Update()
    {
        // RB_TimeManager.Instance.RewindLeft
        // si RewindLeft est superieur a la liste, on ajoute 
        // si RewindLeft est inferieur a la liste, on efface
        // si RewindLeft est egale a la liste, on ne fait rien

        if (RB_PlayerAction.Instance.RewindLeft <= 0)
            RB_PlayerAction.Instance.RewindLeft = 0;

        if (RB_PlayerAction.Instance.RewindLeft != RB_TimeManager.Instance.HourglassList.Count)
        {
            if (RB_PlayerAction.Instance.RewindLeft > RB_TimeManager.Instance.HourglassList.Count)
            {
                AddHourglasses(RB_PlayerAction.Instance.RewindLeft - RB_TimeManager.Instance.HourglassList.Count);
            }
            else if (RB_PlayerAction.Instance.RewindLeft < RB_TimeManager.Instance.HourglassList.Count)
            {
                RemoveHourglasses(RB_TimeManager.Instance.HourglassList.Count - RB_PlayerAction.Instance.RewindLeft);
            }
        }
    }

    public void NumberOfHourglass(int count)
    {
        RB_PlayerAction.Instance.RewindLeft = count;
    }

    private void AddHourglasses(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newHourglass = Instantiate(_prefabHourglass, _hourglassHolster.transform);
            newHourglass.name = $"Hourglass {RB_TimeManager.Instance.HourglassList.Count}";

            StartCoroutine(StartCreateHourglass(newHourglass));

            RB_TimeManager.Instance.HourglassList.Add(newHourglass);
        }
    }

    private void RemoveHourglasses(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject hourglassToRemove = RB_TimeManager.Instance.HourglassList[RB_TimeManager.Instance.HourglassList.Count - 1];
            StartCoroutine(StartUseHourglassUx(hourglassToRemove));

            RB_TimeManager.Instance.HourglassList.RemoveAt(RB_TimeManager.Instance.HourglassList.Count - 1);
            //Destroy(hourglassToRemove);
        }
        
    }

    private IEnumerator StartUseHourglassUx(GameObject hourglass)
    {
        CanvasGroup canvasHourglassToRemove = hourglass.GetComponent<CanvasGroup>();

        float currentRotation = 0f;
        float elapsedTime = 0f;

        while (currentRotation < _targetUsedRotation)
        {
            float rotationThisFrame = (_targetUsedRotation / _rotationDuration) * Time.unscaledDeltaTime;
            hourglass.transform.localRotation *= Quaternion.Euler(0f, 0f, rotationThisFrame); // rotation delta
            currentRotation += rotationThisFrame;
            yield return null;
        }

        //yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorStateInfo(0).length);

        while (elapsedTime < _alphaDuration)
        {
            canvasHourglassToRemove.alpha -= Time.unscaledDeltaTime / _alphaDuration;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Destroy(hourglass);
        //if (RB_TimeManager.Instance.RewindLeft <= 0)
        //joue animation de fin
    }

    private IEnumerator StartCreateHourglass(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        obj.transform.localRotation = Quaternion.Euler(0f, 0f, _targetUsedRotation);

        float currentRotation = _targetUsedRotation;
        float elapsedTime = 0f;

        if (canvasGroup.alpha > 0f)
            canvasGroup.alpha = 0f;


        while (elapsedTime < _alphaDuration || currentRotation > _targetNotUsedRotation)
        {
            if (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime / _alphaDuration;
            }

            if (currentRotation > _targetNotUsedRotation)
            {
                float rotationThisFrame = (_targetUsedRotation / _rotationDuration) * Time.unscaledDeltaTime;
                obj.transform.localRotation *= Quaternion.Euler(0f, 0f, -rotationThisFrame);
                currentRotation -= rotationThisFrame;
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        obj.transform.localRotation = Quaternion.Euler(0f, 0f, _targetNotUsedRotation);
    }
}