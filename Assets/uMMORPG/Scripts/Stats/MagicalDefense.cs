using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IMagicalDefenseBonus
{
    int GetMagicalDefenseBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class MagicalDefense : Stat
{
    public Level level;

    public LinearInt baseMagicalDefense = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IMagicalDefenseBonus[] _bonusComponents;
    IMagicalDefenseBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IMagicalDefenseBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = baseMagicalDefense.Get(level.current);
            foreach (IMagicalDefenseBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetMagicalDefenseBonus();
            return baseThisLevel + bonus;
        }
    }
}