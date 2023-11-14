using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellStats : ScriptableObject
{
    public string spellName;
    public Object spellFX;

    public bool attachToWeapon = true;

    public virtual void CastSpell(BaseCharacterController caster, GameObject target)
    {
        if (spellFX != null)
        {
            GameObject fx;

            if (attachToWeapon)
            {
                fx = Instantiate(spellFX, caster.GetCharacterCombat().weapon.transform) as GameObject;
            }
            else
            {
                fx = Instantiate(spellFX, caster.transform.position, caster.transform.rotation) as GameObject;
            }
        }
    }
}
