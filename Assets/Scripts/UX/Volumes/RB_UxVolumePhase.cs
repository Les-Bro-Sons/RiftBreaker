using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RB_UxVolumePhase : MonoBehaviour
{
    public static RB_UxVolumePhase Instance;
    [Header("UX")]
    [SerializeField] private float _durationToSwitch = 1f;
    [SerializedDictionary("Phases", "Volume")]
    public AYellowpaper.SerializedCollections.SerializedDictionary<PHASES, Volume> UxPhaseColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ActionUxSwitchPhase(canTouchLastLastPhase: false);
    }

    public void ActionUxSwitchPhase (float duration = 1f, bool canTouchLastLastPhase = true) 
    {
        StopAllCoroutines();
        StartCoroutine(SwitchPhaseEffect(duration, canTouchLastLastPhase));
    }

    private IEnumerator SwitchPhaseEffect(float duration, bool canTouchLastLastPhase)
    {
        float elapsedTime = 0f;

        float last_StartWeight = canTouchLastLastPhase ? UxPhaseColor[RB_LevelManager.Instance.LastPhase].weight : 0f;
        float current_StartWeight = UxPhaseColor[RB_LevelManager.Instance.CurrentPhase].weight;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            if (canTouchLastLastPhase)
                UxPhaseColor[RB_LevelManager.Instance.LastPhase].weight = Mathf.Lerp(last_StartWeight, 0, t);
            UxPhaseColor[RB_LevelManager.Instance.CurrentPhase].weight = Mathf.Lerp(current_StartWeight, 1, t);

            yield return null;
        }

        if (canTouchLastLastPhase)
            UxPhaseColor[RB_LevelManager.Instance.LastPhase].weight = 0;
        UxPhaseColor[RB_LevelManager.Instance.CurrentPhase].weight = 1;
    }
}