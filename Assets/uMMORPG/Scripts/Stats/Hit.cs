using UnityEngine;

// inventory, attributes etc. can influence hit stat.
public interface IHitBonus
{
    int GetHitBonus();
}

[RequireComponent(typeof(Level))]
[DisallowMultipleComponent]
public class Hit : Stat
{
    public Level level;

    public LinearInt baseHit = new LinearInt{baseValue=1};

    // cache components that give a bonus (attributes, inventory, etc.)
    // (assigned when needed. NOT in Awake because then prefab.max doesn't work)
    IHitBonus[] _bonusComponents;
    IHitBonus[] bonusComponents =>
        _bonusComponents ?? (_bonusComponents = GetComponents<IHitBonus>());

    // calculate current
    public override int current
    {
        get
        {
            // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
            int bonus = 0;
            int baseThisLevel = baseHit.Get(level.current);
            foreach (IHitBonus bonusComponent in bonusComponents)
                bonus += bonusComponent.GetHitBonus();
            return baseThisLevel + bonus;
        }
    }
}