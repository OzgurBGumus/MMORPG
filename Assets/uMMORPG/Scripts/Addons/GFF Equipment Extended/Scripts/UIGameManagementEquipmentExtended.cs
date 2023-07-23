using UnityEngine;
using UnityEngine.UI;

public class UIGameManagementEquipmentExtended : MonoBehaviour
{
    [Header("GFF UI Elements")]
    public GameObject panel;
    public Transform content;

    [Header("Durability Colors")]
    public Color brokenDurabilityColor = Color.red;
    public Color lowDurabilityColor = Color.magenta;
    [Range(0.01f, 0.99f)] public float lowDurabilityThreshold = 0.1f;

    [Header("For Equipment bags")]
    public GameObject panelBags;

    [Header("For Equipment stats")]
    public GameObject panelStats;
    public Text textDamageValue;
    public Text textDefenseValue;
    public GameObject panelStatsAccuracyDodge;
    public Text textAccuracyValue;
    public Text textDodgeValue;

    public static Player findPlayer;

    // Update is called once per frame
    void Update()
    {
        if (panel.activeSelf)
        {
            if (findPlayer)
            {
                // refresh all equipment items
                for (int i = 0; i < 16; ++i)
                {
                    UniversalSlot slot = content.GetChild(i).transform.GetChild(0).GetComponent<UniversalSlot>();
                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                    ItemSlot itemSlot = findPlayer.equipment.slots[i];
                    bool state = false; // for item rarity addon

                    //view category
                    if (UIEquipmentExtended.singleton.showCategories)
                    {
                        // set category overlay in any case. we use the last noun in the
                        // category string, for example EquipmentWeaponBow => Bow
                        // (disabled if no category, e.g. for archer shield slot)
                        EquipmentInfo slotInfo = ((PlayerEquipment)findPlayer.equipment).slotInfo[i];
                        slot.categoryOverlay.SetActive(slotInfo.requiredCategory != ItemCategory.None);
                        string overlay = Utils.ParseLastNoun(slotInfo.requiredCategory.ToString());
                        slot.categoryText.text = overlay != "" ? overlay : "?";
                    }

                    //if slot not empty
                    if (itemSlot.amount > 0)
                    {
                        state = true;

                        slot.dragAndDropable.dragable = false;

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

                        if (UIEquipmentExtended.singleton.showAmmoAmount && itemSlot.item.data is AmmoItem)
                        {
                            slot.amountOverlay.SetActive(true);
                            slot.amountText.text = itemSlot.amount.ToString();
                        }

                        //if used upgrade addon
                        if (itemSlot.item.data is EquipmentItem equipmentItem)
                            slot.upgradeText.text = "+" + itemSlot.item.currentUpgradeLevel.ToString();
                        else slot.upgradeText.text = "";

                        slot.tooltip.enabled = true;
                        if (slot.tooltip.IsVisible())
                            slot.tooltip.text = itemSlot.ToolTip();
                    }
                    else
                    {
                        // refresh invalid item
                        slot.dragAndDropable.dragable = false;
                        slot.amountOverlay.SetActive(false);
                        slot.categoryOverlay.SetActive(false);
                        slot.tooltip.enabled = false;

                        //if the sprite array is full
                        if (UIEquipmentExtended.singleton.showSlotBackground && UIEquipmentExtended.singleton.SlotBackground.Length > 0)
                        {
                            if (UIEquipmentExtended.singleton.SlotBackground[i] == null) slot.image.color = Color.clear;
                            else slot.image.color = Color.gray;
                            slot.image.sprite = UIEquipmentExtended.singleton.SlotBackground[i];
                        }
                        else slot.image.sprite = null;

                        slot.upgradeText.text = "";
                    }

                    // addon system hooks (Item rarity)
                    Utils.InvokeMany(typeof(UIGameManagementEquipmentExtended), this, "Update_", slot, state ? findPlayer.equipment.slots[i] : new ItemSlot());
                }
            }
            else panel.SetActive(false);
        }
    }
}
