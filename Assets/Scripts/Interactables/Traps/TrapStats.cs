using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrap", menuName = "Interactables/Trap", order = 0)]
public class TrapStats : ScriptableObject
{
    public int damage;
    public float timeDuration;
}

public enum E_TrapType
{
    Shot, Area
}

public enum E_Activation
{
    Sight, Contact, Constant
}

public enum E_Duration
{
    OnlyOnce, Interval, OnceDestroy
}