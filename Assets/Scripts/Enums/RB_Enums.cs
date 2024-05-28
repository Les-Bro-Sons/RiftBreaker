public enum PHASES
{
    Infiltration,
    Combat,
    Boss
}

public enum SCENENAMES
{
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8,
    Level9,

    Boss1,
    Boss2,
    Boss3,
}

// ~~~~~~~~~~ TYPES ~~~~~~~~~~

public enum FADETYPE
{
    Fade,
    Rift,
    BubbleOpen,
    WooshRight,
    WooshUp,
}

public enum ZOOMTYPES
{
    Linear,
    FadeIn,
    FadeOut,
}

public enum SPEEDTYPES
{
    Linear,
    Exponential,
    Logarithm,
}

public enum HUDTYPE
{
    Level,
    Boss,
    Menu,
}

public enum ENTITYTYPES
{
    Ai,
    Player,
}

public enum TEAMS
{
    Ai,
    Player,
    Neutral,
}

public enum ENEMYCLASS
{
    Light,
    Medium,
    Heavy
}

// ~~~~~~~~~~ STATES ~~~~~~~~~~

public enum PLAYERSTATES
{
    Idle,
    Moving,
    Attacking,
    SpecialAttacking,
    ChargingAttack,
    Stunned,
}

public enum SETTINGSTATES
{
    Audio,
    Graphics,
    Controls
}