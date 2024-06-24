using UnityEngine;

public class RB_RobertAnim : MonoBehaviour
{
    private Animator _robertAnimator;
    public enum CurrentAnimation { Angry, AngryNeutral, Bruh, BruhAnnoyed, CloseEyes, CloseEyesSad, CloseEyesSmile, EvilSmile, Happy, Neutral, Sad, SadNeutral, SadSmile, Smile}

    private void Awake()
    {
        _robertAnimator = GetComponent<Animator>();
    }

    public void StartTalk(CurrentAnimation animation)
    {
        _robertAnimator.SetFloat("CurrentAnimation", (int)animation);
        _robertAnimator.SetBool("Talking", true);
    }

    public void SetAnimation(CurrentAnimation animation)
    {
        _robertAnimator.SetFloat("CurrentAnimation", (int)animation);
    }

    public void StartTalk()
    {
        _robertAnimator.SetBool("Talking", true);
    }

    public void StartIdle(CurrentAnimation animation)
    {
        _robertAnimator.SetFloat("CurrentAnimation", (int)animation); 
        _robertAnimator.SetBool("Talking", false);
    }

    public void StopTalk()
    {
        _robertAnimator.SetBool("Talking", false);
    }
}
