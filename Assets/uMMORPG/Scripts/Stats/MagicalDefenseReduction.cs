using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IMagicalDefenseReductionBonus
{
    int GetMagicalDefenseReductionBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class MagicalDefenseReduction : Stat
{
    public Level level;

    public LinearInt baseMagicalDefenseReduction = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IMagicalDefenseReductionBonus[] _bonusComponents;
    IMagicalDefenseReductionBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IMagicalDefenseReductionBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = baseMagicalDefenseReduction.Get(level.current);
            foreach (IMagicalDefenseBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetMagicalDefenseBonus();
            return baseThisLevel + bonus;
        }
    }
}