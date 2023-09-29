using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrap", menuName = "Interactables/Trap", order = 0)]
public class TrapStats : ScriptableObject
{
    public int damage;

    public E_TargetType targetType;

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