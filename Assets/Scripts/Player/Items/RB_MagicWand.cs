using UnityEngine;

public class RB_MagicWand : RB_Items
{
    public override void Bind()
    {
        base.Bind();
        _playerAnimator.SetFloat("WeaponID", 1);
        _colliderAnimator.SetFloat("WeaponID", 1);
    }
}
