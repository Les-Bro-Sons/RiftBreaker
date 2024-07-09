using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_ExclamationMark : MonoBehaviour
{
    private new Transform transform;

    private int _stage = 0;
    private float _timer;

    [SerializeField] private float _appearingTime = 0.5f;
    [SerializeField] private float _disappearingDelay = 0.5f;
    [SerializeField] private float _disappearingTime = 0.5f;

    [SerializeField] private AnimationCurve _appearingCurve;
    [SerializeField] private AnimationCurve _disappearingCurve;

    [SerializeField] private Vector3 _spawnScale;
    [SerializeField] private Vector3 _appearingScale;
    [SerializeField] private Vector3 _disappearingScale;

    [SerializeField] private float _scaleMultiplier = 1f;

    private void Awake()
    {
        _spawnScale *= _scaleMultiplier;
        _disappearingScale *= _scaleMultiplier;
        _appearingScale *= _scaleMultiplier;

        transform = GetComponent<Transform>();
        transform.localScale = _spawnScale;
    }

    private void Update()
    {
        switch (_stage)
        {
            case 0:
                Appearing();
                break;
            case 1:
                DisappearingDelay();
                break;
            case 2:
                Disappearing();
                break;
        }
        _timer += Time.deltaTime;
    }

    private void Appearing()
    {
        transform.localScale = Vector3.Lerp(_spawnScale, _appearingScale, _timer / _appearingTime);
        if (_timer >= _appearingTime) NextStage();
    }

    private void DisappearingDelay()
    {
        if (_timer >= _disappearingDelay) NextStage();
    }

    private void Disappearing()
    {
        transform.localScale = Vector3.Lerp(_appearingScale, _disappearingScale, _timer / _appearingTime);
        if (_timer >= _disappearingTime) NextStage();
    }

    private void NextStage()
    {
        _stage += 1;
        _timer = 0;
    }
}
