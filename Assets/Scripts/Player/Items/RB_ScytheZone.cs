using UnityEngine;

public class RB_ScytheZone : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;
    private ParticleSystem.ShapeModule _partShape;

    private void Start()
    {
        _partShape = _particles.shape;
    }

    private void Update()
    {
        _partShape.radius = transform.lossyScale.x; //set the radius of particles
    }
}
