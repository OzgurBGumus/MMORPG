using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IPhysicalAttackBonus
{
    int GetPhysicalAttackBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class PhysicalAttack : Stat
{
    public Level level;

    public LinearInt basePhysicalAttack = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IPhysicalAttackBonus[] _bonusComponents;
    IPhysicalAttackBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IPhysicalAttackBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = basePhysicalAttack.Get(level.current);
            foreach (IPhysicalAttackBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetPhysicalAttackBonus();
            return baseThisLevel + bonus;
        }
    }
}