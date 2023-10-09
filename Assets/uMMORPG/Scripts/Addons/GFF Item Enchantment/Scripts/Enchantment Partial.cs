using GFFAddons;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class EquipmentItem
{
    [Header("What runes can be used to enchant this item?")]
    //[SerializeField] private UpgradeMaterialItem[] materials;

    [Tooltip("the less the more difficult")]
    [SerializeField] private float difficultyOfEnchantment = 1;
    [SerializeField] private bool considerItemLevel;

    public bool CanUpgradeByMaterial(UpgradeMaterialItem[] material)
    {
        bool result = true;
        for(int j = 0; j < material.Length; j++)
        {
            if (material[j].upgradeMaterialType == null)
            {
                result = false;
                break;
            }
        }
        return result;
    }

    public float LeveFactorForEnchantment()
    {
        return !considerItemLevel ? difficultyOfEnchantment : difficultyOfEnchantment / minLevel;
    }
}
public partial struct Item
{
    public int currentUpgradeLevel;
    public int BonusAsPercentageOfUpgrade(int value)
    {
        return value + (int)(value * (data.upgradeBonusesPercent[currentUpgradeLevel]/100));
    }
    void ToolTip_Upgrade(StringBuilder tip)
    {
        if (data is EquipmentItem item)
        {
            //damage
            tip.Replace("{PATKBONUS}", BonusAsPercentageOfUpgrade(item.physicalAttackBonus).ToString());
            tip.Replace("{MATKBONUS}", BonusAsPercentageOfUpgrade(item.magicalAttackBonus).ToString());

            //defense
            tip.Replace("{PDEFBONUS}", BonusAsPercentageOfUpgrade(item.physicalDefenseBonus).ToString());
            tip.Replace("{MDEFBONUS}", BonusAsPercentageOfUpgrade(item.magicalDefenseBonus).ToString());

            tip.Replace("{MDEFREDUCTIONBONUS}", BonusAsPercentageOfUpgrade(item.magicalDefenseReductionBonus).ToString());
            tip.Replace("{PDEFREDUCTIONBONUS}", BonusAsPercentageOfUpgrade(item.physicalDefenseReductionBonus).ToString());

            tip.Replace("{UPGRADE}", "+" + currentUpgradeLevel.ToString());
        }
    }

}

public partial class Player
{
    [Header("Upgrade Item Addon")]
    public PlayerUpgrade upgrade;
}

public partial class Npc
{
    [Header("Upgrade Item Addon")]
    public NpcEnchantment upgrade;
}

public partial class UIShortcuts
{
    [Header("Upgrade Addon")]
    public Button buttoUpgrade;

    public void Update_Upgrade(Player player)
    {
        buttoUpgrade.gameObject.SetActive(player.upgrade.data.operatingMode == UpgradeAddonMode.shortcuts);
        buttoUpgrade.onClick.SetListener(() =>
        {
            if (UIUpgrade.singleton.panel.activeSelf) UIUpgrade.singleton.Сlose();
            else UIUpgrade.singleton.Show();
        });
    }
}

public partial class UIEquipmentExtended
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UITargetViewEquipment
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UIStorage
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIStorageGuild
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UIMountInventory
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIMountEquipment
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UIMailViewMessage
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIMailNewMessage
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UIItemDurability
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UIGameControlAccount
{
    public void Update_upgrade(UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIGameControlCharacters
{
    public void Update_upgrade(UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIGameControlShop
{
    public void Update_upgrade(UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIGameControlBonuses
{
    public void Update_upgrade(UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIGameControlAuction
{
    public void Update_upgrade(UniversalSlot slot, ItemSlot item)
    {
        //paint upgrade values
        if (item.amount > 0 && item.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + item.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UINpcTradingExtended
{
    public void UpdateItemSlot_upgrade(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //paint upgrade values
        if (itemSlot.amount > 0 && itemSlot.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class UINewQuest
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //paint upgrade values
        if (itemSlot.amount > 0 && itemSlot.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIQuestsByNpc
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //paint upgrade values
        if (itemSlot.amount > 0 && itemSlot.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}
public partial class UIQuestsExtended
{
    public void Update_upgrade(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //paint upgrade values
        if (itemSlot.amount > 0 && itemSlot.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}

public partial class CraftingRecipe
{
    //[Header("Settings : upgrade Addon")]
}
public partial class UICraftingExtended
{
    void UpdateItemSlot_upgrade(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //paint upgrade values
        if (itemSlot.amount > 0 && itemSlot.item.data is EquipmentItem it)
            slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel.ToString();
        else slot.upgradeText.text = "";
    }
}