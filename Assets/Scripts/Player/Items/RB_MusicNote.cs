using UnityEngine;

public class RB_MusicNote : MonoBehaviour
{
    //Music Note Sprite
    private Sprite _currentSprite;
    private RB_MusicBox _currentItem;

    //Components
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }


    private void Start()
    {
        _currentItem = RB_PlayerAction.Instance.Item as RB_MusicBox;
        _currentSprite = _currentItem.NoteSprites[Random.Range(0, _currentItem.NoteSprites.Count)];
        _spriteRenderer.sprite = _currentSprite;
    }
}
