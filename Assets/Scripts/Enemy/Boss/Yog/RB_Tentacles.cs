using System.Collections.Generic;
using UnityEngine;

public class RB_Tentacles : MonoBehaviour
{
    private enum DIRECTION
    {
        Left,
        Right, 
        Up, 
        Down
    }

    [SerializeField] private DIRECTION _direction;

    [SerializeField] private SpriteRenderer _baseSprite;
    [SerializeField] private SpriteRenderer _middleSprite;
    [SerializeField] private SpriteRenderer _endSprite;

    private Transform _baseTransform;
    private Transform _middleTransform;
    private Transform _endTransform;

    [SerializeField] private Transform _middlePivotTransform;
    [SerializeField] private Transform _endPivotTransform;


    [SerializeField] private List<Sprite> _upSprites; //0 = base, 1 = middle, 2 = bout
    [SerializeField] private List<Sprite> _downSprites;
    [SerializeField] private List<Sprite> _leftSprites;
    [SerializeField] private List<Sprite> _rightSprites;

    [SerializeField] public float Size = 1; //float for the offset when adding a full sprite to attach to the base of the tentacle

    [Header("Variables caca")]
    [SerializeField] private float _hTileSize; //float for the tile to add a full sprite
    [SerializeField] private float _hTileOffset; //float for the offset when adding a full sprite to attach to the base of the tentacle
    [SerializeField] private float _vTileSize;
    [SerializeField] private float _vTileOffset;
    

    private void Awake()
    {
        _baseTransform = _baseSprite.transform;
        _middleTransform = _middleSprite.transform;
        _endTransform = _endSprite.transform;


        UpdatePivotPos();
        UpdateSprite();
    }

    private void Update()
    {
        UpdateSprite();
        UpdatePivotPos();

        float realSize = Size - 1;
        Vector3 offsetPos = Vector3.zero;
        switch (_direction)
        {
            case DIRECTION.Left:
                offsetPos.x = -_hTileOffset * realSize;
                _middleSprite.size = new Vector2(2 + (_hTileSize * realSize), 2);
                break;
            case DIRECTION.Right:
                offsetPos.x = _hTileOffset * realSize;
                _middleSprite.size = new Vector2(2 + (_hTileSize * realSize), 2);
                break;
            case DIRECTION.Up:
                offsetPos.y = _vTileOffset * realSize;
                _middleSprite.size = new Vector2(2 , 2 + (_hTileSize * realSize));
                break;
            case DIRECTION.Down:
                offsetPos.y = -_vTileOffset * realSize;
                _middleSprite.size = new Vector2(2 , 2 + (_hTileSize * realSize));
                break;
        }
        _middleTransform.localPosition = offsetPos;
        _endTransform.localPosition = offsetPos;
    }

    private void UpdateSprite()
    {
        switch (_direction)
        {
            case DIRECTION.Up:
                _baseSprite.sprite = _upSprites[0];
                _middleSprite.sprite = _upSprites[1];
                _endSprite.sprite = _upSprites[2];
                break;
            case DIRECTION.Down:
                _baseSprite.sprite = _downSprites[0];
                _middleSprite.sprite = _downSprites[1];
                _endSprite.sprite = _downSprites[2];
                break;
            case DIRECTION.Left:
                _baseSprite.sprite = _leftSprites[0];
                _middleSprite.sprite = _leftSprites[1];
                _endSprite.sprite = _leftSprites[2];
                break;
            case DIRECTION.Right:
                _baseSprite.sprite = _rightSprites[0];
                _middleSprite.sprite = _rightSprites[1];
                _endSprite.sprite = _rightSprites[2];
                break;
        }
    }

    private void UpdatePivotPos()
    {
        if (_direction == DIRECTION.Up || _direction == DIRECTION.Down)
        {
            _middlePivotTransform.localPosition = new Vector3(0, 0.2044f, 0);
            _endPivotTransform.localPosition = new Vector3(0, 0.2187f, 0);
        }
        else
        {
            _middlePivotTransform.localPosition = new Vector3(0, 0.2187f, 0);
            _endPivotTransform.localPosition = new Vector3(0, 0.2187f, 0);
        }
    }
}
