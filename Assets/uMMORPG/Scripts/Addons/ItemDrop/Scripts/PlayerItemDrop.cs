using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    [HideInInspector] public ItemDrop targetItem;
    [HideInInspector] public bool lockRaycast;
    
    public void ControlRaycast(bool state)
    {
        lockRaycast = state;
    }

    public void CancelAction()
    {        
#if ITEM_DROP_C
        agent.ResetPath();
        SetIndicatorViaParent(transform);
#endif
#if ITEM_DROP_R
        movement.Reset();
        indicator?.SetViaParent(transform);
#endif
        CmdCancelAction();

        targetItem = null;
    }

    public void UpdateClient_HighlightLoot()
    {
        if (ItemDropSettings.Settings.highlightAlways && LootManager.instance.HighlightLoot != true) LootManager.instance.HighlightLoot = true;
        else
        {
            if (Input.GetKeyDown(ItemDropSettings.Settings.itemHighlightKey))
            {
                LootManager.instance.HighlightLoot = true;
            }

            if (Input.GetKeyUp(ItemDropSettings.Settings.itemHighlightKey))
            {
                LootManager.instance.HighlightLoot = false;
            }
        }
    }
#if ITEM_DROP_C
    void OnDragAndClear_InventorySlot(int slotIndex)
    {
        if (!Utils.IsCursorOverUserInterface()) CmdDropItemFromInventory(slotIndex);
    }

    void OnDragAndClear_EquipmentSlot(int slotIndex)
    {
        if (!Utils.IsCursorOverUserInterface()) CmdDropItemFromEquipment(slotIndex);
    }

    [Command]
    public void CmdDropItemFromInventory(int index)
    {
        if (AddonItemDrop.ItemPrefab == null)
            return;

        if ((state == "IDLE" || state == "MOVING") && 0 <= index && index < inventory.Count && inventory[index].amount > 0)
        {
            ItemSlot slot = inventory[index];
            if (slot.item.data.prefab != null)
            {
                if (!(slot.item.data is PetItem) && !(slot.item.data is MountItem))
                {                    
                    Vector3 targetPoint = transform.position + transform.forward;

                    ItemDrop clone = AddonItemDrop.GenerateItem(slot.item.name, slot.amount, this, targetPoint);

                    slot.amount = 0;
                    inventory[index] = slot;

                    NetworkServer.Spawn(clone.gameObject);                    
                }
            }
        }
    }

    [Command]
    public void CmdDropItemFromEquipment(int index)
    {
        if (AddonItemDrop.ItemPrefab == null)
            return;

        if ((state == "IDLE" || state == "MOVING") && 0 <= index && index < equipment.Count && equipment[index].amount > 0)
        {
            ItemSlot slot = equipment[index];
            if (slot.item.data.prefab != null)
            {
                if (!(slot.item.data is PetItem) && !(slot.item.data is MountItem))
                {
                    Vector3 targetPoint = transform.position + transform.forward;

                    ItemDrop clone = AddonItemDrop.GenerateItem(slot.item.name, slot.amount, this, targetPoint);

                    slot.amount = 0;
                    equipment[index] = slot;

                    NetworkServer.Spawn(clone.gameObject);                    
                }
            }
        }
    }
#endif
    [Client]
    public void FindNearestItem()
    {
        if (targetItem != null)
            return;

        ItemDrop result = null;
        float distance = float.PositiveInfinity;
        IEnumerator<ItemDrop> itemEnum = LootManager.instance.Items.GetEnumerator();

        while (itemEnum.MoveNext())
        {
            if (itemEnum.Current.IsVisible)
            {
                if (AddonItemDrop.ClosestDistance(this, itemEnum.Current) <= ItemDropSettings.Settings.itemPickupRadius)
                {
                    float currentDist = (itemEnum.Current.transform.position - transform.position).sqrMagnitude;
                    if (currentDist < distance)
                    {
                        result = itemEnum.Current;
                        distance = currentDist;
                    }
                }
            }
        }
        SetTargetItem(result);
    }

    public void SetTargetItem(ItemDrop item)
    {
#if UMMORPG_3D_C || UMMORPG_2D_C || UMMORPG_3D_R || UMMORPG_2D_R
        // Management Tools
        if (IsFrozen())
            return;
#endif
        if (item != null)
        {
            if (state == "IDLE" || state == "MOVING")
            {
#if ITEM_DROP_C
                // check distance between character and target item
                if (AddonItemDrop.ClosestDistance(this, item) <= 1)
                {
                    // is close enough, try to pick it up
                    if (InventoryCanAdd(new Item(item.data), item.stack) || item.data.gold)
                    {
                        targetItem = item;
                        ItemPickup(item);
                    }
                    // information about the lack of space in the inventory
                    else
                    {
                        CmdMsgBackpackIsFull();
                    }
                }
                // too far to reach
                else
                {
                    SetIndicatorViaParent(item.transform);

                    // enough space in the inventory, move closer automatically to pick the item up
                    if (InventoryCanAdd(new Item(item.data), item.stack) || item.data.gold)
                    {
                        targetItem = item;

                        Navigate(AddonItemDrop.ClosestPoint(item, transform.position), 1.5f);
                    }
                    // information about the lack of space in the inventory
                    else
                    {
                        CmdMsgBackpackIsFull();
                    }
                }
#endif
#if ITEM_DROP_R
                if (AddonItemDrop.ClosestDistance(this, item) <= ItemDropSettings.Settings.itemPickupRadius)
                {
                    // is close enough, try to pick it up
                    if (inventory.CanAdd(new Item(item.data), item.stack) || item.data.gold)
                    {
                        targetItem = item;
                        ItemPickup(item);
                    }
                    // information about the lack of space in the inventory
                    else
                    {
                        CmdMsgBackpackIsFull();
                    }
                }
                // too far to reach
                else
                {
                    indicator.SetViaParent(item.transform);

                    // enough space in the inventory, move closer automatically to pick the item up
                    if (inventory.CanAdd(new Item(item.data), item.stack) || item.data.gold)
                    {
                        targetItem = item;

                        movement.Navigate(AddonItemDrop.ClosestPoint(item, transform.position), 1.5f);
                    }
                    // information about the lack of space in the inventory
                    else
                    {
                        CmdMsgBackpackIsFull();
                    }
                }
#endif
            }
        }
    }

    [Command]
    public void CmdMsgBackpackIsFull()
    {
        chat.TargetMsgInfo("Your backpack is full!");
    }

    public void ItemPickup(ItemDrop item)
    {
        if (item != null)
        {
            if (TakeGold(item))
            {
                if (targetItem != null)
                {
                    Destroy(targetItem.gameObject);
                    targetItem = null;
                }
                return;
            }

            Loot loot = new Loot
            {
                hashCode = item.name.GetStableHashCode(),
                uniqueId = item.uniqueId,
                stack = item.stack
            };
            CmdAddItem(item.gameObject, loot);

            if (targetItem != null)
            {
                Destroy(targetItem.gameObject);
                targetItem = null;
            }           
        }
    }
 
    public bool TakeGold(ItemDrop item)
    {
        // gold can always be picked up
        if (item.data.gold)
        {
            Loot loot = new Loot
            {
                uniqueId = item.uniqueId,
                stack = item.stack
            };

            CmdAddGold(item.gameObject, loot);
            return true;
        }
        return false;
    }

    [Command]
    public void CmdAddGold(GameObject item, Loot loot)
    {
        // if the gold sharing option is active, it will be shared among nearby party members
#if ITEM_DROP_C
        if (InParty() && party.shareGold)
        {
            // find all party members nearby
            List<Player> closeMembers = GetPartyMembersInProximity();

            // the amount of gold is calculated by the formula from uMMORPG
            long share = (long)Mathf.Ceil((float)loot.stack / (float)closeMembers.Count);

            foreach (Player member in closeMembers)
            {
                member.gold += share;

                if (ItemDropSettings.Settings.showMessages)
                {
                    string message = $"Gold (<color=#ADFF2F>{share}</color>) has been placed in your backpack.";
                    member.chat.TargetMsgInfo(message);
                }
            }
        }
        else
        {
            gold += loot.stack;

            if (ItemDropSettings.Settings.showMessages)
            {
                string message = $"You put Gold (<color=#ADFF2F>{loot.stack}</color>) in your backpack.";
                chat.TargetMsgInfo(message);
            }
        }
#endif
#if ITEM_DROP_R
        if (party.InParty() && party.party.shareGold)
        {
            // find all party members nearby
            List<Player> closeMembers = party.GetMembersInProximity();

            // the amount of gold is calculated by the formula from uMMORPG
            long share = (long)Mathf.Ceil((float)loot.stack / (float)closeMembers.Count);

            foreach (Player member in closeMembers)
            {
                member.gold += share;

                if (ItemDropSettings.Settings.showMessages)
                {
                    string message = $"Gold (<color=#ADFF2F>{share}</color>) has been placed in your backpack.";
                    member.chat.TargetMsgInfo(message);
                }
            }
        }
        else
        {
            gold += loot.stack;

            if (ItemDropSettings.Settings.showMessages)
            {
                string message = $"You put Gold (<color=#ADFF2F>{loot.stack}</color>) in your backpack.";
                chat.TargetMsgInfo(message);
            }
        }
#endif
        if (item != null)
        {
            //Debug.Log($"server item: {item}");
            AddonItemDrop.DeleteItem(loot.uniqueId);
            NetworkServer.Destroy(item);
        }
    }

    [Command]
    public void CmdAddItem(GameObject item, Loot loot)
    {
#if ITEM_DROP_C
        // try to find an item in the item dictionary by hashCode
        if (ScriptableItem.dict.TryGetValue(loot.hashCode, out var data))
        {
            // enough space in the inventory, pick up the item
            if (InventoryAdd(new Item(data), loot.stack))
            {
                if (ItemDropSettings.Settings.showMessages)
                {
                    string message = loot.stack == 1 ? $"You put [{data.name}] in your backpack" : $"You put [{data.name}] (<color=#ADFF2F>{loot.stack}</color>) in your backpack.";
                    chat.TargetMsgInfo(message);
                }

                if (item != null)
                {
                    //Debug.Log($"server item: {item}");
                    AddonItemDrop.DeleteItem(loot.uniqueId);
                    NetworkServer.Destroy(item);
                }
            }
        }
#endif
#if ITEM_DROP_R
        // try to find an item in the item dictionary by hashCode
        if (ScriptableItem.All.TryGetValue(loot.hashCode, out var data))
        {
            // enough space in the inventory, pick up the item
            if (inventory.Add(new Item(data), loot.stack))
            {
                if (ItemDropSettings.Settings.showMessages)
                {
                    string message = loot.stack == 1 ? $"You put [{data.name}] in your backpack" : $"You put [{data.name}] (<color=#ADFF2F>{loot.stack}</color>) in your backpack.";
                    chat.TargetMsgInfo(message);
                }

                if (item != null)
                {
                    //Debug.Log($"server item: {item}");
                    AddonItemDrop.DeleteItem(loot.uniqueId);
                    NetworkServer.Destroy(item);
                }
            }
        }
#endif
    }

    #region GoldLoss
    public void GoldLoss()
    {
        if (!ItemDropSettings.Settings.goldLoss.isActive)
            return;

        if (AddonItemDrop.ItemPrefab != null)
        {
            Vector3 targetPoint = transform.position + transform.forward;

            if (gold > 0)
            {
                long goldLoss = (long)Mathf.Round(ItemDropSettings.Settings.goldLoss.percentageLoss * gold / 100);
                //Debug.Log(goldLoss);

                ItemDrop loot = AddonItemDrop.GenerateItem("Gold", Convert.ToInt32(goldLoss), this, targetPoint);

                gold -= goldLoss;

                NetworkServer.Spawn(loot.gameObject);

                string message = $"You died and lost <color=#ADFF2F>{goldLoss}</color> gold.";
                chat.TargetMsgInfo(message);
            }
        }
    }
    #endregion

    #region ItemLoss
    public void ItemLoss()
    {
        if (!ItemDropSettings.Settings.itemLoss.isActive)
            return;

        if (AddonItemDrop.ItemPrefab != null)
        {
#if ITEM_DROP_C
            if (InventorySlotsOccupied() > 0)
            {
                List<int> itemSlots = new List<int>();

                for (int i = 0; i < inventory.Count; ++i)
                {
                    ItemSlot slot = inventory[i];
                    if (slot.amount > 0 && slot.item.destroyable)
                    {
                        if (slot.item.data.prefab != null)
                        {
                            if (!(slot.item.data is PetItem) && !(slot.item.data is MountItem))
                            {
                                if (!itemSlots.Contains(i))
                                {
                                    itemSlots.Add(i);
                                }
                            }
                        }
                    }
                }

                List<int> randomItems = AddonItemDrop.GetRandomElements(itemSlots, ItemDropSettings.Settings.itemLoss.amount);
                //Debug.Log(string.Join(", ", randomItems));

                Vector3 targetPoint = transform.position + transform.forward;
                
                for (int i = 0; i < randomItems.Count; ++i)
                {
                    int targetSlot = randomItems[i];
                    if (AddonItemDrop.RandomPoint3D(targetPoint, out var point))
                    {
                        ItemSlot slot = inventory[targetSlot];
                        ItemDrop clone = AddonItemDrop.GenerateItem(slot.item.name, slot.amount, this, point);

                        slot.amount = 0;
                        inventory[targetSlot] = slot;

                        NetworkServer.Spawn(clone.gameObject);
                    }
                }
            }
#endif
#if ITEM_DROP_R
            if (inventory.SlotsOccupied() > 0)
            {
                List<int> itemSlots = new List<int>();

                for (int i = 0; i < inventory.slots.Count; ++i)
                {
                    ItemSlot slot = inventory.slots[i];
                    if (slot.amount > 0 && slot.item.destroyable)
                    {
                        if (slot.item.data.prefab != null)
                        {
                            if (!(slot.item.data is PetItem) && !(slot.item.data is MountItem))
                            {
                                if (!itemSlots.Contains(i))
                                {
                                    itemSlots.Add(i);
                                }
                            }
                        }
                    }
                }

                List<int> randomItems = AddonItemDrop.GetRandomElements(itemSlots, ItemDropSettings.Settings.itemLoss.amount);
                //Debug.Log(string.Join(", ", randomItems));

                Vector3 targetPoint = transform.position + transform.forward;

                for (int i = 0; i < randomItems.Count; ++i)
                {
                    int targetSlot = randomItems[i];
                    if (AddonItemDrop.RandomPoint3D(targetPoint, out var point))
                    {
                        ItemSlot slot = inventory.slots[targetSlot];
                        ItemDrop clone = AddonItemDrop.GenerateItem(slot.item.name, slot.amount, this, point);

                        slot.amount = 0;
                        inventory.slots[targetSlot] = slot;

                        NetworkServer.Spawn(clone.gameObject);
                    }
                }
            }
#endif
        }
    }
    #endregion

    void OnDeath_ItemDrop()
    {
        GoldLoss();
        ItemLoss();
    }

    #region Commands
    [Command]
    public void CmdMsgSaveItems()
    {
        string message = AddonItemDrop.SaveItems() ? "All items have been saved." : "All items are already saved.";
        chat.TargetMsgInfo(message);
    }

    [Command]
    public void CmdMsgDeleteItems()
    {
        AddonItemDrop.DeleteItems();

        string message = "All items have been deleted from the database!";
        chat.TargetMsgInfo(message);
    }

    [TargetRpc]
    public void TargetDeleteItems()
    {
        foreach (ItemDrop item in LootManager.instance.Items)
        {
            if (item.uniqueId == "")
            {
                Destroy(item.gameObject);
            }
        }
    }
    #endregion
}
#if ITEM_DROP_R
public partial class PlayerInventory
{
    void OnDragAndClear_InventorySlot(int slotIndex)
    {
        if (!Utils.IsCursorOverUserInterface()) CmdDropItemFromInventory(slotIndex);
    }

    [Command]
    public void CmdDropItemFromInventory(int index)
    {
        if (AddonItemDrop.ItemPrefab == null)
            return;

        if ((player.state == "IDLE" || player.state == "MOVING") && 0 <= index && index < slots.Count && slots[index].amount > 0)
        {
            ItemSlot slot = slots[index];
            if (slot.item.data.prefab != null)
            {
                if (!(slot.item.data is PetItem) && !(slot.item.data is MountItem))
                {
                    Vector3 targetPoint = player.transform.position + player.transform.forward;

                    ItemDrop clone = AddonItemDrop.GenerateItem(slot.item.name, slot.amount, player, targetPoint);

                    slot.amount = 0;
                    slots[index] = slot;

                    NetworkServer.Spawn(clone.gameObject);
                }
            }
        }
    }
}

public partial class PlayerEquipment
{
    void OnDragAndClear_EquipmentSlot(int slotIndex)
    {
        if (!Utils.IsCursorOverUserInterface()) CmdDropItemFromEquipment(slotIndex);
    }

    [Command]
    public void CmdDropItemFromEquipment(int index)
    {
        if (AddonItemDrop.ItemPrefab == null)
            return;

        if ((player.state == "IDLE" || player.state == "MOVING") && 0 <= index && index < slots.Count && slots[index].amount > 0)
        {
            ItemSlot slot = slots[index];
            if (slot.item.data.prefab != null)
            {
                if (!(slot.item.data is PetItem) && !(slot.item.data is MountItem))
                {
                    Vector3 targetPoint = player.transform.position + player.transform.forward;

                    ItemDrop clone = AddonItemDrop.GenerateItem(slot.item.name, slot.amount, player, targetPoint);

                    slot.amount = 0;
                    slots[index] = slot;

                    NetworkServer.Spawn(clone.gameObject);
                }
            }
        }
    }
}
#endif