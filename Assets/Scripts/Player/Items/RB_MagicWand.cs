using UnityEngine;

public class RB_MagicWand : RB_Items
{
    protected override void Start()
    {
        base.Start();
        _playerAnimator.SetFloat("WeaponID", 1);
    }
}
