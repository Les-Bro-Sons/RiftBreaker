using UnityEditor;
using UnityEngine;

public class RB_MusicNoteZone : MonoBehaviour
{
    //Properties
    private ZonePropertiesClass _zoneProperties = new();
    private Vector3 _takeAwayDirection;
    private Vector3 _defaultPos;

    //Conditions
    [SerializeField] private bool _shouldAnimate = false;
    [SerializeField] private bool _shouldTakeAway = false;
    private bool _isInit = false;

    //Components
    [Header("Components")]
    [SerializeField] private Transform _musicNoteSpriteParent;
    private Transform _musicNote;
    private Transform _musicNoteSprite;
    private Transform _playerTransform;
    private RB_CollisionDetection _collisionDetection;

    private void Awake()
    {
        _musicNote = transform;
        _musicNoteSprite = _musicNote.GetComponentInChildren<SpriteRenderer>().transform;
        _collisionDetection = GetComponentInChildren<RB_CollisionDetection>();
    }

    public void IntializeProperties(ZonePropertiesClass zoneProperties)
    {
        if (!_isInit)
        {
            _zoneProperties = zoneProperties;

            _playerTransform = RB_PlayerMovement.Instance.transform;
            _zoneProperties.Height = Random.Range(.1f, _zoneProperties.Height);
            _zoneProperties.UpDownSpeed = Random.Range(.1f, _zoneProperties.UpDownSpeed);
            _zoneProperties.RotationSpeed = Random.Range(-_zoneProperties.RotationSpeed, _zoneProperties.RotationSpeed);
            _zoneProperties.MaxMusicNoteDistance = Random.Range(.1f, _zoneProperties.MaxMusicNoteDistance);
            _defaultPos = _musicNote.position;
            _takeAwayDirection = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f)).normalized;
            _collisionDetection.EventOnEnemyEntered.AddListener(OnEnemyEntered);
        }
        
    }

    private void OnEnemyEntered()
    {
        foreach(GameObject detectedEnemy in _collisionDetection.GetDetectedEnnemies())
        {
            if (RB_Tools.TryGetComponentInParent(detectedEnemy, out RB_Health enemyHeath))
            {
                print(_zoneProperties.Damages);
                enemyHeath.TakeDamage(_zoneProperties.Damages);
            }
        }
        
    }

    private void Update()
    {
        Animate();
        TakeAway();
    }

    private void Animate()
    {
        if (_shouldAnimate)
        {
            AnimateUpDownLeftRight();
            //AnimateLeftRight();
        }
    }

    private void StartAnimate()
    {
        _defaultPos = _musicNote.position;
        _shouldAnimate = true;
    }

    private void TakeAway()
    {
        if (_shouldTakeAway)
        {
            _musicNoteSpriteParent.localPosition += _takeAwayDirection * Time.deltaTime * _zoneProperties.TakeAwaySpeed;
            if(Mathf.Abs(_musicNoteSpriteParent.localPosition.magnitude) >= _zoneProperties.MaxMusicNoteDistance)
                _shouldTakeAway = false;
        }
    }

    public void StopTakeAway()
    {
        _shouldTakeAway = false;
    }

    private void AnimateUpDownLeftRight()
    {
        _musicNoteSprite.localPosition = _defaultPos + Vector3.up * Mathf.Sin(Time.time * _zoneProperties.UpDownSpeed) * _zoneProperties.Height;
        _musicNote.Rotate(Vector3.up * Time.deltaTime * _zoneProperties.RotationSpeed);
        _musicNote.position = _playerTransform.position;
    }
}

[System.Serializable]
public class ZonePropertiesClass
{
    public float Height;
    public float UpDownSpeed;
    public float RotationSpeed;
    public float TakeAwaySpeed;
    public float MaxMusicNoteDistance;
    public int NoteAmount;
    [HideInInspector] public float Damages;

    public ZonePropertiesClass Copy()
    {
        return new ZonePropertiesClass()
        {
            Height = this.Height,
            UpDownSpeed = this.UpDownSpeed,
            RotationSpeed = this.RotationSpeed,
            TakeAwaySpeed = this.TakeAwaySpeed,
            MaxMusicNoteDistance = this.MaxMusicNoteDistance,
            Damages = this.Damages
        };
    }
}
