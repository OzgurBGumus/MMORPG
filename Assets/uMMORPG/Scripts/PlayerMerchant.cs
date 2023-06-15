using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerInventory))]
[DisallowMultipleComponent]
public class PlayerMerchant : NetworkBehaviour
{
    [Header("Components")]
    public Player player;
    public PlayerInventory inventory;

    public TextMeshPro overlay;
    public GameObject overlayContainer;
    // Value[0] = offerItemSlot, Value[1] = Price

    [SyncVar]public Player merchantPlayer;
    public bool isInMerchant;
    [SyncVar]public string title;
    //the UIMerchant will check this variable, so we'll know if we drag an item to outside and wqe need to remove it.
    public int removeFromMerchantSlot = -1;

    [HideInInspector]public int selectedInventorySlot = -1;
    [HideInInspector]public int selectedMerchantSlot = -1;
    [HideInInspector]public int selectedBuyMerchantSlot = -1;
    [HideInInspector]public int selectedBuyInventorySlot = -1;

    public readonly SyncList<int> offerItems = new SyncList<int>();
    public readonly SyncList<int> itemPrices = new SyncList<int>();
    public readonly SyncList<int> itemAmounts = new SyncList<int>();

    //public PlayerMerchant merchant;
    // Start is called before the first frame update
    private void Start()
    {
        selectedInventorySlot = -1;
        selectedMerchantSlot = -1;
        selectedBuyMerchantSlot = -1;
        selectedBuyInventorySlot = -1;
        removeFromMerchantSlot = -1;
    }
    void Update()
    {
        // update overlays in any case, except on server-only mode
        // (also update for character selection previews etc. then)
        if (!isServerOnly)
        {
            if (player.state == "MERCHANT")
            {
                if (!overlayContainer.activeSelf)
                {
                    overlay.text = title;
                    overlayContainer.SetActive(true);
                }
            }
            else
            {
                if (overlayContainer.activeSelf)
                {
                    overlayContainer.SetActive(false);

                }
            }
        }
    }
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
        // a player can only trade if he is not trading already and alive and if he have some items in merchant
        if (player.health.current > 0 && player.state == "IDLE")
        {
            return true;
        }
        return false;
    }

    public void RemoveItemFromMerchant(int index)
    {
        removeFromMerchantSlot = index;
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
        merchantPlayer = null;
        selectedInventorySlot = -1;
        selectedMerchantSlot = -1;
        selectedBuyMerchantSlot = -1;
        selectedBuyInventorySlot = -1;
    }

    [Command]
    public void CmdBuyItem(int merchantSlot, int inventorySlot, int amount)
    {
        Debug.Log("Player(" + player.name + ") Tries to Buy item from Merchant(" + merchantPlayer.name + ") in Server");
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
                    bool isInventoryOk = true;
                    ItemSlot item = merchantPlayer.inventory.slots[merchantPlayer.merchant.offerItems[merchantSlot]];
                    int remainedAmount = item.amount - amount;
                    item.amount = amount;
                    //////ADD ITEM TO BUYER
                    if (inventory.slots[inventorySlot].amount > 0 && inventory.SlotsFree() > 0)
                    {
                        if (inventory.CanAdd(item.item, amount))
                        {
                            //is the selected slot is not empty? then add it as normal
                            inventory.Add(item.item, amount);
                        }
                        else isInventoryOk = false;
                    }
                    else
                    {
                        //so the choosen slot is empty
                        item.amount = amount;
                        inventory.slots[inventorySlot] = item;
                    }
                    if (isInventoryOk)
                    {
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
                        Debug.Log("Not Enough Space in inventory");
                    }
                    

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
        else Debug.Log("current player's state is not accepted or merchantPlayer is not found.");
    }

    [Command]
    public void CmdStartMerchant(List<int> _offerItems, List<int> _itemPrices, List<int> _itemAmounts, string _title)
    {
        for(int i=0; i<_offerItems.Count; i++)
        {
            offerItems[i] = _offerItems[i];
            itemPrices[i] = _itemPrices[i];
            itemAmounts[i] = _itemAmounts[i];
        }
        isInMerchant = true;
        title = _title;
    }
    [Command]
    public void EndMerchant()
    {
        isInMerchant = false;
        overlayContainer.SetActive(false);
    }

    public void OnOpenMerchant(Player mPlayer)
    {
        merchantPlayer = mPlayer;
        CmdOpenMerchant(mPlayer);
    }
    [Command]
    public void CmdOpenMerchant(Player mPlayer)
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

        return itemAmounts[merchantSlot] >= amount && inventory.slots[inventorySlot].amount >= amount && IsInventorySlotMerchantable(inventorySlot);
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
        if (inventory.slots[slotIndices[0]].item.tradable )
        {
            selectedInventorySlot = slotIndices[0];
            selectedMerchantSlot = slotIndices[1];
        }
            
    }

    // drag & drop /////////////////////////////////////////////////////////////
    void OnDragAndDrop_MerchantSlot_InventorySlot(int[] slotIndices)
    {
        if(merchantPlayer != null)
        {
            if (merchantPlayer.inventory.slots[merchantPlayer.merchant.offerItems[slotIndices[0]]].item.tradable && merchantPlayer.merchant.offerItems[slotIndices[0]] != -1)
            {
                if (merchantPlayer.merchant.itemAmounts[slotIndices[0]] == 1)
                {
                    CmdBuyItem(slotIndices[0], slotIndices[1], 1);
                }
                else
                {
                    selectedBuyInventorySlot = slotIndices[1];
                    selectedBuyMerchantSlot = slotIndices[0];
                }

            }
        }
        else
        {
            RemoveItemFromMerchant(slotIndices[0]);
        }

    }
    // drag & drop /////////////////////////////////////////////////////////////

    void OnDragAndClear_MerchantSlot(int slotIndex)
    {
        if(isInMerchant != true)
        {
            RemoveItemFromMerchant(slotIndex);
        }
    }

}
