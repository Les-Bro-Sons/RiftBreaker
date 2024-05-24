public class RB_Katana : RB_Items
{
    public override void Bind()
    {
        base.Bind();
        _playerAnimator.SetFloat("WeaponID", 0);
        _colliderAnimator.SetFloat("WeaponID", 0);
    }
}
