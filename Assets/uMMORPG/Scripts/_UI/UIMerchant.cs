using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMerchant : MonoBehaviour
{
    public static UIMerchant singleton;

    public GameObject panel;



    public UIPlayerMerchantSlot slotPrefab;

    public Transform content;

    public Button createSellMerchantButton;
    public Button endMerchantButton;

    public UIShortcuts shortcuts;

    public InputField title;

    /// <summary>
    /// /Add Item Panel Params
    /// </summary>
    public GameObject addItemPanel;
    public Button addItemToMerchantButton;
    public Button cancelAddItem;
    public InputField perPrice;
    public InputField amount;

    /// <summary>
    /// Buy Item Panel Params
    /// </summary>
    public GameObject buyItemPanel;
    public Button buyItemButton;
    public Button cancelBuyItem;
    public InputField buyAmount;
    public Text totalPrice;


    private List<int> offerItems = new List<int>();
    private List<int> itemPrices = new List<int>();
    private List<int> itemAmounts = new List<int>();
    public UIMerchant()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    private void Start()
    {
        for (int i = 0; i < 16; ++i)
        {
            offerItems.Add(-1);
            itemPrices.Add(-1);
            itemAmounts.Add(-1);
        }
    }
    // Update is called once per frame
    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            if (player.merchant.merchantPlayer != null)
            {
                if(!panel.activeSelf) panel.SetActive(true);
                if(!shortcuts.inventoryPanel.activeSelf)shortcuts.inventoryPanel.SetActive(true);
                
            }
            if (panel.activeSelf)
            {
                //Check is player still allowed to stay in Merchant tab.
                if ((player.state != "IDLE" && player.state != "MERCHANT" && player.state != "MOVING") &&
                    (player.merchant.merchantPlayer == null || player.merchant.merchantPlayer.state != "MERCHANT")
                    )
                {
                    panel.SetActive(false);
                    OnWindowClose();
                    return;
                }

                //Player opens someone else's merchant
                if (player.merchant.merchantPlayer != null && player.state != "MERCHANT" &&
                player.merchant.merchantPlayer.state == "MERCHANT")
                {
                    //SET TITLE
                    if (title.interactable) title.interactable = false;
                    if (!title.text.Equals(player.merchant.merchantPlayer.merchant.title)) title.text = player.merchant.merchantPlayer.merchant.title;

                    //CHECK IS AN ITEM ALREADY SELECTED OR NOT
                    if (player.merchant.selectedBuyMerchantSlot != -1)
                    {
                        buyItemPanel.SetActive(true);
                    }
                    else
                    {
                        if (buyItemPanel.activeSelf)
                        {
                            ClearBuyTab();
                            buyItemPanel.SetActive(false);
                        }
                        
                    }
                    //SET CREATE AND END BUTTONS TO INACTIVE SINCE THIS IS AN ANOTHER PLAYER'S MERCHANT
                    createSellMerchantButton.gameObject.SetActive(false);
                    endMerchantButton.gameObject.SetActive(false);


                    UIUtils.BalancePrefabs(slotPrefab.gameObject, player.merchant.offerItems.Count, content);
                    for (int i = 0; i < player.merchant.merchantPlayer.merchant.offerItems.Count; i++)
                    {
                        UIPlayerMerchantSlot slot = content.GetChild(i).GetComponent<UIPlayerMerchantSlot>();
                        slot.dragAndDropable.name = i.ToString();
                        int inventoryIndex = player.merchant.merchantPlayer.merchant.offerItems[i];

                        if (0 <= inventoryIndex && inventoryIndex < player.merchant.merchantPlayer.inventory.slots.Count &&
                        player.merchant.merchantPlayer.inventory.slots[inventoryIndex].amount > 0)
                        {
                            ItemSlot itemSlot = player.merchant.merchantPlayer.inventory.slots[inventoryIndex];

                            // refresh valid item

                            // only build tooltip while it's actually shown. this
                            // avoids MASSIVE amounts of StringBuilder allocations.
                            slot.tooltip.enabled = true;

                            if (slot.tooltip.IsVisible())
                                slot.tooltip.text = itemSlot.ToolTip(player.merchant.merchantPlayer.merchant.itemPrices[i]);

                            slot.image.color = Color.white;
                            slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                            slot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(itemSlot.item);
                            slot.image.sprite = itemSlot.item.image;
                            slot.amountOverlay.SetActive(player.merchant.merchantPlayer.merchant.itemAmounts[i] > 1 && itemSlot.amount > 1);
                            slot.amountText.text = player.merchant.merchantPlayer.merchant.itemAmounts[i].ToString();

                        }
                        else
                        {
                            slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                            slot.GetComponent<Image>().color = Color.gray;
                            // refresh invalid item
                            slot.image.color = Color.clear;
                            slot.tooltip.enabled = false;
                            slot.image.sprite = null;
                            slot.amountOverlay.SetActive(false);
                        }
                    }
                }

                //player open own merchant
                else
                {
                    addItemPanel.SetActive(player.merchant.selectedInventorySlot >= 0);
                    shortcuts.CloseAllPanels(exceptMerchant: true, exceptInventory: true);
                    if (player.state != "MERCHANT")
                    {
                        if (!title.interactable) title.interactable = true;
                        createSellMerchantButton.gameObject.SetActive(true);
                        endMerchantButton.gameObject.SetActive(false);
                        if (player.merchant.removeFromMerchantSlot >= 0)
                        {
                            offerItems[player.merchant.removeFromMerchantSlot] = -1;
                            itemAmounts[player.merchant.removeFromMerchantSlot] = -1;
                            itemPrices[player.merchant.removeFromMerchantSlot] = -1;
                            player.merchant.removeFromMerchantSlot = -1;
                        }
                    }
                    else
                    {
                        if (title.interactable) title.interactable = false;
                        createSellMerchantButton.gameObject.SetActive(false);
                        endMerchantButton.gameObject.SetActive(true);
                        for (var i = 0; i < offerItems.Count; i++)
                        {
                            offerItems[i] = player.merchant.offerItems[i];
                            itemAmounts[i] = player.merchant.itemAmounts[i];
                        }
                    }
                    UIUtils.BalancePrefabs(slotPrefab.gameObject, offerItems.Count, content);
                    for (int i = 0; i < offerItems.Count; i++)
                    {
                        UIPlayerMerchantSlot slot = content.GetChild(i).GetComponent<UIPlayerMerchantSlot>();
                        slot.dragAndDropable.name = i.ToString();
                        int inventoryIndex = offerItems[i];

                        if (0 <= inventoryIndex && inventoryIndex < player.inventory.slots.Count &&
                        player.inventory.slots[inventoryIndex].amount > 0)
                        {
                            ItemSlot itemSlot = player.inventory.slots[inventoryIndex];

                            // refresh valid item

                            // only build tooltip while it's actually shown. this
                            // avoids MASSIVE amounts of StringBuilder allocations.
                            slot.tooltip.enabled = true;

                            if (slot.tooltip.IsVisible())
                                slot.tooltip.text = itemSlot.ToolTip(itemPrices[i]);

                            slot.image.color = Color.white;
                            slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                            slot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(itemSlot.item);
                            slot.image.sprite = itemSlot.item.image;
                            slot.amountOverlay.SetActive(itemAmounts[i] > 1 && itemSlot.amount > 1);
                            slot.amountText.text = itemAmounts[i].ToString();
                        }
                        else
                        {
                            slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                            slot.GetComponent<Image>().color = Color.gray;
                            // refresh invalid item
                            slot.tooltip.enabled = false;
                            slot.image.sprite = null;
                            slot.image.color = Color.clear;
                            slot.amountOverlay.SetActive(false);
                        }
                    }

                }
                listenButtons(player);
            }
            else
            {
                if (player.merchant.merchantPlayer != null)
                {
                    player.merchant.merchantPlayer = null;
                }
            }

        }
    }

    void listenButtons(Player player)
    {
        createSellMerchantButton.interactable = player.health.current > 0 && player.state == "IDLE";
        createSellMerchantButton.onClick.SetListener(() =>
        {
            if (player.merchant.CanStartMerchant())
            {
                foreach (int i in offerItems)
                {
                    if (i >= 0) {
                        player.merchant.isInMerchant = true;
                        player.merchant.CmdStartMerchant(offerItems, itemPrices, itemAmounts, title.text);
                        break;
                    }
                }

            }
        });
        endMerchantButton.onClick.SetListener(() =>
        {
            if (player.merchant.isInMerchant)
            {
                player.merchant.EndMerchant();
            }
        });
        if (player.merchant.selectedInventorySlot >= 0)
        {
            amount.interactable = player.inventory.slots[player.merchant.selectedInventorySlot].item.maxStack > 1;
            addItemToMerchantButton.interactable = !string.IsNullOrEmpty(perPrice.text) && !string.IsNullOrEmpty(amount.text) && perPrice.text.ToInt() > 0 && amount.text.ToInt() > 0;
            addItemToMerchantButton.onClick.SetListener(() =>
            {
                if (
                    player.state == "IDLE" &&
                    player.merchant.selectedMerchantSlot >= 0 && player.merchant.selectedMerchantSlot < offerItems.Count &&
                    !offerItems.Contains(player.merchant.selectedInventorySlot)

                )
                {
                    ItemSlot slot = player.inventory.slots[player.merchant.selectedInventorySlot];
                    if (slot.item.tradable &&
                        !slot.item.summoned &&
                        slot.amount > 0 && slot.amount >= amount.text.ToInt())
                    {
                        offerItems[player.merchant.selectedMerchantSlot] = player.merchant.selectedInventorySlot;
                        itemPrices[player.merchant.selectedMerchantSlot] = perPrice.text.ToInt();
                        itemAmounts[player.merchant.selectedMerchantSlot] = amount.text.ToInt();
                    }
                }
                player.merchant.selectedInventorySlot = -1;
                player.merchant.selectedMerchantSlot = -1;
                clearAddItemFields();
            });
            cancelAddItem.onClick.SetListener(() =>
            {
                player.merchant.selectedInventorySlot = -1;
                player.merchant.selectedMerchantSlot = -1;
                clearAddItemFields();
            });
        }
        if (player.merchant.selectedBuyMerchantSlot >= 0)
        {
            totalPrice.text = (buyAmount.text.ToInt() * player.merchant.merchantPlayer.merchant.itemPrices[player.merchant.selectedBuyMerchantSlot]).ToString();

            buyItemButton.interactable = !string.IsNullOrEmpty(buyAmount.text) && buyAmount.text.ToInt() > 0 &&
                buyAmount.text.ToInt() <= player.merchant.merchantPlayer.merchant.itemAmounts[player.merchant.selectedBuyMerchantSlot] &&
                player.gold >= totalPrice.text.ToInt();
            buyItemButton.onClick.SetListener(() =>
            {
                player.merchant.CmdBuyItem(player.merchant.selectedBuyMerchantSlot, player.merchant.selectedBuyInventorySlot, buyAmount.text.ToInt());
                player.merchant.selectedBuyMerchantSlot = -1;
                player.merchant.selectedBuyInventorySlot = -1;

            });
            cancelBuyItem.onClick.SetListener(() =>
            {
                player.merchant.selectedBuyInventorySlot = -1;
                player.merchant.selectedBuyMerchantSlot = -1;
            });

        }
    }
    void clearAddItemFields()
    {
        perPrice.text = "";
        amount.text = "";
    }
    void clearBuyItemFields()
    {
        buyAmount.text = "";
    }

    public void ClearAll()
    {
        Player.localPlayer.merchant.merchantPlayer = null;
        Player.localPlayer.merchant.selectedInventorySlot = -1;
        Player.localPlayer.merchant.selectedMerchantSlot = -1;
        Player.localPlayer.merchant.selectedBuyMerchantSlot = -1;
        Player.localPlayer.merchant.selectedBuyInventorySlot = -1;
        clearAddItemFields();
        clearBuyItemFields();

    }
    void ClearBuyTab()
    {
        Player.localPlayer.merchant.selectedBuyMerchantSlot = -1;
        Player.localPlayer.merchant.selectedBuyInventorySlot = -1;
        totalPrice.text = "";
        buyAmount.text = "";
    }
    void ClearAddTab()
    {
        Player.localPlayer.merchant.selectedInventorySlot = -1;
        Player.localPlayer.merchant.selectedMerchantSlot = -1;
        amount.text = "";
        perPrice.text = "";
    }
    void OnWindowClose()
    {
        Player.localPlayer.merchant.merchantPlayer = null;
        Player.localPlayer.merchant.removeFromMerchantSlot = -1;
        title.text = "";
        ClearBuyTab();
        if (buyItemPanel.activeSelf)buyItemPanel.SetActive(false);

        ClearAddTab();
        if (addItemPanel.activeSelf) addItemPanel.SetActive(false);

        Player.localPlayer.merchant.Cleanup();



    }
}
