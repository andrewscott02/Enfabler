using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon", order = 0)]
public class WeaponMoveset : ScriptableObject
{
    public Object[] weapons, offhandWeapons;

    public AttackTypes[] attacks;
}
