using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class OverrideWeapon : MonoBehaviour
{
    public Attacks attacks;
    public CharacterCombat combat;

    public void EquipWeapon()
    {
        attacks.attacks = WeaponManager.equippedWeapon.attacks;

        combat.ForceSetupWeapon(0);
    }
}
