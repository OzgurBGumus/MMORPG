﻿// Note: this script has to be on an always-active UI parent, so that we can
// always react to the hotkey.
using UnityEngine;
using UnityEngine.UI;

public partial class UICharacterInfo : MonoBehaviour
{
    public KeyCode hotKey = KeyCode.T;
    public GameObject panel;
    public Text damageText;
    public Text defenseText;
    public Text healthText;
    public Text manaText;
    public Text criticalChanceText;
    public Text blockChanceText;
    public Text speedText;
    public Text levelText;
    public Text currentExperienceText;
    public Text maximumExperienceText;
    public Text skillExperienceText;
    public Text attributesText;
    public Text strengthText;
    public Text intelligenceText;
    public Button strengthButton;
    public Button intelligenceButton;


    public GameObject equipmentPanel;
    // remember default attributes header text so we can append "(remaining)"
    string attributesTextDefault;

    public static UICharacterInfo singleton;
    public UICharacterInfo()
    {
        singleton = this;
    }

    void Awake()
    {
        attributesTextDefault = attributesText.text;
    }

    void Update()
    {
        Player player = Player.localPlayer;
        if (player)
        {
            // hotkey (not while typing in chat, etc.)
            if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive())
                Toggle();

            // only refresh the panel while it's active
            if (panel.activeSelf)
            {
                //refresh all equipments
                //for (int i = 0; i < player.equipment.slots.Count; ++i)
                //{
                //    UniversalSlot slot = equipmentContent.GetChild(i).GetComponent<UniversalSlot>();
                //    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                //    ItemSlot itemSlot = player.equipment.slots[i];
                //    slot.dragAndDropable.tag = "EquipmentSlot";

                //    // set category overlay in any case. we use the last noun in the
                //    // category string, for example EquipmentWeaponBow => Bow
                //    // (disabled if no category, e.g. for archer shield slot)
                //    EquipmentInfo slotInfo = ((PlayerEquipment)player.equipment).slotInfo[i];
                //    //slot.categoryOverlay.SetActive(slotInfo.requiredCategory != ItemCategory.None);
                //    slot.categoryOverlay.SetActive(UIInventory.singleton.categories);
                //    string overlay = Utils.ParseLastNoun(slotInfo.requiredCategory.ToString());
                //    slot.categoryText.text = overlay != "" ? overlay : "?";

                //    if (itemSlot.amount > 0)
                //    {
                //        // refresh valid item

                //        // only build tooltip while it's actually shown. this
                //        // avoids MASSIVE amounts of StringBuilder allocations.
                //        slot.tooltip.enabled = true;
                //        if (slot.tooltip.IsVisible())
                //            slot.tooltip.text = itemSlot.ToolTip();
                //        slot.dragAndDropable.dragable = true;

                //        // use durability colors?
                //        if (itemSlot.item.maxDurability > 0)
                //        {
                //            if (itemSlot.item.durability == 0)
                //                slot.image.color = UIInventory.singleton.brokenDurabilityColor;
                //            else if (itemSlot.item.DurabilityPercent() < UIInventory.singleton.lowDurabilityThreshold)
                //                slot.image.color = UIInventory.singleton.lowDurabilityColor;
                //            else
                //                slot.image.color = Color.white;
                //        }
                //        else slot.image.color = Color.white; // reset for no-durability items
                //        slot.image.sprite = itemSlot.item.image;

                //        // cooldown if usable item
                //        if (itemSlot.item.data is UsableItem usable)
                //        {
                //            float cooldown = player.GetItemCooldown(usable.cooldownCategory);
                //            slot.cooldownCircle.fillAmount = usable.cooldown > 0 ? cooldown / usable.cooldown : 0;
                //        }
                //        else slot.cooldownCircle.fillAmount = 0;
                //        slot.amountOverlay.SetActive(itemSlot.amount > 1);
                //        slot.amountText.text = itemSlot.amount.ToString();
                //        slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel;
                //        slot.GetComponent<UnityEngine.UI.Image>().sprite = player.itemRarityConfig.GetSprite;
                //        slot.GetComponent<UnityEngine.UI.Image>().color = player.itemRarityConfig.GetColor(itemSlot.item);
                //    }
                //    else
                //    {
                //        // refresh invalid item
                //        slot.tooltip.enabled = false;
                //        slot.dragAndDropable.dragable = false;
                //        slot.image.color = Color.clear;
                //        slot.image.sprite = null;
                //        slot.cooldownCircle.fillAmount = 0;
                //        slot.amountOverlay.SetActive(false);
                //        slot.upgradeText.text = "";
                //        slot.GetComponent<UnityEngine.UI.Image>().sprite = player.itemRarityConfig.GetSprite;
                //        slot.GetComponent<UnityEngine.UI.Image>().color = player.itemRarityConfig.GetColorForEmptySlot();
                //    }
                //}

                damageText.text = player.combat.damage.ToString();
                defenseText.text = player.combat.defense.ToString();
                healthText.text = player.health.max.ToString();
                manaText.text = player.mana.max.ToString();
                criticalChanceText.text = (player.combat.criticalChance * 100).ToString("F0") + "%";
                blockChanceText.text = (player.combat.blockChance * 100).ToString("F0") + "%";
                speedText.text = player.speed.ToString("F1");
                levelText.text = player.level.current.ToString();
                currentExperienceText.text = player.experience.current.ToString();
                maximumExperienceText.text = player.experience.max.ToString();
                skillExperienceText.text = ((PlayerSkills)player.skills).skillExperience.ToString();
                Utils.InvokeMany(typeof(UICharacterInfo), this, "Update_", player);

                // attributes (show spendable if >1 so it's more obvious)
                // (each Attribute component has .PointsSpendable. can use any.)
                int spendable = player.strength.PointsSpendable();
                string suffix = "";
                if (spendable > 0)
                    suffix = " (" + player.strength.PointsSpendable() + ")";
                attributesText.text = attributesTextDefault + suffix;

                strengthText.text = player.strength.value.ToString();
                strengthButton.interactable = player.strength.PointsSpendable() > 0;
                strengthButton.onClick.SetListener(() => {
                    player.strength.CmdIncrease();
                });

                intelligenceText.text = player.intelligence.value.ToString();
                intelligenceButton.interactable = player.intelligence.PointsSpendable() > 0;
                intelligenceButton.onClick.SetListener(() => {
                    player.intelligence.CmdIncrease();
                });
            }
        }
        else panel.SetActive(false);
    }

    public void Toggle()
    {
        if (panel.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }

    }
    public void Open()
    {
        FindObjectOfType<Canvas>().GetComponent<UIUniqueWindow>().CloseWindows(inventory:false, skills: false, crafting: false, guild: false, party: false, gameMasterTool: false, characterInfo: false);
        panel.SetActive(true);
    }
    public void Close()
    {
        panel.SetActive(false);
    }
}
