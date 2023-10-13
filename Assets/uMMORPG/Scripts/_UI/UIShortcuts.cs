﻿using UnityEngine;
using UnityEngine.UI;

public partial class UIShortcuts : MonoBehaviour
{
    public GameObject panel;

    public Button inventoryButton;
    public GameObject inventoryPanel;

    public Button wingUpgradeButton;
    public GameObject wingUpgradePanel;

    public Button merchantButton;
    public GameObject merchantPanel;

    public Button skillsButton;
    public GameObject skillsPanel;

    public Button characterInfoButton;
    public GameObject characterInfoPanel;

    public Button questsButton;
    public GameObject questsPanel;

    public Button craftingButton;
    public GameObject craftingPanel;

    public Button guildButton;
    public GameObject guildPanel;

    public Button partyButton;
    public GameObject partyPanel;

    public Button itemMallButton;
    public GameObject itemMallPanel;

    public Button gameMasterButton;
    public GameObject gameMasterPanel;

    public Button quitButton;

    void Update()
    {
        Player player = Player.localPlayer;
        if (player != null)
        {
            panel.SetActive(true);

            inventoryButton.onClick.SetListener(() => {
                inventoryPanel.transform.parent.GetComponent<UIInventory>().Toggle();
            });

            wingUpgradeButton.onClick.SetListener(() => {
                wingUpgradePanel.transform.parent.GetComponent<UIWingUpgrade>().Toggle();
            });

            merchantButton.onClick.SetListener(() => {
                if (player.state != "MERCHANT" && merchantPanel.activeSelf != true) {
                    player.merchant.Cleanup();
                }
                merchantPanel.transform.parent.GetComponent<UIMerchant>().Toggle();
            });

            skillsButton.onClick.SetListener(() => {
                skillsPanel.transform.parent.GetComponent<UISkills>().Toggle();
            });

            characterInfoButton.onClick.SetListener(() => {
                characterInfoPanel.transform.parent.GetComponent<UICharacterInfo>().Toggle();
            });

            questsButton.onClick.SetListener(() => {
                questsPanel.SetActive(!questsPanel.activeSelf);
            });

            craftingButton.onClick.SetListener(() => {
                craftingPanel.transform.parent.GetComponent<UIMerchant>().Toggle();
            });

            partyButton.onClick.SetListener(() => {
                partyPanel.SetActive(!partyPanel.activeSelf);
            });

            guildButton.onClick.SetListener(() => {
                guildPanel.SetActive(!guildPanel.activeSelf);
            });

            itemMallButton.onClick.SetListener(() => {
                itemMallPanel.transform.parent.GetComponent<UIItemMall>().Toggle();
            });

            gameMasterButton.gameObject.SetActive(player.isGameMaster);
            gameMasterButton.onClick.SetListener(() => {
                gameMasterPanel.SetActive(!gameMasterPanel.activeSelf);
            });

            // show "(5)Quit" if we can't log out during combat
            // -> CeilToInt so that 0.1 shows as '1' and not as '0'
            string quitPrefix = "";
            if (player.remainingLogoutTime > 0)
                quitPrefix = "(" + Mathf.CeilToInt((float)player.remainingLogoutTime) + ") ";
            quitButton.GetComponent<UIShowToolTip>().text = quitPrefix + "Quit";
            quitButton.interactable = player.remainingLogoutTime == 0;
            quitButton.onClick.SetListener(() => {
                NetworkManagerMMO.Quit();
            });
        }
        else panel.SetActive(false);
    }

    public void CloseAllPanels(bool exceptMerchant = false, bool exceptInventory = false)
    {
        if (inventoryPanel.activeSelf && !exceptInventory)
        {
            inventoryPanel.SetActive(false);
        }
        if (merchantPanel.activeSelf && !exceptMerchant)
        {
            merchantPanel.SetActive(false);
        }
        if (skillsPanel.activeSelf)
        {
            skillsPanel.SetActive(false);
        }
        if (questsPanel.activeSelf)
        {
            questsPanel.SetActive(false);
        }
        if (craftingPanel.activeSelf)
        {
            craftingPanel.SetActive(false);
        }
        if (guildPanel.activeSelf)
        {
            guildPanel.SetActive(false);
        }
        if (partyPanel.activeSelf)
        {
            partyPanel.SetActive(false);
        }
        if (itemMallPanel.activeSelf)
        {
            itemMallPanel.SetActive(false);
        }

    }
}
