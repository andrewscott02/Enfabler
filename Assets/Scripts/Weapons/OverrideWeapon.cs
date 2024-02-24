using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enfabler.Attacking;

public class OverrideWeapon : MonoBehaviour
{
    public Attacks attacks;

    public void EquipWeapon()
    {
        attacks.attacks = WeaponManager.equippedWeapon.attacks;
    }
}
