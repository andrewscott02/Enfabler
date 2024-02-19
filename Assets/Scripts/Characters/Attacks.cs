using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enfabler.Attacking
{
    public class Attacks : MonoBehaviour
    {
        public AttackTypes[] attacks;

        public AttackTypes GetAttackData(E_AttackType attackType)
        {
            foreach (var item in attacks)
            {
                if (item.attackType == attackType)
                    return item;
            }

            AttackTypes nullData = new AttackTypes();
            nullData.attackType = E_AttackType.None;

            return nullData;
        }

        public int GetVariation(E_AttackType attackType, int wantedVariation)
        {
            foreach (var item in attacks)
            {
                if (item.attackType == attackType)
                {
                    if (wantedVariation < item.variations.Length)
                    {
                        return wantedVariation;
                    }
                }
            }

            return 0;
        }
    }

    [System.Serializable]
    public struct AttackTypes
    {
        public E_AttackType attackType;
        public int weaponIndex;
        public bool rangedInput;
        public AttackData[] variations;
    }

    [System.Serializable]
    public struct AttackData
    {
        public string animation;
        public int damage;
        public float chargeDamageScaling;

        public int currentAttackAnimModifier;
        public int nextAttackIndex;

        public TrapStats projectileData;
        public Object projectileFX;
        public float[] additionalShotAngle;
        public float additionalProjectileDamageMultiplier;
    }

    public enum E_AttackType
    {
        None, PrimaryAttack, SecondaryAttack, SwitchPrimaryAttack, SwitchSecondaryAttack, LungeAttack, SprintPrimaryAttack, SprintSecondaryAttack, DodgePrimaryAttack, DodgeSecondaryAttack
    }
}