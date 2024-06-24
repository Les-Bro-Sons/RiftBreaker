using UnityEngine;

public class RB_MusicNoteZone : MonoBehaviour
{
    //Properties
    [Header("Properties")]
    [SerializeField] private float _height;
    [SerializeField] private float _upDownSpeed;
    [SerializeField] private float _leftRightDistance;
    [SerializeField] private float _leftRightSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _takeAwaySpeed;
    [SerializeField] private float _maxMusicNoteDistance;
    private Vector3 _takeAwayDirection;
    private Vector3 _defaultPos;

    //Conditions
    [SerializeField] private bool _shouldAnimate = false;
    [SerializeField] private bool _shouldTakeAway = false;

    //Components
    [Header("Components")]
    [SerializeField] private Transform _musicNoteSpriteParent;
    private Transform _musicNote;
    private Transform _musicNoteSprite;
    private Transform _playerTransform;

    private void Awake()
    {
        _musicNote = transform;
        _musicNoteSprite = _musicNote.GetComponentInChildren<SpriteRenderer>().transform;
    }

    private void Start()
    {
        _playerTransform = RB_PlayerMovement.Instance.transform;
        _height = Random.Range(.1f, _height);
        _upDownSpeed = Random.Range(.1f, _upDownSpeed);
        _leftRightDistance = Random.Range(.1f, _leftRightDistance);
        _leftRightSpeed = Random.Range(.1f, _leftRightSpeed);
        _rotationSpeed = Random.Range(-_rotationSpeed, _rotationSpeed);
        _defaultPos = _musicNote.position;
        _takeAwayDirection = new Vector3(Random.Range(.1f, 1f), 0, Random.Range(.1f, 1f)).normalized;
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
            _musicNoteSpriteParent.localPosition += _takeAwayDirection * Time.deltaTime * _takeAwaySpeed;
            if(Mathf.Abs(_musicNoteSpriteParent.localPosition.x) >= _maxMusicNoteDistance)
                _shouldTakeAway = false;
        }
    }

    private void AnimateUpDownLeftRight()
    {
        _musicNoteSprite.localPosition = _defaultPos + Vector3.up * Mathf.Sin(Time.time * _upDownSpeed) * _height + _defaultPos + Vector3.right * Mathf.Sin(Time.time * _leftRightSpeed) * _leftRightDistance;
        _musicNote.Rotate(Vector3.up * Time.deltaTime * _rotationSpeed);
        _musicNote.position = _playerTransform.position;
    }
}
