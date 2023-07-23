using UnityEngine;
using UnityEngine.UI;

public partial class UIEquipmentExtended : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode hotKey = KeyCode.E;
    public bool showCategories;
    public bool showAmmoAmount;
    public bool showSlotBackground;
    public Sprite[] SlotBackground;

    [Header("Settings : addons")]
    public bool useMenuAddon;

    [Header("UI Elements")]
    public GameObject panel;
    public UniversalSlot prefab;
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

    //singleton
    public static UIEquipmentExtended singleton;
    public UIEquipmentExtended()
    {
        singleton = this;
    }

    private void Start()
    {
    }

    void Update()
    {
        Player player = Player.localPlayer;
        if (player)
        {
            // hotkey (not while typing in chat, etc.)
            if (!useMenuAddon)
            {
                if (Input.GetKeyDown(hotKey) && !UIUtils.AnyInputActive()) panel.SetActive(!panel.activeSelf);
            }

            // only update the panel if it's active
            if (panel.activeSelf)
            {
                // refresh all equipment items
                for (int i = 0; i < 16; ++i)
                {
                    UniversalSlot slot = content.GetChild(i).transform.GetChild(0).GetComponent<UniversalSlot>();
                    slot.dragAndDropable.name = i.ToString(); // drag and drop slot
                    ItemSlot itemSlot = player.equipment.slots[i];

                    //view category
                    if (showCategories)
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
                        slot.dragAndDropable.dragable = true;
                        slot.image.sprite = itemSlot.item.image;
                        slot.image.color = Color.white;

                        if (showAmmoAmount && itemSlot.item.data is AmmoItem)
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
                        slot.dragAndDropable.dragable = false;
                        slot.amountOverlay.SetActive(false);
                        slot.categoryOverlay.SetActive(false);
                        slot.tooltip.enabled = false;

                        //if the sprite array is full
                        if (showSlotBackground && SlotBackground.Length > 0 && SlotBackground[i] != null)
                        {
                            slot.image.color = Color.gray;
                            slot.image.sprite = SlotBackground[i];
                        }
                        else slot.image.sprite = null;
                    }

                    // addon system hooks (Item rarity)
                    Utils.InvokeMany(typeof(UIEquipmentExtended), this, "Update_", player, slot, itemSlot);
                }
            }
        }
        else panel.SetActive(false);
    }
}
