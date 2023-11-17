using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectileSpell", menuName = "Spells/Projectile", order = 0)]
public class ProjectileSpell : SpellStats
{
    public TrapStats projectileData;

    public override void CastSpell(BaseCharacterController caster, GameObject target)
    {
        base.CastSpell(caster, target);

        SpawnProjectile(caster, target.gameObject.transform.position, projectileData.damage);
    }

    void SpawnProjectile(BaseCharacterController caster, Vector3 targetPos, int projectileDamage)
    {
        GameObject projectileObj = Instantiate(projectileData.projectile, caster.transform.position, caster.transform.rotation) as GameObject;
        ProjectileMovement projectileMove = projectileObj.GetComponent<ProjectileMovement>();
        projectileMove.Fire(targetPos, projectileData, caster.gameObject, projectileDamage);
    }
}
