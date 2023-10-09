// Agility Attribute that grants extra dodge, hit, criticalStrike Chance.
// IMPORTANT: SyncMode=Observers needed to show other player's health correctly!
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Agility : PlayerAttribute, IDodgeBonus, IHitBonus, ICritBonus
{
    public int baseValue;
    // 1 point means 1% of max bonus
    public float hitBonusPerPoint = 1;
    public float dodgeBonusPerPoint = 1;
    public float critBonusPerPoint = 1;

    public int GetCritBonus()
    {
        return Convert.ToInt32(value * critBonusPerPoint);
    }

    public int GetDodgeBonus()
    {
        return Convert.ToInt32(value * dodgeBonusPerPoint);
    }

    public int GetHitBonus()
    {
        return Convert.ToInt32(value * hitBonusPerPoint);
    }
}
