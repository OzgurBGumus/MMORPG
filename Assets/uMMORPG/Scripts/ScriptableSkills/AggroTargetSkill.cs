using UnityEngine;

[CreateAssetMenu(menuName = "uMMORPG Skill/Agro Target", order = 999)]
public class AggroTargetSkill : TargetProjectileSkill
{
    public override bool CheckTarget(Entity caster)
    {
        // target exists, alive, not self, oktype?
        return caster.target != null && caster.target is Monster && caster.CanAttack(caster.target);
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        // target still around?
        if (caster.target != null)
        {
            destination = Utils.ClosestPoint(caster.target, caster.transform.position);
            return Utils.ClosestDistance(caster, caster.target) <= castRange.Get(skillLevel);
        }
        destination = caster.transform.position;
        return false;
    }

    public override void Apply(Entity caster, int skillLevel)
    {
        Monster monster = (Monster)caster.target;
        monster.target = caster;
        monster.UpdateAggroList(caster, monster.aggroValueMax);

        // deal damage directly with base damage + skill damage
        caster.combat.DealDamageAt(monster, caster.name,
                            GetDamage(caster.combat.physicalAttack, caster.combat.magicalAttack, skillLevel),
                            damageType,
                            //gff
                            //caster.combat.damage + damage.Get(skillLevel) + caster.ammoDamageBonuses(),

                            stunChance.Get(skillLevel),
                            stunTime.Get(skillLevel));
    }
}
