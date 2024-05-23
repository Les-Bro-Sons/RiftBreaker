public class RB_Katana : RB_Items
{
    protected override void Start()
    {
        base.Start();
        _playerAnimator.SetFloat("WeaponID", 0);
    }
}
