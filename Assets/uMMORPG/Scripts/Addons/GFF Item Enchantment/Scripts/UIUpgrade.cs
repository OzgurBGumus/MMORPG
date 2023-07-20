using GFFAddons;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public partial class UIUpgrade : MonoBehaviour
{
    public Sprite spriteRuneNull;

    [Header("Settings : Info")]
    public float messageDisplayTime;
    double messageDisplayTimeEnd;

    [Header("UI Elements")]
    public GameObject panel;
    public Transform content;
    public Button buttonUpgrade;

    public Text textUpgradeChanceValue;
    public Text textUpgradeChance;
    public Transform contentForUpgradeIND;
    public GameObject prefabRuneImage;

    [Header("UI Elements: Info")]
    public GameObject panelInfo;
    public Text textInfo;
    public Text textInfoMessage;

    [Header("UI Elements: panel successfully enchanted")]
    public UpdatedDisableAfter panelSuccessfullyEnchanted;
    public Text textSuccessfullyEnchanted;


    public Sprite UpgradeIndImage;
    //singleton
    public static UIUpgrade singleton;

    private bool lastFrameIsFalse = false;
    public UIUpgrade()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        //if (singleton == null)
        singleton = this;
    }

    public void Show()
    {
        FindObjectOfType<Canvas>().GetComponent<UIUniqueWindow>().CloseWindows();
        UIInventory.singleton.Open();
        panel.SetActive(true);
    }

    public void Сlose()
    {
        panel.SetActive(false);
        Player player = Player.localPlayer;
        player.upgrade.ClearUpgradeIndices();

        //null all prefabs
        for (int i = 0; i < content.childCount; ++i)
        {
            UniversalSlot slot = content.GetChild(i).GetChild(0).GetComponent<UniversalSlot>();

            // refresh invalid item
            slot.button.onClick.RemoveAllListeners();
            slot.tooltip.enabled = false;
            slot.dragAndDropable.dragable = false;
            slot.image.color = Color.clear;
            slot.image.sprite = null;
            slot.amountOverlay.SetActive(false);
            slot.upgradeText.text = "";

            //if used ItemRarity Addon
            //if (useRarityAddon) slot.GetComponent<Image>().color = player.itemRarityConfig.GetColorForEmptySlot();
        }

        //null runes images
        foreach (Transform child in contentForUpgradeIND.transform) child.gameObject.SetActive(false);
    }

    private void FirstActiveFrame()
    {
        if (lastFrameIsFalse)
        {
            Show();
            lastFrameIsFalse = false;
        }
    }
    private void FirstInActiveFrame()
    {
        if (!lastFrameIsFalse)
        {
            Сlose();
            lastFrameIsFalse = true;
        }
    }

    void Update()
    {
        // only update the panel if it's active
        if (panel.activeSelf)
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                if (player.upgrade.data.AllowedToShowPanel(player))
                {
                    FirstActiveFrame();
                    // refresh all items
                    for (int i = 0; i < player.upgrade.upgradeIndices.Count; ++i)
                    {
                        UniversalSlot slot = content.GetChild(i).GetChild(0).GetComponent<UniversalSlot>();
                        bool state = false; // for item rarity addon

                        if (player.upgrade.upgradeIndices[i] != -1 && player.upgrade.upgradeIndices[i] < player.inventory.slots.Count && player.inventory.slots[player.upgrade.upgradeIndices[i]].amount > 0)
                        {
                            state = true;

                            ItemSlot TempSlot = player.inventory.slots[player.upgrade.upgradeIndices[i]];

                            slot.button.onClick.SetListener(() => { player.upgrade.ClearUpgradeIndex(int.Parse(slot.dragAndDropable.name)); });

                            slot.tooltip.enabled = true;
                            slot.tooltip.text = TempSlot.ToolTip();
                            slot.dragAndDropable.dragable = true;
                            slot.image.color = Color.white;
                            slot.image.sprite = TempSlot.item.image;
                            slot.amountOverlay.SetActive(TempSlot.amount > 1);
                            slot.amountText.text = TempSlot.amount.ToString();

                            slot.upgradeText.text = "+" + TempSlot.item.currentUpgradeLevel;
                        }
                        else
                        {
                            // refresh invalid item
                            slot.button.onClick.RemoveAllListeners();
                            slot.tooltip.enabled = false;
                            slot.dragAndDropable.dragable = false;
                            slot.image.color = Color.clear;
                            slot.image.sprite = null;
                            slot.amountOverlay.SetActive(false);
                            slot.upgradeText.text = "";
                        }

                        // addon system hooks (Item rarity)
                        Utils.InvokeMany(typeof(UIUpgrade), this, "Update_", player, slot, state ? player.inventory.slots[player.upgrade.upgradeIndices[i]] : new ItemSlot());
                    }

                    //paint Item rune
                    ShowRunesImages(player);

                    buttonUpgrade.interactable = player.upgrade.UpgradeTimeRemaining() == 0;
                    buttonUpgrade.onClick.SetListener(() =>
                    {
                        player.upgrade.CmdStartEnchantment(player.upgrade.upgradeIndices);
                    });

                    //message time end
                    if (panelInfo.activeSelf && messageDisplayTimeEnd <= NetworkTime.time) panelInfo.SetActive(false);
                }
                else Сlose();
            }
            else panel.SetActive(false);
        }
        else
        {
            FirstInActiveFrame();
        }
    }

    private void ShowRunesImages(Player player)
    {
        //if enchantment item is not null
        if (player.upgrade.upgradeIndices[0] != -1 && player.inventory.slots[player.upgrade.upgradeIndices[0]].amount > 0)
        {
            ItemSlot slot0 = player.inventory.slots[player.upgrade.upgradeIndices[0]];

            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefabRuneImage, slot0.item.data.upgradeBonusesPercent.Length-1, contentForUpgradeIND);

            //paint runes images
            if (slot0.item.currentUpgradeLevel != 0)
            {
                for (int i = 0; i < contentForUpgradeIND.childCount; i++)
                {
                    if (i+1 < slot0.item.currentUpgradeLevel)
                    {
                        contentForUpgradeIND.GetChild(i).GetComponent<Image>().sprite = UpgradeIndImage;
                        contentForUpgradeIND.GetChild(i).GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        contentForUpgradeIND.GetChild(i).GetComponent<Image>().sprite = spriteRuneNull;
                        contentForUpgradeIND.GetChild(i).GetComponent<Image>().color = Color.gray;
                    }
                }
                
                ////show upgrade info
                //if (player.enchantment.upgradeIndices[1] != -1)
                //{

                //    //in the list of all runes we find the index of the rune we want to enchant
                //    int amount = slot0.item.amountRunes(newRune.runeType);
                //    if (amount > 0)
                //    {
                //        //current
                //        textUpgradeCurrent.text = "+" + newRune.effect[amount - 1] + " " + newRune.textInfo;

                //        //next
                //        if (slot0.item.upgradeInd.Length < contentForUpgradeIND.childCount)
                //            textUpgradeNext.text = "+" + newRune.effect[amount] + " " + newRune.textInfo;
                //        else textUpgradeNext.text = "Max";
                //    }
                //}
                //else
                //{
                //    textUpgradeCurrent.text = "";
                //    textUpgradeNext.text = "";
                //}
            }
            else
            {
                for (int i = 0; i < contentForUpgradeIND.childCount; i++)
                {
                    contentForUpgradeIND.GetChild(i).GetComponent<Image>().sprite = spriteRuneNull;
                    contentForUpgradeIND.GetChild(i).GetComponent<Image>().color = Color.gray;
                }
            }
            float upgradeChance = player.upgrade.CalculateUpgradeBonuses();
        }
        else
        {
            // instantiate/destroy enough slots
            UIUtils.BalancePrefabs(prefabRuneImage, 0, contentForUpgradeIND);
        }
    }

    //info panel
    public void ItemUpgradeError(string message)
    {
        textInfoMessage.text = message;
        panelInfo.SetActive(true);
        messageDisplayTimeEnd = NetworkTime.time + messageDisplayTime;
    }
    public void ItemInfoUpgradeSuccess(string player, string itemname, int amount)
    {
        textSuccessfullyEnchanted.text = player + " Successfully enchanted a " + itemname + " for " + amount;
        panelSuccessfullyEnchanted.Show();
    }
}
