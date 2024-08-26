using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RB_LevelChoice : RB_Portal
{
    [Header("Component")]
    [SerializeField] private TextMeshProUGUI _levelNameDisplay;

    protected override void Start()
    {
        base.Start();

        OpenPortal();
        _levelNameDisplay.text = _nextSceneName.ToString();
    }
}
