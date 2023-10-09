using UnityEngine;

[DisallowMultipleComponent]
public abstract class Equipment : ItemContainer, IHealthBonus, IManaBonus, ICritBonus, IHitBonus, IDodgeBonus, IPhysicalDefenseBonus, IMagicalDefenseBonus, IMagicalDefenseReductionBonus, IPhysicalAttackBonus, IMagicalAttackBonus, IPhysicalDefenseReductionBonus, ICombatBonus
{
    // boni ////////////////////////////////////////////////////////////////////
    //Convert.ToInt32(value * dodgeBonusPercentPerPoint)
    public int GetHealthBonus()
    {
        // calculate equipment bonus
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).healthBonus;
        return bonus;
    }

    public int GetHealthRecoveryBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).healthRecoveryBonus;
        return bonus;
    }

    public int GetManaBonus()
    {
        // calculate equipment bonus
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).manaBonus;
        return bonus;
    }

    public int GetManaRecoveryBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).ManaRecoveryBonus;
        return bonus;
    }

    public int GetCritBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).critBonus;
        return bonus;
    }

    public int GetHitBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).hitBonus;
        return bonus;
    }

    public int GetDodgeBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).dodgeBonus;
        return bonus;
    }

    public int GetPhysicalDefenseBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).physicalDefenseBonus;
        return bonus;
    }

    public int GetPhysicalDefenseReductionBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).physicalDefenseReductionBonus;
        return bonus;
    }

    public int GetMagicalDefenseBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).magicalDefenseBonus;
        return bonus;
    }

    public int GetMagicalDefenseReductionBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).magicalDefenseReductionBonus;
        return bonus;
    }

    public int GetPhysicalAttackBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).physicalAttackBonus;
        return bonus;
    }

    public int GetMagicalAttackBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).magicalAttackBonus;
        return bonus;
    }

    public int GetAttackSpeedBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).attackSpeedBonus;
        return bonus;
    }

    public int GetCastSpeedBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).castSpeedBonus;
        return bonus;
    }

    public int GetMoveSpeedBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).moveSpeedBonus;
        return bonus;
    }

    public int GetLuckBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).luckBonus;
        return bonus;
    }

    public int GetCritDamageBonus()
    {
        // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
        int bonus = 0;
        foreach (ItemSlot slot in slots)
            if (slot.amount > 0 && slot.item.CheckDurability())
                bonus += ((EquipmentItem)slot.item.data).critDamageBonus;
        return bonus;
    }

    //public int GetDamageBonus()
    //{
    //    // calculate equipment bonus
    //    // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
    //    int bonus = 0;
    //    foreach (ItemSlot slot in slots)
    //        if (slot.amount > 0 && slot.item.CheckDurability())
    //            bonus += slot.item.BonusAsPercentageOfUpgrade(((EquipmentItem)slot.item.data).damageBonus);
    //    return bonus;
    //}

    //public int GetDefenseBonus()
    //{
    //    // calculate equipment bonus
    //    // sum up manually. Linq.Sum() is HEAVY(!) on GC and performance (190 KB/call!)
    //    int bonus = 0;
    //    foreach (ItemSlot slot in slots)
    //        if (slot.amount > 0 && slot.item.CheckDurability())
    //            bonus += slot.item.BonusAsPercentageOfUpgrade(((EquipmentItem)slot.item.data).defenseBonus);
    //    return bonus;
    //}

    ////////////////////////////////////////////////////////////////////////////
    // helper function to find the equipped weapon index
    // -> works for all entity types. returns -1 if no weapon equipped.
    public int GetEquippedWeaponIndex()
    {
        // (avoid FindIndex to minimize allocations)
        for (int i = 0; i < slots.Count; ++i)
        {
            ItemSlot slot = slots[i];
            if (slot.amount > 0 && slot.item.data is WeaponItem)
                return i;
        }
        return -1;
    }

    // get currently equipped weapon category to check if skills can be casted
    // with this weapon. returns "" if none.
    public string GetEquippedWeaponCategory()
    {
        // find the weapon slot
        int index = GetEquippedWeaponIndex();
        return index != -1 ? ((WeaponItem)slots[index].item.data).category.ToString() : "";
    }

}
