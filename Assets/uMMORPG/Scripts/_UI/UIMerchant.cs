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

    public Button CreateSellMerchantButton;
    public Button EndMerchantButton;

    public UIShortcuts shortcuts;
    public UIMerchant()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            //Check is player still allowed to stay in Merchant tab.
            if (panel.activeSelf && player.state != "IDLE" && player.state != "MOVING")
            {
                panel.SetActive(false);
                return;
            }
            
            //Player opens someone else's merchant
            if (player.state != "MERCHANT" &&
            player.target != null &&
            player.target is Player merchantPlayer &&
            merchantPlayer.state == "MERCHANT" &&
            Input.GetMouseButtonUp(0))
            {
                panel.SetActive(true);

                CreateSellMerchantButton.gameObject.SetActive(false);
                EndMerchantButton.gameObject.SetActive(false);
                UIUtils.BalancePrefabs(slotPrefab.gameObject, player.merchant.offerItems.Count, content);
                for(int i=0; i<merchantPlayer.merchant.offerItems.Count; i++)
                {
                    UIPlayerTradingSlot slot = content.GetChild(i).GetComponent<UIPlayerTradingSlot>();
                    slot.dragAndDropable.name = i.ToString();
                    int inventoryIndex = merchantPlayer.merchant.offerItems[i];

                    if (0 <= inventoryIndex && inventoryIndex < merchantPlayer.inventory.slots.Count &&
                    merchantPlayer.inventory.slots[inventoryIndex].amount > 0)
                    {
                        ItemSlot itemSlot = merchantPlayer.inventory.slots[inventoryIndex];

                        // refresh valid item

                        // only build tooltip while it's actually shown. this
                        // avoids MASSIVE amounts of StringBuilder allocations.
                        slot.tooltip.enabled = true;

                        if (slot.tooltip.IsVisible())
                            slot.tooltip.text = itemSlot.ToolTip();

                        slot.image.color = Color.white;
                        slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                        slot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(itemSlot.item);
                        slot.image.sprite = itemSlot.item.image;
                        slot.amountOverlay.SetActive(merchantPlayer.merchant.itemAmounts[i] > 1 && itemSlot.amount > 1);
                        slot.amountText.text = merchantPlayer.merchant.itemAmounts[i].ToString();
                        
                    }
                    else
                    {
                        // refresh invalid item
                        slot.tooltip.enabled = false;
                        slot.image.sprite = null;
                        slot.amountOverlay.SetActive(false);
                    }
                }


                listenButtons(player);
            }

            //player open own merchant
            else if (panel.activeSelf)
            {
                shortcuts.CloseAllPanels(exceptMerchant: true);
                if (player.state != "MERCHANT")
                {
                    CreateSellMerchantButton.gameObject.SetActive(true);
                    EndMerchantButton.gameObject.SetActive(false);
                }
                else
                {
                    CreateSellMerchantButton.gameObject.SetActive(false);
                    EndMerchantButton.gameObject.SetActive(true);
                }

                UIUtils.BalancePrefabs(slotPrefab.gameObject, player.merchant.offerItems.Count, content);
                for (int i = 0; i < player.merchant.offerItems.Count; i++)
                {
                    UIPlayerMerchantSlot slot = content.GetChild(i).GetComponent<UIPlayerMerchantSlot>();
                    slot.dragAndDropable.name = i.ToString();
                    int inventoryIndex = player.merchant.offerItems[i];

                    if (0 <= inventoryIndex && inventoryIndex < player.inventory.slots.Count &&
                    player.inventory.slots[inventoryIndex].amount > 0)
                    {
                        ItemSlot itemSlot = player.inventory.slots[inventoryIndex];

                        // refresh valid item

                        // only build tooltip while it's actually shown. this
                        // avoids MASSIVE amounts of StringBuilder allocations.
                        slot.tooltip.enabled = true;

                        if (slot.tooltip.IsVisible())
                            slot.tooltip.text = itemSlot.ToolTip();

                        slot.image.color = Color.white;
                        slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                        slot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(itemSlot.item);
                        slot.image.sprite = itemSlot.item.image;
                        slot.amountOverlay.SetActive(player.merchant.itemAmounts[i] > 1 && itemSlot.amount > 1);
                        slot.amountText.text = player.merchant.itemAmounts[i].ToString();
                    }
                    else
                    {
                        // refresh invalid item
                        slot.tooltip.enabled = false;
                        slot.image.sprite = null;
                        slot.amountOverlay.SetActive(false);
                    }
                }
                listenButtons(player);
            }
        }
    }

    void listenButtons(Player player)
    {
        CreateSellMerchantButton.interactable = player.health.current > 0 && player.state == "IDLE";
        CreateSellMerchantButton.onClick.SetListener(() =>
        {
            if (player.merchant.CanStartMerchant())
            {
                player.merchant.isInMerchant = true;
                player.merchant.CmdStartMerchant(player.merchant.offerItems.ToArray(), player.merchant.itemAmounts.ToArray(), player.merchant.itemPrices.ToArray());
            }
        });
        EndMerchantButton.onClick.SetListener(() =>
        {
            if (player.merchant.isInMerchant)
            {
                player.merchant.EndMerchant();
            }
        });
    }
}
