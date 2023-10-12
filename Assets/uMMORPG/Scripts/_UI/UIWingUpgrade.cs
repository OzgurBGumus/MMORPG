using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public partial class UIWingUpgrade : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text currentWingNameText;
    public TMP_Text nextWingNameText;
    public TMP_Text additionalCount;
    public UniversalSlot currentWingSlot;
    public UniversalSlot nextWingSlot;
    public UniversalSlot materialSlot;
    public Image additionalLuckBar;
    public Button upgradeButton;
    public static UIWingUpgrade singleton;
    private bool lastFrameWasInactive = true;

    public UIWingUpgrade()
    {
        if(singleton == null)
            singleton = this;
    }

    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            // only update the panel if it's active
            if (panel.activeSelf)
            {
                FirstActiveFrame();

                SetSlots(player);

                upgradeButton.interactable = selectedInventoryWingSlot > -1;
                upgradeButton.onClick.SetListener(() =>
                {
                    if (
                          )
                    {
                        ItemSlot wingSlot = player.inventory.slots[selectedInventoryWingSlot];
                        ItemSlot materialSlot = player.inventory.slots[selectedInventoryMaterialSlot];

                        if (wingSlot.item.data is WingItem wingItem && materialSlot.item.data.Equals(wingItem.material))
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
            }
        }
        else FirstInActiveFrame();
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
        Player.localPlayer.inventory.ItemUsingBlocked = true;
        FindObjectOfType<Canvas>().GetComponent<UIUniqueWindow>().CloseWindows();
        panel.SetActive(true);
    }
    public void Close()
    {
        Player.localPlayer.inventory.ItemUsingBlocked = false;
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
    private void SetSlots(Player player)
    {
        if (player.equipment.slots[14].amount > 0)
        {
            WingItem currentWing = (WingItem)player.equipment.slots[14].item.data;
            currentWingNameText.text = currentWing.name;
            nextWingNameText.text = currentWing.nextWing.name;

            currentWingSlot.tooltip.enabled = true;
            if (currentWingSlot.tooltip.IsVisible())
                currentWingSlot.tooltip.text = currentWing.ToolTip();
            currentWingSlot.dragAndDropable.dragable = false;
            currentWingSlot.image.color = Color.white; // reset for no-durability items
            currentWingSlot.image.sprite = currentWing.image;
            currentWingSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
            currentWingSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(currentWing);

            nextWingSlot.tooltip.enabled = true;
            if (nextWingSlot.tooltip.IsVisible())
                nextWingSlot.tooltip.text = currentWing.nextWing.ToolTip();
            nextWingSlot.dragAndDropable.dragable = false;
            nextWingSlot.image.color = Color.white; // reset for no-durability items
            nextWingSlot.image.sprite = currentWing.nextWing.image;
            nextWingSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
            nextWingSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(currentWing.nextWing);

            materialSlot.tooltip.enabled = true;
            if (materialSlot.tooltip.IsVisible())
                materialSlot.tooltip.text = currentWing.material.ToolTip();
            materialSlot.dragAndDropable.dragable = false;
            materialSlot.image.color = Color.white; // reset for no-durability items
            materialSlot.image.sprite = currentWing.material.image;
            materialSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
            materialSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(currentWing.material);

            materialSlot.amountOverlay.SetActive(true);
            materialSlot.amountText.text = currentWing.materialAmount.ToString();


        }
    }

    private bool canUpgrade(Player player)
    {
        if (player.equipment.slots[14].amount > 0)
        {
            WingItem currentWing = (WingItem)player.equipment.slots[14].item.data;
        }
        else
        {
            return false;
        }
            return (player.state == "IDLE" || player.state == "MOVING" || player.state == "CASTING" || player.state == "STUNNED") 
            && player.equipment.slots[14].amount > 0 
            && 
    }
}
