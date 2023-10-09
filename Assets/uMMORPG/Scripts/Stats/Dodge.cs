using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IDodgeBonus
{
    int GetDodgeBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Dodge : Stat
{
    public Level level;

    public LinearInt baseDodge = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IDodgeBonus[] _bonusComponents;
    IDodgeBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IDodgeBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = baseDodge.Get(level.current);
            foreach (IDodgeBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetDodgeBonus();
            return baseThisLevel + bonus;
        }
    }
}