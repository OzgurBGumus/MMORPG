using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface ICritBonus
{
    int GetCritBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Crit : Stat
{
    public Level level;

    public LinearInt baseCrit = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    ICritBonus[] _bonusComponents;
    ICritBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<ICritBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = baseCrit.Get(level.current);
            foreach (ICritBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetCritBonus();
            return baseThisLevel + bonus;
        }
    }
}