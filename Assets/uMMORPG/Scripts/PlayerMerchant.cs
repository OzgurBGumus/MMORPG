using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerInventory))]
[DisallowMultipleComponent]
public class PlayerMerchant : NetworkBehaviour
{
    [Header("Components")]
    public Player player;
    public PlayerInventory inventory;
    public List<int> offerItems = new List<int>();
    // Value[0] = offerItemSlot, Value[1] = Price
    public List<int> itemPrices = new List<int>();
    public List<int> itemAmounts = new List<int>();
    public Player merchantPlayer;
    public bool isInMerchant;
    //public PlayerMerchant merchant;
    // Start is called before the first frame update
    public override void OnStartServer()
    {

        for (int i = 0; i < 16; ++i)
        {
            offerItems.Add(-1);
            itemPrices.Add(-1);
            itemAmounts.Add(-1);
        }
            
            
    }

    public bool CanStartMerchant()
    {
        // a player can only trade if he is not trading already and alive
        return player.health.current > 0 && player.state == "IDLE";
    }

    public void AddItemToMerchant(int inventoryIndex, int merchantIndex, int amount, int price)
    {
        if (
            player.state == "IDLE" &&
            merchantIndex >= 0 && merchantIndex < offerItems.Count && !offerItems.Contains(inventoryIndex)
        )
        {
            ItemSlot slot = inventory.slots[inventoryIndex];
            if (slot.item.tradable &&
                !slot.item.summoned &&
                slot.amount > 0)
            {
                offerItems[merchantIndex] = inventoryIndex;
                itemPrices[merchantIndex] = price;
                itemAmounts[merchantIndex] = amount;
            }


        }
    }

    public void RemoveItemFromMerchant(int index)
    {
        if (player.state == "IDLE" && index >= 0 && index < offerItems.Count)
        {
            offerItems[index] = -1;
            itemPrices[index] = -1;
            itemAmounts[index] = -1;
        }
    }


    public void Cleanup()
    {
        for (int i = 0; i < offerItems.Count; ++i)
        {
            offerItems[i] = -1;
            itemAmounts[i] = -1;
            itemPrices[i] = -1;
        }
            
        isInMerchant = false;
    }

    [Command]
    public void CmdBuyItem(int merchantSlot, int inventorySlot, int amount)
    {
        if (
            player.state != "TRADING" && player.state != "CRAFTING" &&
            player.state != "DEAD" && player.state != "MERCHANT" &&
            merchantPlayer != null)
        {
            //otherPlayer is a merchant?
            if (merchantPlayer.state == "MERCHANT")
            {
                //check the gold and the item still exist
                if (
                    IsGoldStillValid(merchantPlayer.merchant.itemPrices[merchantSlot] * amount) &&
                    merchantPlayer.merchant.IsItemStillValid(merchantSlot, amount)
                    )
                {
                    ItemSlot item = merchantPlayer.inventory.slots[merchantPlayer.merchant.offerItems[merchantSlot]];
                    item.amount = amount;
                    int remainedAmount = item.amount - amount;
                    //////ADD ITEM TO BUYER
                    if (inventory.slots[inventorySlot].amount != 0 && inventory.SlotsFree() > 0)
                    {
                        //it the selectod slot is not empty? then add it as normal
                        inventory.Add(item.item, amount);
                    }
                    else
                    {
                        //so the choosen slot is empty
                        item.amount = amount;
                        inventory.slots[inventorySlot] = item;
                    }
                    //////REMOVE GOLD FROM BUYER
                    player.gold -= merchantPlayer.merchant.itemPrices[merchantSlot] * amount;

                    /////REMOVE ITEM FROM MERCHANT
                    item.amount = remainedAmount;
                    merchantPlayer.inventory.slots[merchantPlayer.merchant.offerItems[merchantSlot]] = item;

                    /////ADD GOLD TO THE MERCHANT
                    merchantPlayer.gold += merchantPlayer.merchant.itemPrices[merchantSlot] * amount;

                    /////REMOVE ITEM FROM MERCHANT
                    merchantPlayer.merchant.RemoveItemFromMerchant(merchantSlot, amount);

                }
                else
                {
                    Debug.Log("insufficient gold or item");
                }
            }
            else
            {
                Debug.Log("No open merchant found");
            }
        }
        Debug.Log("current player's state is not accepted or merchantPlayer is not found.");
    }


    [Command]
    public void CmdStartMerchant(int[] _offerItems, int[] _itemPrices, int[] _itemAmounts)
    {
        for(int i=0; i<_offerItems.Length; i++)
        {
            offerItems[i] = _offerItems[i];
            itemPrices[i] = _itemPrices[i];
            itemAmounts[i] = _itemAmounts[i];
        }
        isInMerchant = true;
    }
    [Command]
    public void EndMerchant()
    {
        isInMerchant = false;
    }
    public void OnOpenMerchant(Player mPlayer)
    {
        merchantPlayer = mPlayer;
    }
    void RemoveItemFromMerchant(int merchantSlot, int amount)
    {
        itemAmounts[merchantSlot] -= amount;
        if(itemAmounts[merchantSlot] == 0)
        {
            itemPrices[merchantSlot] = -1;
            itemAmounts[merchantSlot] = -1;
            offerItems[merchantSlot] = -1;

        }
    }
    [Server]
    bool IsGoldStillValid(int gold)
    {
        return player.gold >= gold;
    }
    [Server]
    bool IsItemStillValid(int merchantSlot, int amount)
    {
        int inventorySlot = offerItems[merchantSlot];

        return itemAmounts[merchantSlot] > amount && inventory.slots[inventorySlot].amount >= amount && IsInventorySlotMerchantable(inventorySlot);
    }
    bool IsInventorySlotMerchantable(int index)
    {
        return 0 <= index && index < inventory.slots.Count &&
               inventory.slots[index].amount > 0 &&
               inventory.slots[index].item.tradable;
    }

    // drag & drop /////////////////////////////////////////////////////////////
    void OnDragAndDrop_InventorySlot_MerchantSlot(int[] slotIndices)
    {
        if (inventory.slots[slotIndices[0]].item.tradable)
        {
            AddItemToMerchant(slotIndices[0], slotIndices[1], 1, 2);
        }
            
    }
    // drag & drop /////////////////////////////////////////////////////////////

    void OnDragAndClear_TradingSlot(int slotIndex)
    {
        RemoveItemFromMerchant(slotIndex);
    }

}
