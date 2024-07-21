using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class RB_DifficultyManager : MonoBehaviour
{
    //Instance
    public static RB_DifficultyManager Instance;

    //Difficulties
    [SerializedDictionary("Phases", "Difficulty")] public SerializedDictionary<PHASES, DIFFICULTY> _difficultiesByPhase; //All the difficulty of the phases
    private DIFFICULTY _currentDifficulty; //The difficulty of the current phase

    //Awake
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    //Start
    private void Start()
    {
        RB_LevelManager.Instance.EventSwitchPhase.AddListener(OnSwitchPhase);
        ChangeCurrentDifficulty();
    }

    private void OnSwitchPhase() //When the event EventSwitchPhase is called
    {
        ChangeCurrentDifficulty();
    }

    private void ChangeCurrentDifficulty() //Put the current difficulty to the one of the current phase
    {
        _currentDifficulty = _difficultiesByPhase[RB_LevelManager.Instance.CurrentPhase];
    }


    public DIFFICULTY GetDifficulty(PHASES phase) //Get the difficulty of the phase asked
    {
        return _difficultiesByPhase[phase]; 
    }

    public DIFFICULTY GetCurrentDifficulty() //Get the difficulty of the current phase
    {
        return _currentDifficulty;
    }

    public void SetDifficulty(PHASES phase,  DIFFICULTY difficulty) //Set the difficulty of the phase asked to the difficulty asked
    {
        _difficultiesByPhase[phase] = difficulty;
    }

    public void SetDifficulty(DIFFICULTY difficulty) //Set all the difficulties to the difficulty asked
    {
        foreach(var phase in _difficultiesByPhase.Keys)
        {
            _difficultiesByPhase[phase] = difficulty;
        }
    }
}
