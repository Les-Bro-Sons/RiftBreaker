using MANAGERS;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_EndCreditCinematic : MonoBehaviour
{
    [SerializeField] private RectTransform _creditTransform;
    [SerializeField] private float _creditSpeed;

    private void Start()
    {
        RB_AudioManager.Instance.PlayMusic("Credit_Music");
    }
    private void Update()
    {
        _creditTransform.position += Vector3.up * _creditSpeed * Time.deltaTime;
        if (_creditTransform.position.y > 4600)
        {
            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 0);
        }
    }
}
