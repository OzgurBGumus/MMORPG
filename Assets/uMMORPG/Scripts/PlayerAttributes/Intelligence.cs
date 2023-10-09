// Strength Attribute that grants extra health.
// IMPORTANT: SyncMode=Observers needed to show other player's mana correctly!
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Intelligence : PlayerAttribute, IManaBonus, IMagicalAttackBonus, IMagicalDefenseBonus
{
    public int baseValue;
    public float manaBonusPerPoint = 4;
    public float manaRecoveryBonusPerPoint = 1;
    public float magicalAttackBonusPerPoint = 2;
    public float magicalDefenseBonusPerPoint = 1;

    public int GetManaBonus()
    {
        return Convert.ToInt32(value * manaBonusPerPoint);
    }

    public int GetManaRecoveryBonus()
    {
        return Convert.ToInt32(value * manaRecoveryBonusPerPoint);
    }

    public int GetMagicalAttackBonus()
    {
        return Convert.ToInt32(value * magicalAttackBonusPerPoint);
    }

    public int GetMagicalDefenseBonus()
    {
        return Convert.ToInt32(value * magicalDefenseBonusPerPoint);
    }
}
