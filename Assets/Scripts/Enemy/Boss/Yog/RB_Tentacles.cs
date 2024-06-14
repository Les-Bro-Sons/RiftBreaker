using UnityEngine;

public class RB_Tentacles : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _baseSprite;
    [SerializeField] private SpriteRenderer _middleSprite;
    [SerializeField] private SpriteRenderer _endSprite;

    private Transform _baseTransform;
    private Transform _middleTransform;
    private Transform _endTransform;

    private void Awake()
    {
        _baseTransform = _baseSprite.transform;
        _middleTransform = _middleSprite.transform;
        _endTransform = _endSprite.transform;
    }
}
