using UnityEngine;

public class RB_Door : MonoBehaviour
{
    private Animator _doorAnimator;
    public bool IsControledByRoom = true;

    private void Awake()
    {
        _doorAnimator = GetComponent<Animator>();   
    }

    public void Open()
    {
        _doorAnimator.SetTrigger("Down");
    }

    public void Close()
    {
        _doorAnimator.SetTrigger("Up");
    }
}
