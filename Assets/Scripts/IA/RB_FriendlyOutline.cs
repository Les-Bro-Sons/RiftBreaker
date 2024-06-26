using UnityEngine;

public class RB_FriendlyOutline : MonoBehaviour
{
    private SpriteRenderer _selfSpriteRenderer;
    private RB_Enemy _entity;

    [SerializeField] private Material _outlineMaterial;
    private Material _baseMaterial;

    private void Start()
    {
        SpriteRenderer[] sprites;
        _selfSpriteRenderer = GetComponent<SpriteRenderer>();
        if (RB_Tools.TryGetComponentInParent<RB_Enemy>(transform, out RB_Enemy enemy))
        {
            _entity = enemy;
            _entity.EventAllyTeam?.AddListener(ApplyFriendlyEffect);
            _entity.EventEnemyTeam?.AddListener(DisableFriendEffect);
        }
    }

    public void ApplyFriendlyEffect()
    {
        _selfSpriteRenderer.enabled = true;
        _selfSpriteRenderer.material = _outlineMaterial;
    }

    public void DisableFriendEffect()
    {
        _selfSpriteRenderer.enabled = false;
        _selfSpriteRenderer.material = _baseMaterial;
    }
}
