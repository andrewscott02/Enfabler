using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSummonSpell", menuName = "Spells/Summon", order = 1)]
public class SummonSpell : SpellStats
{
    [System.Serializable]
    public struct SummonData
    {
        public Object summon;
        public int count;
    }

    public SummonData[] summonData;
    public bool spawnAtCaster = true;

    public override void CastSpell(BaseCharacterController caster, GameObject target)
    {
        base.CastSpell(caster, target);

        SpawnObjects(caster, target);
    }

    void SpawnObjects(BaseCharacterController caster, GameObject target)
    {
        for (int i = 0; i < summonData.Length; i++)
        {
            for (int x = 0; x < summonData[i].count; x++)
            {
                Vector3 spawnPos;

                Vector3 origin = caster.transform.position;
                if (!spawnAtCaster)
                    origin = target == null ? caster.transform.position : target.transform.position;

                if (!HelperFunctions.GetRandomPointOnNavmesh(origin, 10f, 0.5f, 100, out spawnPos))
                {
                    spawnPos = caster.transform.position;
                }

                Instantiate(summonData[i].summon, spawnPos, Quaternion.identity);
            }
        }
    }
}
