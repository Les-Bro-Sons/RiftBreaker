using UnityEngine;
using UnityEngine.SceneManagement;

public class RB_BossRushManager : MonoBehaviour
{
    public static RB_BossRushManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RB_PlayerController.Instance.GetComponent<RB_Health>().Hp = RB_SaveManager.Instance.SaveObject.HpBossRush;
        RB_LevelExit.Instance.EventEnterInPortal.AddListener(SaveHp);
        RB_LevelManager.Instance.EventPlayerLost.AddListener(StopBossRush);
    }

    public void SaveHp()
    {
        RB_SaveManager.Instance.SaveObject.HpBossRush = RB_PlayerController.Instance.GetComponent<RB_Health>().Hp;
    }

    public void StopBossRush()
    {
        RB_SceneTransitionManager.Instance.NewTransition(FADETYPE.Rift, 0);
    }
}
