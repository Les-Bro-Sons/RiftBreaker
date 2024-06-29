using System.Collections.Generic;
using UnityEngine;

public class RB_TimescaleManager : MonoBehaviour
{
    public static RB_TimescaleManager Instance;

    public Dictionary<string, TimescaleModifier> Modifiers = new();

    private float _currentTimescaleTarget;
    private float _currentLerpSpeed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(gameObject);
        }
        AddModifier("BaseTimescale", 1, 0);
    }

    private void Update()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, _currentTimescaleTarget, _currentLerpSpeed * Time.unscaledDeltaTime);
        if (Mathf.Abs(Time.timeScale - _currentTimescaleTarget) < 0.05)
        {
            Time.timeScale = _currentTimescaleTarget;
        }
    }

    private void RefreshCurrentTimescaleTarget()
    {
        List<float> timescaleTargets = new();
        List<float> lerpSpeeds = new();
        float highestPriority = float.MinValue;

        Dictionary<string, TimescaleModifier>.KeyCollection modifierKeys = Modifiers.Keys;
        foreach(string key in modifierKeys)
        {
            TimescaleModifier modifier = Modifiers[key];
            if (modifier.Priority > highestPriority)
            {
                timescaleTargets.Clear();
                lerpSpeeds.Clear();
                highestPriority = modifier.Priority;
                timescaleTargets.Add(modifier.TimescaleTarget);
                lerpSpeeds.Add(modifier.LerpSpeed);
            }
            else if (modifier.Priority == highestPriority)
            {
                timescaleTargets.Add(modifier.TimescaleTarget);
                lerpSpeeds.Add(modifier.LerpSpeed);
            }
        }

        if (timescaleTargets.Count == 1)
        {
            _currentTimescaleTarget = timescaleTargets[0];
            _currentLerpSpeed = lerpSpeeds[0];
        }
        else if (timescaleTargets.Count > 1)
        {
            float averageTarget = 0;
            float averageLerpSpeed = 0;
            for (int i = 0; i < timescaleTargets.Count; i++)
            {
                averageTarget += timescaleTargets[i];
                averageLerpSpeed += lerpSpeeds[i];
            }
            averageTarget /= timescaleTargets.Count;
            averageLerpSpeed /= timescaleTargets.Count;
            _currentTimescaleTarget = averageTarget;
            _currentLerpSpeed = averageLerpSpeed;
        }
        else
        {
            Debug.LogWarning("There is no timescale Target in the TimescaleManager");
        }
    }

    public void AddModifier(string id, float timescale, float priority, float lerpSpeed = 10)
    {
        TimescaleModifier modifier = new TimescaleModifier();
        modifier.TimescaleTarget = timescale;
        modifier.Priority = priority;
        modifier.LerpSpeed = lerpSpeed;
        AddModifier(id, modifier);
    }

    public void AddModifier(string id, TimescaleModifier modifier)
    {
        if (Modifiers.ContainsKey(id)) Modifiers[id] = modifier;
        else Modifiers.Add(id, modifier);
        RefreshCurrentTimescaleTarget();
    }

    public void RemoveModifier(string id)
    {
        if (Modifiers.ContainsKey(id)) Modifiers.Remove(id);
        RefreshCurrentTimescaleTarget();
    }
}
