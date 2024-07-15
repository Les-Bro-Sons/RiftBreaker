public enum PHASES
{
    Infiltration,
    Combat,
    Boss
}

public enum SCENENAMES 
{ 
    Tuto,
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

    FirstCinematic,
    EndCinematic,

    BossRush1,
    BossRush2,
    BossRush3,
}

// ~~~~~~~~~~ TYPES ~~~~~~~~~~

public enum FADETYPE
{
    Fade,
    Rift,
    BubbleOpen,
    WooshRight,
    WooshUp,
    Circle,
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
    Heavy,
    Pawn,
    Tower,
    Megaknight,
    RobertLeNec,
    Yog,
}

public enum TARGETMODE
{
    Closest,
    Furthest,
    Random
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

public enum MIXERNAME
{
    Master,
    SFX,
    Music
}

// BT

public enum BTBOOLVALUES
{
    IsTargetSpotted,
    IsTargetInSight,
    HasACorrectView,
    GoToLastTargetPos,
    IsDistracted,

    PlayerSpottedInCombat,
    IsAttacking,
    IsWaitingForAttack,
    AlreadyAttacked,

    HeavyAttackSlash,
}

public enum DISTRACTIONTYPE
{
    BrokenPot,
    DeadBody,
    BrokenDoor,
    Dash,
    SomeoneSuspicious,
}

public enum REWINDENTITYTYPE
{
    All,
    AiOnly,
    PlayerOnly,
}