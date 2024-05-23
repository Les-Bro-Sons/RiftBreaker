using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RB_Particles : MonoBehaviour
{
    private new Transform transform;

    [Header("Destroy Settings")]
    [SerializeField] private bool _isDestroyedOnStart = true;
    [SerializeField] private float _delayBeforeDeleting = 0;
    //[SerializeField] private bool _isExploding = true;

    [Header("Attached to Object Settings")]
    [SerializeField] public Transform FollowObject;
    [SerializeField] private bool _stopOnObjectDestroyed = true;


    private ParticleSystem _particles;

    private bool _followedObject = false;


    private void Awake()
    {
        transform = GetComponent<Transform>();
        _particles = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        if (_isDestroyedOnStart)
        {
            MainModule particleMain = _particles.main;
            particleMain.loop = false;
            Destroy(gameObject, particleMain.startLifetime.constantMax + particleMain.duration + _delayBeforeDeleting);
        }
    }

    private void Update()
    {
        if (FollowObject)
        {
            _followedObject = true;
            transform.position = FollowObject.position;
            transform.rotation = FollowObject.rotation;
        }
        else if (_followedObject)
        {
            if (_stopOnObjectDestroyed)
            {
                MainModule particleMain = _particles.main;
                particleMain.loop = false;
                _particles.Stop();
                Destroy(gameObject, particleMain.startLifetime.constantMax + particleMain.duration + _delayBeforeDeleting);
            }
        }
    }
}
