using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrap", menuName = "Interactables/Trap", order = 0)]
public class TrapStats : ScriptableObject
{
    public int damage;
    public bool canBlock = false;
    public bool canParry = false;

    public E_TargetType targetType;
    public Object projectile;
    public Object explosionFX;
    public float range;
    public float shotAOE = 0;

    public E_Duration durationType;
    public float activateInterval;
}

public enum E_TargetType
{
    Shot, Area
}

public enum E_Duration
{
    OnlyOnce, Interval, OnceDestroy
}