using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSummonSpell", menuName = "Spells/SummonSpell", order = 0)]
public class SummonSpell : SpellStats
{
    [System.Serializable]
    public struct SummonData
    {
        public Object summon;
        public int count;
    }

    public SummonData[] summonData;

    public override void CastSpell(BaseCharacterController caster)
    {
        base.CastSpell(caster);

        for (int i = 0; i < summonData.Length; i++)
        {
            for(int x = 0; x < summonData[i].count; x++)
            {
                Vector3 spawnPos;
                if (!HelperFunctions.GetRandomPointOnNavmesh(caster.transform.position, 10f, 0.5f, 100, out spawnPos))
                {
                    spawnPos = caster.transform.position;
                }

                Instantiate(summonData[i].summon, spawnPos, Quaternion.identity);
            }
        }
    }
}
