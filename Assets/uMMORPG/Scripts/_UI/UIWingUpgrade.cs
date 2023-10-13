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

                SetLuck(player);
                SetSlots(player);

                upgradeButton.interactable = player.upgrade.CanUpgradeWing(player);
                upgradeButton.onClick.SetListener(() =>
                {
                    if (player.upgrade.CanUpgradeWing(player))
                    {
                        player.upgrade.CmdStartWingUpgrade(player);

                        
                    }
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
            

            currentWingSlot.tooltip.enabled = true;
            if (currentWingSlot.tooltip.IsVisible())
                currentWingSlot.tooltip.text = currentWing.ToolTip();
            currentWingSlot.dragAndDropable.dragable = false;
            currentWingSlot.image.color = Color.white; // reset for no-durability items
            currentWingSlot.image.sprite = currentWing.image;
            currentWingSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
            currentWingSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(currentWing);

            if(currentWing.nextWing != null)
            {
                nextWingNameText.text = currentWing.nextWing.name;

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
                    materialSlot.tooltip.text = currentWing.wingUpgradeMaterial.ToolTip();
                materialSlot.dragAndDropable.dragable = false;
                materialSlot.image.color = Color.white; // reset for no-durability items
                materialSlot.image.sprite = currentWing.wingUpgradeMaterial.image;
                materialSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
                materialSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColor(currentWing.wingUpgradeMaterial);

                materialSlot.amountOverlay.SetActive(true);
                materialSlot.amountText.text = currentWing.wingUpgradeMaterialAmount.ToString();
            }

        }
        else
        {
            ResetSlots(player);
        }
    }
    private void SetLuck(Player player)
    {
        additionalCount.text = "x"+player.upgrade.wingUpgradePoint.ToString();
        additionalLuckBar.fillAmount = player.upgrade.wingUpgradePointProgress/100f;
    }
    private void ResetSlots(Player player)
    {
        currentWingNameText.text ="";
        nextWingNameText.text = "";
        materialSlot.amountText.text = "";


        // refresh invalid item
        currentWingSlot.button.onClick.RemoveAllListeners();
        currentWingSlot.tooltip.enabled = false;
        currentWingSlot.dragAndDropable.dragable = false;
        currentWingSlot.image.color = Color.clear;
        currentWingSlot.image.sprite = null;
        currentWingSlot.cooldownCircle.fillAmount = 0;
        currentWingSlot.amountOverlay.SetActive(false);
        currentWingSlot.upgradeText.text = "";
        currentWingSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
        currentWingSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColorForEmptySlot();


        nextWingSlot.button.onClick.RemoveAllListeners();
        nextWingSlot.tooltip.enabled = false;
        nextWingSlot.dragAndDropable.dragable = false;
        nextWingSlot.image.color = Color.clear;
        nextWingSlot.image.sprite = null;
        nextWingSlot.cooldownCircle.fillAmount = 0;
        nextWingSlot.amountOverlay.SetActive(false);
        nextWingSlot.upgradeText.text = "";
        nextWingSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
        nextWingSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColorForEmptySlot();

        materialSlot.button.onClick.RemoveAllListeners();
        materialSlot.tooltip.enabled = false;
        materialSlot.dragAndDropable.dragable = false;
        materialSlot.image.color = Color.clear;
        materialSlot.image.sprite = null;
        materialSlot.cooldownCircle.fillAmount = 0;
        materialSlot.amountOverlay.SetActive(false);
        materialSlot.upgradeText.text = "";
        materialSlot.GetComponent<Image>().sprite = player.itemRarityConfig.GetSprite;
        materialSlot.GetComponent<Image>().color = player.itemRarityConfig.GetColorForEmptySlot();

    }
}
