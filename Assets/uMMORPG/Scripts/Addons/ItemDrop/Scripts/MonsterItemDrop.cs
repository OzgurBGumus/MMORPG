using Mirror;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
#if ITEM_DROP_C
    void UpdateClient_ItemDrop()
    {
        if (EventDied())
        {
            if (collider.enabled)
            {
                collider.enabled = false;
            }
        }

        if (EventRespawnTimeElapsed())
        {
            if (!collider.enabled)
            {
                collider.enabled = true;
            }
        }
    }

    void OnDeath_ItemDrop()
    {
        if (AddonItemDrop.ItemPrefab == null)
            return;

        if (gold > 0)
        {
            if (ScriptableItem.dict.TryGetValue(AddonItemDrop.goldHashCode, out var data))
            {
                inventory.Add(new ItemSlot(new Item(data)));
            }
        }

        Vector3 center = new Vector3(transform.position.x, transform.position.y - agent.baseOffset, transform.position.z);

        for (int i = 0; i < inventory.Count; i++)
        {
            ScriptableItem itemData = inventory[i].item.data;

            if (itemData.prefab != null)
            {
                if (!(itemData is PetItem) && !(itemData is MountItem))
                {
                    if (ItemDropSettings.Settings.individualLoot)
                    {
                        if (AddonItemDrop.RandomPoint3D(center, out var point))
                        {
                            // spawn an item on client only and without generating a unique ID
                            RpcDropItem(itemData.name, itemData.gold, gold, center, point);
                        }
                    }
                    else
                    {
                        if (AddonItemDrop.RandomPoint3D(center, out var point))
                        {
                            // spawn an item without generating a unique ID
                            ItemDrop loot = AddonItemDrop.GenerateLoot(itemData.name, itemData.gold, gold, center, point);
                            NetworkServer.Spawn(loot.gameObject);
                        }
                    }
                }
            }
        }        
    }
#endif
#if ITEM_DROP_R
    public void UpdateClient_ItemDrop()
    {
        if (state == "DEAD")
        {
            if (collider.enabled)
            {
                collider.enabled = false;
            }
        }

        if (state == "IDLE")
        {
            if (!collider.enabled)
            {
                collider.enabled = true;
            }
        }
    }
#endif
    [ClientRpc]
    public void RpcDropItem(string itemName, bool currency, long gold, Vector3 center, Vector3 point)
    {
        AddonItemDrop.GenerateLoot("", itemName, currency, gold, center, point);
    }
}
#if ITEM_DROP_R
public partial class MonsterInventory
{
    RegularNavMeshMovement regularMovement;
    void Awake()
    {
        regularMovement = monster.movement as RegularNavMeshMovement;
    }

    void OnDeath_ItemDrop()
    {
        if (AddonItemDrop.ItemPrefab == null)
            return;

        List<string> owners = monster.DroppedItemOwners(slots.Count);

        if (monster.gold > 0)
        {
            if (ScriptableItem.All.TryGetValue(AddonItemDrop.goldHashCode, out var data))
            {
                slots.Add(new ItemSlot(new Item(data)));
            }
        }
        
        Vector3 center = new Vector3(transform.position.x, transform.position.y - regularMovement.agent.baseOffset, transform.position.z);

        for (int i = 0; i < slots.Count; i++)
        {
            ScriptableItem itemData = slots[i].item.data;

            if (itemData.prefab != null)
            {
                if (!(itemData is PetItem) && !(itemData is MountItem))
                {
                    if (ItemDropSettings.Settings.individualLoot)
                    {
                        if (AddonItemDrop.RandomPoint3D(center, out var point))
                        {
                            // spawn an item on client only and without generating a unique ID
                            monster.RpcDropItem(itemData.name, itemData.gold, monster.gold, center, point);
                        }
                    }
                    else
                    {
                        if (AddonItemDrop.RandomPoint3D(center, out var point))
                        {
                            string lootOwner = string.IsNullOrEmpty(owners[i]) ? "" : owners[i];
                            // spawn an item without generating a unique ID
                            ItemDrop loot = AddonItemDrop.GenerateLoot(lootOwner, itemData.name, itemData.gold, monster.gold, center, point);
                            NetworkServer.Spawn(loot.gameObject);
                        }
                    }
                }
            }
        }
    }
}
#endif