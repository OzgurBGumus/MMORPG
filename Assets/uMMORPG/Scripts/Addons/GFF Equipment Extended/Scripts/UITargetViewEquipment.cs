using UnityEngine;
using UnityEngine.UI;

public partial class UITargetViewEquipment : MonoBehaviour
{
    [Header("Settings")]
    public UIEquipmentExtended equipmentExtended;

    [Header("UI Elements")]
    public GameObject panel;
    public UniversalSlot slotPrefab;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null && player.target != null && player.target is Player && player.target.health.current > 0)
        {
            // refresh all equipment items
            for (int i = 0; i < 16; ++i)
            {
                UniversalSlot slot = content.GetChild(i).transform.GetChild(0).GetComponent<UniversalSlot>();
                slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                ItemSlot itemSlot = player.target.equipment.slots[i];

                //view category
                if (equipmentExtended.showCategories)
                {
                    // set category overlay in any case. we use the last noun in the
                    // category string, for example EquipmentWeaponBow => Bow
                    // (disabled if no category, e.g. for archer shield slot)
                    EquipmentInfo slotInfo = ((PlayerEquipment)player.equipment).slotInfo[i];
                    slot.categoryOverlay.SetActive(slotInfo.requiredCategory != ItemCategory.None);
                    string overlay = Utils.ParseLastNoun(slotInfo.requiredCategory.ToString());
                    slot.categoryText.text = overlay != "" ? overlay : "?";
                }

                //if slot not empty
                if (itemSlot.amount > 0)
                {
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

                    if (equipmentExtended.showAmmoAmount && itemSlot.item.data is AmmoItem)
                    {
                        slot.amountOverlay.SetActive(true);
                        slot.amountText.text = itemSlot.amount.ToString();
                    }

                    slot.tooltip.enabled = true;
                    if (slot.tooltip.IsVisible())
                        slot.tooltip.text = itemSlot.ToolTip();
                }
                else
                {
                    // refresh invalid item
                    slot.tooltip.enabled = false;
                    slot.dragAndDropable.dragable = false;
                    slot.cooldownCircle.fillAmount = 0;
                    slot.amountOverlay.SetActive(false);
                    slot.categoryOverlay.SetActive(false);

                    //if the sprite array is full
                    if (equipmentExtended.showSlotBackground && equipmentExtended.SlotBackground.Length > 0 && equipmentExtended.SlotBackground[i] != null)
                    {
                        slot.image.color = Color.gray;
                        slot.image.sprite = equipmentExtended.SlotBackground[i];
                    }
                    else slot.image.sprite = null;
                }

                // addon system hooks (Item rarity and Upgrade)
                Utils.InvokeMany(typeof(UITargetViewEquipment), this, "Update_", slot, itemSlot);
            }
        }
        else panel.SetActive(false);
    }
}
