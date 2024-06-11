using UnityEngine;

public class RB_HUDManager : MonoBehaviour  
{
    public static RB_HUDManager Instance;

    public Animator AnimatorHud;

    public RB_HUDHealthBar BossHealthBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        AnimatorHud = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        AnimatorHud.Play(animationName);
    }
}