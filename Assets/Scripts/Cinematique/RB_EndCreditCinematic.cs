using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_EndCreditCinematic : MonoBehaviour
{
    [SerializeField] private RectTransform _creditTransform;
    [SerializeField] private float _creditSpeed;

    private void Start()
    {
        RB_InputManager.Instance.EventAttackStarted.AddListener(OnStartCreditSkip);
        RB_InputManager.Instance.EventAttackCanceled.AddListener(OnStopCreditSkip);
    }

    private void Update()
    {
        _creditTransform.localPosition += Vector3.up * _creditSpeed * Time.deltaTime;
        if (_creditTransform.localPosition.y > 4600)
        {
            RB_SceneTransitionManager.Instance.NewTransition(RB_SceneTransitionManager.Instance.FadeType.ToString(), 0);
            RB_SaveManager.Instance.ResetSave();
        }
    }

    private void OnStartCreditSkip()
    {
        _creditSpeed *= 10;
    }

    private void OnStopCreditSkip()
    {
        _creditSpeed /= 10;
    }
}
