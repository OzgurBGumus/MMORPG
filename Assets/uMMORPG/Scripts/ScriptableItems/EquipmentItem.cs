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
        string toolTip =
        "<b>" + name + "</b>\n" +
        "\n" +
        (defenseBonus != 0 ? "Defense Bonus: {DEFENSEBONUS}\n" : "") +
        (damageBonus != 0 ? "Pyshical Attack Bonus: {DAMAGEBONUS}\n" : "") +
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
