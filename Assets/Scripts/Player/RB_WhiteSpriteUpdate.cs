using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_WhiteSpriteUpdate : MonoBehaviour
{
    SpriteRenderer ParentSpriteRenderer;
    SpriteRenderer SelfSpriteRenderer;

    private void Start()
    {
        SpriteRenderer[] sprites;
        sprites = GetComponentsInParent<SpriteRenderer>();
        ParentSpriteRenderer = sprites[1];
        SelfSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SelfSpriteRenderer.sprite = ParentSpriteRenderer.sprite;
    }
}
