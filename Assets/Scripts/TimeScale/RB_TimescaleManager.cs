using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RB_TimescaleManager : MonoBehaviour
{
    public static RB_TimescaleManager Instance;

    public Dictionary<string, TimescaleModifier> Modifiers = new();

    private float _lastRefreshTime = 0;
    [SerializeField] private float _refreshDelay = 1;

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
        SetModifier(gameObject, "BaseTimescale", 1, 0);
    }

    private void Update()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, _currentTimescaleTarget, _currentLerpSpeed * Time.unscaledDeltaTime);
        if (Mathf.Abs(Time.timeScale - _currentTimescaleTarget) < 0.01f)
        {
            Time.timeScale = _currentTimescaleTarget;
        }
        if (Time.unscaledTime > _lastRefreshTime + _refreshDelay)
        {
            _lastRefreshTime = Time.unscaledTime;
            HasModifierNullOwner(true, true);
        }
    }

    private bool HasModifierNullOwner(bool remove = true, bool refreshCurrentTarget = true)
    {
        bool nullOwner = false;

        Dictionary<string, TimescaleModifier>.KeyCollection modifierKeys = Modifiers.Keys;
        foreach (string key in modifierKeys.ToList())
        {
            if (Modifiers[key].Owner == null)
            {
                if (remove) Modifiers.Remove(key);
                nullOwner = true;
            }
        }

        if (nullOwner && refreshCurrentTarget) RefreshCurrentTimescaleTarget(); 
        return nullOwner;
    }

    private void RefreshCurrentTimescaleTarget()
    {
        List<float> timescaleTargets = new();
        List<float> lerpSpeeds = new();
        float highestPriority = float.MinValue;

        Dictionary<string, TimescaleModifier>.KeyCollection modifierKeys = Modifiers.Keys;
        foreach(string key in modifierKeys.ToList())
        {
            TimescaleModifier modifier = Modifiers[key];
            if (modifier.Owner == null) Modifiers.Remove(key);

            if (modifier.Priority > highestPriority) //check the highest priority modifier
            {
                timescaleTargets.Clear();
                lerpSpeeds.Clear();
                highestPriority = modifier.Priority;
                timescaleTargets.Add(modifier.TimescaleTarget);
                lerpSpeeds.Add(modifier.LerpSpeed);
            }
            else if (modifier.Priority == highestPriority) //if there's more than one modifier of a priority
            {
                timescaleTargets.Add(modifier.TimescaleTarget);
                lerpSpeeds.Add(modifier.LerpSpeed);
            }
        }

        if (timescaleTargets.Count == 1) //if there's only one modifier of this priority
        {
            _currentTimescaleTarget = timescaleTargets[0];
            _currentLerpSpeed = lerpSpeeds[0];
        }
        else if (timescaleTargets.Count > 1) //does an average of the timescales value of the same priority
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

    public void SetModifier(GameObject owner, string id, float timescale, float priority, float lerpSpeed = 10)
    {
        TimescaleModifier modifier = new TimescaleModifier();
        modifier.TimescaleTarget = timescale;
        modifier.Priority = priority;
        modifier.LerpSpeed = lerpSpeed;
        modifier.Owner = owner;
        SetModifier(id, modifier);
    }

    public void SetModifier(string id, TimescaleModifier modifier)
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
