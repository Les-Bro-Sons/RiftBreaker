using UnityEngine;
using UnityEngine.Splines;

public class RB_SplineManager : MonoBehaviour
{
    public static RB_SplineManager Instance;
    public static SplineContainer Splines;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Splines = GetComponent<SplineContainer>();
        }
    }
}
