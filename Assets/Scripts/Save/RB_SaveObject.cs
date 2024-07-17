[System.Serializable]
public class RB_SaveObject
{
    public int CurrentLevel = 1;
    public bool TutoDone = false;
    public string PlayerName = "";
    public bool IsGameFinish = false;

    public float HpBossRush = 150;

    public DIFFICULTY InfiltrationDifficulty = DIFFICULTY.Normal;
    public DIFFICULTY CombatDifficulty = DIFFICULTY.Normal;
}