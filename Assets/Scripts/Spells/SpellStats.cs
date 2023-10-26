using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellStats : ScriptableObject
{
    public string spellName;
    public Object spellFX;

    public virtual void CastSpell(BaseCharacterController caster)
    {
        if (spellFX != null)
            Instantiate(spellFX, caster.transform.position, caster.transform.rotation);
    }
}
