using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IPhysicalDefenseBonus
{
    int GetPhysicalDefenseBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class PhysicalDefense : Stat
{
    public Level level;

    public LinearInt basePhysicalDefense = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IPhysicalDefenseBonus[] _bonusComponents;
    IPhysicalDefenseBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IPhysicalDefenseBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = basePhysicalDefense.Get(level.current);
            foreach (IPhysicalDefenseBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetPhysicalDefenseBonus();
            return baseThisLevel + bonus;
        }
    }
}