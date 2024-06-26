using UnityEngine;

public class RB_FriendlyOutline : MonoBehaviour
{
    private SpriteRenderer _selfSpriteRenderer;
    private RB_Enemy _entity;

    [SerializeField] private Material _outlineMaterial;
    private Material _baseMaterial;

    private void Awake()
    {
        _selfSpriteRenderer = GetComponent<SpriteRenderer>();
        if (RB_Tools.TryGetComponentInParent<RB_Enemy>(transform, out RB_Enemy enemy))
        {
            _entity = enemy;
            _entity.EventAllyTeam?.AddListener(ApplyFriendlyEffect);
            _entity.EventEnemyTeam?.AddListener(DisableFriendlyEffect);
            _entity.GetComponent<RB_Health>().EventDeath?.AddListener(DisableFriendlyEffect);
        }
    }

    public void ApplyFriendlyEffect()
    {
        _selfSpriteRenderer.enabled = true;
        _selfSpriteRenderer.material = _outlineMaterial;
    }

    public void DisableFriendlyEffect()
    {
        _selfSpriteRenderer.enabled = false;
        _selfSpriteRenderer.material = _baseMaterial;
    }
}
