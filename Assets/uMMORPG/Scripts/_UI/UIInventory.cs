﻿// Note: this script has to be on an always-active UI parent, so that we can
// always react to the hotkey.
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class UIInventory : MonoBehaviour
{
    public static UIInventory singleton;
    public KeyCode hotKey = KeyCode.I;
    public GameObject panel;
    public UniversalSlot slotPrefab;
    public Transform inventoryContent;
    public bool categories;
    public Text goldText;
    public UIDragAndDropable trash;
    public Image trashImage;
    public GameObject trashOverlay;
    public Text trashAmountText;

    public UIShortcuts shortcuts;

    [Header("Durability Colors")]
    public Color brokenDurabilityColor = Color.red;
    public Color lowDurabilityColor = Color.magenta;
    [Range(0.01f, 0.99f)] public float lowDurabilityThreshold = 0.1f;

    private bool lastFrameWasInactive=true;


    public UIInventory()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            // hotkey (not while typing in chat, etc.)
            if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
            {
                Toggle();
            }


            ((PlayerEquipment)player.equipment).avatarCamera.enabled = panel.activeSelf;

            // only update the panel if it's active
            if (panel.activeSelf)
            {
                FirstActiveFrame();
                // instantiate/destroy enough slots
                UIUtils.BalancePrefabs(slotPrefab.gameObject, player.inventory.slots.Count, inventoryContent);


                // refresh all items
                for (int i = 0; i < player.inventory.slots.Count; ++i)
                {
                    UniversalSlot slot = inventoryContent.GetChild(i).GetComponent<UniversalSlot>();
                    slot.dragAndDropable.name = i.ToString(); // drag and drop index
                    ItemSlot itemSlot = player.inventory.slots[i];

                    if (itemSlot.amount > 0)
                    {
                        // refresh valid item
                        int icopy = i; // needed for lambdas, otherwise i is Count
                        slot.button.onClick.SetListener(() =>
                        {
                            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                            {
                                string color = "#" + ColorUtility.ToHtmlStringRGBA(player.itemRarityConfig.GetColor(itemSlot.item));
                                string param1 = "item";
                                //SHOULD CREATE THE JSON OF THE ITEM'S CURRENT DATA AND PUT THAT JSON TO LINK WITH param2
                                string param2 = EncodeToBase64(itemSlot.ToolTip());
                                string htmlElement = "[<color=" + color + "><link=" + param1 + ":" + param2 + ">" + itemSlot.item.name + "</link></color>]";
                                UIChat.singleton.AppendText(htmlElement);
                            }
                            else if (itemSlot.item.data is UsableItem usable &&
                                usable.CanUse(player, icopy))
                            {
                                player.inventory.CmdUseItem(icopy);
                            }
                            else
                            {
                                if (UIUpgrade.singleton.panel.activeSelf)
                                {
                                    UIUpgrade.singleton.OnInventoryItemClick(player, itemSlot, icopy);
                                }
                                else if (UIMerchant.singleton.panel.activeSelf)
                                {
                                    UIMerchant.singleton.OnInventoryItemClick(player, itemSlot, icopy);
                                }
                                //else if (UICrafting.singleton.panel.activeSelf)
                                //{
                                //    UICrafting.singleton.OnInventoryItemClick(player, itemSlot, icopy);
                                //}
                            }

                        });
                        // only build tooltip while it's actually shown. this
                        // avoids MASSIVE amounts of StringBuilder allocations.
                        slot.tooltip.enabled = true;
                        if (slot.tooltip.IsVisible())
                            slot.tooltip.text = itemSlot.ToolTip();
                        slot.dragAndDropable.dragable = true;

                        // use durability colors?
                        if (itemSlot.item.maxDurability > 0)
                        {
                            if (itemSlot.item.durability == 0)
                                slot.image.color = brokenDurabilityColor;
                            else if (itemSlot.item.DurabilityPercent() < lowDurabilityThreshold)
                                slot.image.color = lowDurabilityColor;
                            else
                                slot.image.color = Color.white;
                        }
                        else slot.image.color = Color.white; // reset for no-durability items
                        slot.image.sprite = itemSlot.item.image;

                        // cooldown if usable item
                        if (itemSlot.item.data is UsableItem usable2)
                        {
                            float cooldown = player.GetItemCooldown(usable2.cooldownCategory);
                            slot.cooldownCircle.fillAmount = usable2.cooldown > 0 ? cooldown / usable2.cooldown : 0;
                        }
                        else slot.cooldownCircle.fillAmount = 0;
                        slot.amountOverlay.SetActive(itemSlot.amount > 1);
                        slot.amountText.text = itemSlot.amount.ToString();
                        slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel;
                        slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                        slot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(itemSlot.item);
                    }
                    else
                    {
                        // refresh invalid item
                        slot.button.onClick.RemoveAllListeners();
                        slot.tooltip.enabled = false;
                        slot.dragAndDropable.dragable = false;
                        slot.image.color = Color.clear;
                        slot.image.sprite = null;
                        slot.cooldownCircle.fillAmount = 0;
                        slot.amountOverlay.SetActive(false);
                        slot.upgradeText.text = "";
                        slot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                        slot.GetComponent<Image>().color = player.itemRarityConfig.GetColorForEmptySlot();
                    }
                }



                // gold
                goldText.text = player.gold.ToString();

                // trash (tooltip always enabled, dropable always true)
                trash.dragable = player.inventory.trash.amount > 0;
                if (player.inventory.trash.amount > 0)
                {
                    // refresh valid item
                    if (player.inventory.trash.item.maxDurability > 0)
                    {
                        if (player.inventory.trash.item.durability == 0)
                            trashImage.color = brokenDurabilityColor;
                        else if (player.inventory.trash.item.DurabilityPercent() < lowDurabilityThreshold)
                            trashImage.color = lowDurabilityColor;
                        else
                            trashImage.color = Color.white;
                    }
                    else trashImage.color = Color.white; // reset for no-durability items
                    trashImage.sprite = player.inventory.trash.item.image;

                    trashOverlay.SetActive(player.inventory.trash.amount > 1);
                    trashAmountText.text = player.inventory.trash.amount.ToString();
                }
                else
                {
                    // refresh invalid item
                    trashImage.color = Color.clear;
                    trashImage.sprite = null;
                    trashOverlay.SetActive(false);
                }
            }
        }
        else {
            FirstInActiveFrame();
        }

    }

    public void Toggle()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }

    }
    
    public void Open()
    {
        FindObjectOfType<Canvas>().GetComponent<UIUniqueWindow>().CloseWindows(merchant: false, skills: false, crafting: false, guild: false, playerTrading: false, party: false, gameMasterTool: false, npcTrading: false, characterInfo: false, upgrade:false);
        panel.SetActive(true);
    }
    
    public void Close()
    {
        panel.SetActive(false);
    }

    private void FirstActiveFrame()
    {
        if (lastFrameWasInactive)
        {
            Open();
            lastFrameWasInactive = false;
        }
    }
    private void FirstInActiveFrame()
    {
        if (!lastFrameWasInactive)
        {
            Close();
            lastFrameWasInactive = true;
        }
    }

    public string EncodeToBase64(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }
}
