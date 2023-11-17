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
        Transform spawnTransform = attachToWeapon ? caster.GetCharacterCombat().weapon.transform : caster.transform;

        GameObject projectileObj = Instantiate(projectileData.projectile, spawnTransform.position, new Quaternion(0, 0, 0, 0)) as GameObject;
        ProjectileMovement projectileMove = projectileObj.GetComponent<ProjectileMovement>();
        projectileMove.Fire(targetPos, projectileData, caster.gameObject, projectileDamage);
    }
}