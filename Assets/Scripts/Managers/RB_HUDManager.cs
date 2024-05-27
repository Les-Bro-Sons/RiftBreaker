using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_HUDManager : MonoBehaviour  {

    [SerializeField] HUDTYPE _currentHUD;

    private Canvas _canvas;

    private void RefreshHud(){

    }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        _canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        RefreshHud();
    }
}
