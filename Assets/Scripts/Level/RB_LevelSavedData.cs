public class RB_LevelSavedData
{
    public RB_LevelSavedData(SCENENAMES currentScene)
    {
        CurrentScene = currentScene;
    }

    public SCENENAMES CurrentScene;
    public bool HasReachedWeapon = false;
    public bool ShouldRobertFirstTalk = true;
}
