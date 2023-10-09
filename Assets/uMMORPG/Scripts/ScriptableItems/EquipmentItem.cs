using System.Text;
using UnityEngine;
[CreateAssetMenu(menuName="uMMORPG Item/Equipment", order=999)]
public partial class EquipmentItem : UsableItem
{
    [Header("Equipment")]
    public int healthBonus;
    public int healthRecoveryBonus;
    public int manaBonus;
    public int ManaRecoveryBonus;
    /*[Range(0, 1)]*/ public int dodgeBonus;
    public int critBonus;
    public int hitBonus;
    public int physicalDefenseBonus;
    public int physicalDefenseReductionBonus;
    public int magicalDefenseBonus;
    public int magicalDefenseReductionBonus;
    public int physicalAttackBonus;
    public int magicalAttackBonus;
    public int attackSpeedBonus;
    public int castSpeedBonus;
    public int moveSpeedBonus;
    public int luckBonus;
    public int critDamageBonus;
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
        string toolTip =
        "<b>" + name + "</b>\n" +
        "\n" +
        (physicalDefenseBonus != 0 ? "PDEF: {PDEF}\n" : "") +
        (physicalDefenseReductionBonus != 0 ? "PDEF Reduction: {PDEFREDUCTION}\n" : "") +
        (magicalDefenseBonus != 0 ? "MDEF: {MDEF}\n" : "") +
        (magicalDefenseReductionBonus != 0 ? "MDEF Reduction: {MDEFREDUCTION}\n" : "") +
        (physicalAttackBonus != 0 ? "PATK: {PATK}\n" : "") +
        (magicalAttackBonus != 0 ? "MATK: {MATK}\n" : "") +
        "Durability: {DURABILITY}%\n" +
        "Destroyable: " + (destroyable ? "Yes" : "No") + "\n" +
        "Sellable: " + (sellable ? "Yes" : "No") + "\n" +
        "Tradable: " + (tradable ? "Yes" : "No") + "\n" +
        "Required Level: " + minLevel + "\n" +
        "\n" +
        "Enchantment: {UPGRADE}\n" +
        "\n" +
        "Price: " + buyPrice.ToString() + " Gold\n" +
        "<i>Sells for: " + sellPrice.ToString() + " Gold</i>\n" +
        "\n" +
        base.toolTip;
        return toolTip;
    }
}
