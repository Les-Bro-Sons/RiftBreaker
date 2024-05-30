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
    [SerializeField] private float _targetRotation = 180f;

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
        /*if (RB_TimeManager.Instance.HourglassList.Count == 0)
        {        }
        else // ajoute le manque*/
        
        for (int i = 0; i < RB_TimeManager.Instance.NumberOfRewind; i++)
        {
            GameObject obj = Instantiate(_prefabHourglass, _hourglassHolster.transform);
            obj.name = $"Hourglass {i}";
            RB_TimeManager.Instance.HourglassList.Add(obj);
        }

    }    
    
    public void CreateOneHourglass()
    {
        GameObject obj = Instantiate(_prefabHourglass, _hourglassHolster.transform);
        RB_TimeManager.Instance.HourglassList.Add(obj);
    }

    public void StartUseHourglassUx()
    {
        GameObject lastHourglass = RB_TimeManager.Instance.HourglassList[RB_TimeManager.Instance.NumberOfRewind - 1];

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

        while (currentRotation < _targetRotation)
        {
            float rotationThisFrame = (_targetRotation / _rotationDuration) * Time.deltaTime;
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

    private IEnumerator StopHourglassUx()
    {
        yield return null;
    }
}