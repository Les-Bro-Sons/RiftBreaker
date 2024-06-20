using UnityEngine;

public class RB_GroundManager : MonoBehaviour
{
    public static RB_GroundManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
