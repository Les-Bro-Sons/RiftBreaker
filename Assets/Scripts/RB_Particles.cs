using UnityEngine;

public class RB_Particles : MonoBehaviour
{
    [SerializeField] private float _delayBeforeDeleting = 0;

    private void Start()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, particleSystem.main.startLifetime.constantMax + _delayBeforeDeleting);
    }
}
