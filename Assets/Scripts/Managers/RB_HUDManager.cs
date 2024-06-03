using UnityEngine;

public class RB_HUDManager : MonoBehaviour  
{
    public static RB_HUDManager Instance;

    private Animator _animatorHud;

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

        _animatorHud = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        _animatorHud.Play(animationName);
    }
}