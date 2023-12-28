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
    public bool attachToTarget = false;

    public override void CastSpell(BaseCharacterController caster, GameObject target)
    {
        base.CastSpell(caster, target);

        SpawnObjects(caster, target);
    }

    void SpawnObjects(BaseCharacterController caster, GameObject target)
    {
        foreach (var item in summonData)
        {
            for (int i = 0; i < item.count; i++)
            {
                Vector3 spawnPos;

                Vector3 origin = caster.transform.position;
                if (!spawnAtCaster)
                    origin = target == null ? caster.transform.position : target.transform.position;

                if (!HelperFunctions.GetRandomPointOnNavmesh(origin, 10f, 0.5f, 100, out spawnPos))
                {
                    spawnPos = caster.transform.position;
                }

                GameObject go = Instantiate(item.summon, spawnPos, Quaternion.identity) as GameObject;

                if (attachToTarget)
                {
                    go.transform.parent = spawnAtCaster ? caster.transform : target.transform;
                    go.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}
