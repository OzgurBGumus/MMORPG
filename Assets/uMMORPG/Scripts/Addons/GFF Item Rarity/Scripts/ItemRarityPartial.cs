using UnityEngine;
using System.Text;
using System;

public enum ItemRarity { Normal, Rare, Elite, Epic, Legendary, Mythical }

[Serializable] public class ItemTypes
{
    public ItemRarity rarity;
    public Color color;
}

public partial class ScriptableItem
{
    [Header("GFF Item Rarity")]
    [SerializeField] private ItemRarity rarity;

    public ItemRarity GetItemRarity
    {
        get
        {
            return rarity;
        }
    }
}

public partial struct Item
{
    void ToolTip_rarityType(StringBuilder tip)
    {
        if (data.GetItemRarity != ItemRarity.Normal)
        {
            Player player = Player.localPlayer;
            string color = "<color=#" + ColorUtility.ToHtmlStringRGBA(player.itemRarityConfig.GetColor(this)) + ">";
            tip.Replace("{ITEMRARITY}", "<b>" + color + data.GetItemRarity.ToString() + "</color></b>");
        }
        else tip.Replace("{ITEMRARITY}", data.GetItemRarity.ToString());
    }
}

public partial class Player
{
    [Header("GFF Item Rarity Addon")]
    public ScriptableItemRarity itemRarityConfig;

    void rarityValue_rarity(Item item)
    {
        //if used item enchantment Addon
        //enchantment.rarityIndex = itemRarityConfig.GetRarityIndex(item);
    }
}

public partial class UIInventoryExtended
{
    //void Update_rarity(Player player, UIInventoryExtendedSlot slot, ItemSlot itemSlot)
    //{
    //    //border
    //    slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

    //    //color
    //    if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
    //    else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    //}
}

public partial class UIEquipmentExtended
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UITargetViewEquipment
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIGameManagementEquipment
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIStorage
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIStorageGuild
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UILootExtended
{
    void Update_rarity(Player player, UniversalSlot slot, Item item)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(item);
    }
}

public partial class UIMailNewMessage
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIMailViewMessage
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIDailyRewards
{
    void Update_rarity(Player player, UniversalSlot slot, ScriptableItem item)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(new Item(item));
    }
}

public partial class UIGathering
{
    void Update_rarity(Player player, UniversalSlot slot, ScriptableItem item)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(new Item(item));
    }
}

public partial class UICraftingExtended
{
    void UpdateItem_rarity(Player player, UniversalSlot slot, ScriptableItem item)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(new Item(item));
    }

    void UpdateItemSlot_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UINpcTradingExtended
{
    void UpdateItemSlot_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIQuestsExtended
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
    }
}

public partial class UINewQuest
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIQuestsByNpc
{
    public void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot(); ;
    }
}

public partial class UIAuction
{
    void Update_rarity(Player player, UniversalSlot slot, Item item)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(item);
    }
}

public partial class UIRegistrationItemOnAuction
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        slot.rarityImage.sprite = player.itemRarityConfig.GetSprite;

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIPetsExtended
{
    [Header("Settings : Item Rarity addon")]
    public bool useRarityAddon;

    public void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        //slot.rarityImage.sprite = ItemRarity.singleton.GetSprite();

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIUpgrade
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //border
        //slot.rarityImage.sprite = ItemRarity.singleton.GetSprite();

        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIItemDurability
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIMountInventory
{
    public void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIMountEquipment
{
    public void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIGameControlAccount
{
    public void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIGameControlCharacters
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIGameControlShop
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
    }
}

public partial class UIGameControlBonuses
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
    }
}

public partial class UIGameControlAuction
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIOreProcessing
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}

public partial class UIMacrosPotion
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}
public partial class UIMacrosAction
{
    void Update_rarity(Player player, UniversalSlot slot, ItemSlot itemSlot)
    {
        //color
        if (itemSlot.amount > 0) slot.rarityImage.color = player.itemRarityConfig.GetColor(itemSlot.item);
        else slot.rarityImage.color = player.itemRarityConfig.GetColorForEmptySlot();
    }
}