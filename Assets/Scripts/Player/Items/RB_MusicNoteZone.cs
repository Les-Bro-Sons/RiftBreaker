using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
    private bool _shouldDisapear = false;

    //Components
    [Header("Components")]
    [SerializeField] private Transform _musicNoteSpriteParent;
    private Transform _musicNoteTransform;
    private Transform _musicNoteSpriteTransform;
    private Transform _playerTransform;
    private RB_CollisionDetection _collisionDetection;
    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;

    //Events
    [HideInInspector] public UnityEvent EventOnDestroy;

    private void Awake()
    {
        _musicNoteTransform = transform;
        _musicNoteSpriteTransform = _musicNoteTransform.GetComponentInChildren<SpriteRenderer>().transform;
        _collisionDetection = GetComponentInChildren<RB_CollisionDetection>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _defaultColor = _spriteRenderer.color;
        _playerTransform = RB_PlayerMovement.Instance.transform; //Get the player transform
    }

    public void IntializeProperties(ZonePropertiesClass zoneProperties) //Init properties
    {
        if (!_isInit)
        {
            _zoneProperties = zoneProperties; //Set the properties to the note

            //Random the properties with a the default value being the max
            _zoneProperties.Height = Random.Range(.1f, _zoneProperties.Height); 
            _zoneProperties.UpDownSpeed = Random.Range(.1f, _zoneProperties.UpDownSpeed);
            _zoneProperties.RotationSpeed = Random.Range(-_zoneProperties.RotationSpeed, _zoneProperties.RotationSpeed);
            _zoneProperties.MaxMusicNoteDistance = Random.Range(.1f, _zoneProperties.MaxMusicNoteDistance);
            _zoneProperties.DisapearSpeed = Mathf.Clamp(_zoneProperties.DisapearSpeed, 1, float.MaxValue);
            _zoneProperties.DisapearSpeed = Random.Range(1f, _zoneProperties.DisapearSpeed);
            _defaultPos = _musicNoteTransform.position;
            _takeAwayDirection = new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f)).normalized;
            _collisionDetection.EventOnEnemyEntered.AddListener(OnEnemyEntered); //Add listener for damages
        }
        
    }

    private void OnEnemyEntered() //When an enemy enter the trigger
    {
        foreach(GameObject detectedEnemy in _collisionDetection.GetDetectedEnnemies())
        {
            if (RB_Tools.TryGetComponentInParent(detectedEnemy, out RB_Health enemyHeath))
            {
                enemyHeath.TakeDamage(_zoneProperties.Damages); //Deal damages to enemy
            }
        }
        
    }

    private void Update()
    {
        //Constantly animate
        Animate();
        TakeAway();
        Disapear();
    }

    private void Animate()
    {
        if (_shouldAnimate)
        {
            AnimateUpDown();
        }
    }

    private void StartAnimate() //Start the animations
    {
        _defaultPos = _musicNoteTransform.position;
        _shouldAnimate = true;
    }

    private void TakeAway() //take away the notes from the player
    {
        if (_shouldTakeAway)
        {
            _musicNoteSpriteParent.localPosition += _takeAwayDirection * Time.deltaTime * _zoneProperties.TakeAwaySpeed;
            if(Mathf.Abs(_musicNoteSpriteParent.localPosition.magnitude) >= _zoneProperties.MaxMusicNoteDistance) //If the distance is enough, stop the take away
                _shouldTakeAway = false;
        }
    }

    public void StopTakeAway() //Stop the take away
    {
        _shouldTakeAway = false;
    }

    private void AnimateUpDown() //Animate the up down animation
    {
        _musicNoteSpriteTransform.localPosition = _defaultPos + Vector3.up * Mathf.Sin(Time.time * _zoneProperties.UpDownSpeed) * _zoneProperties.Height;
        _musicNoteTransform.Rotate(Vector3.up * Time.deltaTime * _zoneProperties.RotationSpeed);
        _musicNoteTransform.position = _playerTransform.position;
    }

    public void StartDisapear() //Start to disapear the note
    {
        _shouldDisapear = true;
    }

    private void Disapear() //Disapear the notes
    {
        if (_shouldDisapear)
        {
            _spriteRenderer.color -= new Color(0, 0, 0, Time.deltaTime * _zoneProperties.DisapearSpeed);
            if (_spriteRenderer.color.a <= 0)
            {
                EventOnDestroy?.Invoke();
                Destroy(gameObject);
            }
        }
    }
        
}

[System.Serializable]
public class ZonePropertiesClass //Properties of the zones
{
    public float Height;
    public float UpDownSpeed;
    public float RotationSpeed;
    public float TakeAwaySpeed;
    public float MaxMusicNoteDistance;
    public int NoteAmount;
    public float DisapearSpeed;
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
            Damages = this.Damages,
            DisapearSpeed = this.DisapearSpeed
        };
    }
}
