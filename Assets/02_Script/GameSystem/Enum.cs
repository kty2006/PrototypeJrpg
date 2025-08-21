using UnityEngine;

public enum Job
{
    Guner,
    Warrior,
    Mage,
    Axe,
    Spear,
    Golem
}

public enum AttackType
{
    Melee, //근거리
    Ranged, //원거리
}

public enum TurnStates
{
    Start,
    Play,
    Playing,
    End
}

public enum UnitType
{
    Friendly,
    Enemy,
    Object
}

public enum UnitStates
{
    Idle,
    Move,
    Push,
    Attack,
    Skill1,
    Skill2,
    
}

public enum Sortings
{
    Remove,
    AllRemove
}