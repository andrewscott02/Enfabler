using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CharacterCombat
{
    #region Buddy AI

    ConstructPlayerModel modelConstructor;

    private void Start()
    {
        modelConstructor = GameObject.FindObjectOfType<ConstructPlayerModel>();
    }

    public override void HitEnemy(bool hit)
    {
        modelConstructor.PlayerAttack(hit);
    }

    public override void Parry()
    {
        if (canAttack)
        {
            modelConstructor.PlayerParry(true);
        }
        base.Parry();
    }

    public override void Dodge()
    {
        if (canAttack)
        {
            modelConstructor.PlayerDodge(true, true);
        }
        base.Dodge();
    }

    #endregion
}