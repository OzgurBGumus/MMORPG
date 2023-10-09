using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IMagicalAttackBonus
{
    int GetMagicalAttackBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class MagicalAttack : Stat
{
    public Level level;

    public LinearInt baseMagicalAttack = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IMagicalAttackBonus[] _bonusComponents;
    IMagicalAttackBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IMagicalAttackBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = baseMagicalAttack.Get(level.current);
            foreach (IMagicalAttackBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetMagicalAttackBonus();
            return baseThisLevel + bonus;
        }
    }
}