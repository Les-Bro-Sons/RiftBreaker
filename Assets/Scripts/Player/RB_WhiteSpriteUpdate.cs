using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_WhiteSpriteUpdate : MonoBehaviour
{
    [SerializeField] SpriteRenderer PlayerSpriteRenderer;
    SpriteRenderer SelfSpriteRenderer;

    private void Start()
    {
        SelfSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SelfSpriteRenderer.sprite = PlayerSpriteRenderer.sprite;
    }
}
