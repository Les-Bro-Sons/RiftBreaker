using UnityEngine;

public class RB_ParticleRotationOffset : MonoBehaviour
{
    [SerializeField] float rotOffset = 90;

    private void Awake()
    {
        ParticleSystem particles = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainPart = particles.main;
        mainPart.startRotation = Mathf.Deg2Rad * (rotOffset + transform.eulerAngles.y);
    }
}
