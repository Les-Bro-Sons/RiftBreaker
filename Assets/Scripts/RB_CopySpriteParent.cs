using UnityEngine;

public class RB_CopySpriteParent : MonoBehaviour
{
    private SpriteRenderer _parentSpriteRenderer;
    private SpriteRenderer _selfSpriteRenderer;

    private void Start()
    {
        SpriteRenderer[] sprites;
        sprites = GetComponentsInParent<SpriteRenderer>();
        _parentSpriteRenderer = sprites[1];
        _selfSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _selfSpriteRenderer.sprite = _parentSpriteRenderer.sprite;
    }
}
