using UnityEngine;

public class RB_ColliderAnim : MonoBehaviour
{

    [SerializeField] Animator _collisionAnimations;

    public void StopAnimation(string AnimationToStop)
    {
        //Stop the animation
        _collisionAnimations.SetBool(AnimationToStop, false);
    }
}
