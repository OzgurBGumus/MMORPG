// Strength Attribute that grants extra Hit and Physicaldefense.
// IMPORTANT: SyncMode=Observers needed to show other player's health correctly!
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Endurance : PlayerAttribute, IHealthBonus, IPhysicalDefenseBonus
{
    public int baseValue;
    public float healthBonusPerPoint = 16;
    public float physicalDefenseBonusPerPoint = 1;

    public int GetHealthBonus()
    {
        return Convert.ToInt32(value * healthBonusPerPoint);
    }

    public int GetHealthRecoveryBonus()
    {
        return 0;
    }

    public int GetPhysicalDefenseBonus()
    {
        return Convert.ToInt32(value * physicalDefenseBonusPerPoint);
    }
}
