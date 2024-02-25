using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon", order = 0)]
public class WeaponMoveset : ScriptableObject
{
    public string weaponName;
    [TextArea(3, 10)]
    public string weaponDescription;
    public int baseAttackPwr = 3;

    public int goldCost;

    public Object[] weapons, offhandWeapons;

    public AttackTypes[] attacks;
}
