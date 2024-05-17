using UnityEngine;

public class RB_SpriteFadeOut : MonoBehaviour
{
    //Dash fade out properties
    [HideInInspector] public float FadeForce;
    private bool _isFading = false;
    

    //Components
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FadeOut()
    {
        //Fading out the alpha
        if(_spriteRenderer.color.a > 0)
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, _spriteRenderer.color.a - FadeForce);
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        //Constantly fading out until alpha is 0
        FadeOut();
    }

}
