using Mirror;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "uMMORPG Skill/Aggro Area", order = 999)]
public class AggroAreaSkill : TargetProjectileSkill
{
    public override bool CheckTarget(Entity caster)
    {
        return true;
    }

    public override bool CheckDistance(Entity caster, int skillLevel, out Vector3 destination)
    {
        destination = caster.transform.position;
        return true;
    }

    public override void Apply(Entity caster, int skillLevel)
    {
        // candidates hashset to be 100% sure that we don't apply an area skill
        // to a candidate twice. this could happen if the candidate has more
        // than one collider (which it often has).
        HashSet<Monster> candidates = new HashSet<Monster>();

        // find all entities of same type in castRange around the caster
        Collider[] colliders = Physics.OverlapSphere(caster.transform.position, castRange.Get(skillLevel));
        foreach (Collider co in colliders)
        {
            Monster candidate = co.GetComponentInParent<Monster>();

            if (candidate != null && candidate.health.current > 0)
            {
                candidates.Add(candidate);
            }
        }

        // apply to all candidates
        foreach (Monster candidate in candidates)
        {
            candidate.UpdateAggroList(caster, candidate.aggroValueMax);

            // show effect on candidate
            //SpawnEffect(caster, candidate);

            // deal damage directly with base damage + skill damage
            caster.combat.DealDamageAt(candidate,
                                caster.combat.damage + damage.Get(skillLevel),

                                //gff
                                //caster.combat.damage + damage.Get(skillLevel) + caster.ammoDamageBonuses(),

                                stunChance.Get(skillLevel),
                                stunTime.Get(skillLevel));
        }
    }
}
