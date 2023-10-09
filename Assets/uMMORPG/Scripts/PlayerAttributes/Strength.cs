// Strength Attribute that grants extra health.
// IMPORTANT: SyncMode=Observers needed to show other player's health correctly!
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Strength : PlayerAttribute, IHealthBonus, IPhysicalAttackBonus
{
    public int baseValue;
    public float healthBonusPerPoint = 0.01f;
    public float physicalAttackBonusPerPoint = 0.01f;

    public int GetHealthBonus() =>
        Convert.ToInt32(value * healthBonusPerPoint);

    public int GetHealthRecoveryBonus() => 0;

    public int GetPhysicalAttackBonus()
    {
        return Convert.ToInt32(value * physicalAttackBonusPerPoint);
    }
}
