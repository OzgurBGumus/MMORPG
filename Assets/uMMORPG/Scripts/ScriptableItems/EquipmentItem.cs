using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName="uMMORPG Item/Equipment", order=999)]
public partial class EquipmentItem : UsableItem
{
    [Header("Equipment")]
    public int healthBonus;
    public int manaBonus;
    public int damageBonus;
    public int defenseBonus;
    [Range(0, 1)] public float blockChanceBonus;
    [Range(0, 1)] public float criticalChanceBonus;
    public GameObject modelPrefab;

    // usage
    // -> can we equip this into any slot?
    public override bool CanUse(Player player, int inventoryIndex)
    {
        return FindEquipableSlotFor(player, inventoryIndex) != -1;
    }

    // can we equip this item into this specific equipment slot?
    public bool CanEquip(Player player, int inventoryIndex, int equipmentIndex)
    {
        EquipmentInfo slotInfo = ((PlayerEquipment)player.equipment).slotInfo[equipmentIndex];
        string requiredCategory = slotInfo.requiredCategory.ToString();
        return base.CanUse(player, inventoryIndex) &&
               requiredCategory != "" &&
               category.ToString().StartsWith(requiredCategory.ToString());
    }

    int FindEquipableSlotFor(Player player, int inventoryIndex)
    {
        for (int i = 0; i < player.equipment.slots.Count; ++i)
            if (CanEquip(player, inventoryIndex, i))
                return i;
        return -1;
    }

    // tooltip
    public override string ToolTip()
    {
        StringBuilder tip = new StringBuilder(base.ToolTip());
        tip.Replace("{CATEGORY}", category.ToString());
        //tip.Replace("{DAMAGEBONUS}", damageBonus.ToString());
        //tip.Replace("{DEFENSEBONUS}", defenseBonus.ToString());
        tip.Replace("{HEALTHBONUS}", healthBonus.ToString());
        tip.Replace("{MANABONUS}", manaBonus.ToString());
        tip.Replace("{BLOCKCHANCEBONUS}", Mathf.RoundToInt(blockChanceBonus * 100).ToString());
        tip.Replace("{CRITICALCHANCEBONUS}", Mathf.RoundToInt(criticalChanceBonus * 100).ToString());
        return tip.ToString();
    }
}
