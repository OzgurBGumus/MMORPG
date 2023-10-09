using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IPhysicalDefenseReductionBonus
{
    int GetPhysicalDefenseReductionBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class PhysicalDefenseReduction : Stat
{
    public Level level;

    public LinearInt basePhysicalDefenseReduction = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IPhysicalDefenseReductionBonus[] _bonusComponents;
    IPhysicalDefenseReductionBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IPhysicalDefenseReductionBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = basePhysicalDefenseReduction.Get(level.current);
            foreach (IPhysicalDefenseBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetPhysicalDefenseBonus();
            return baseThisLevel + bonus;
        }
    }
}