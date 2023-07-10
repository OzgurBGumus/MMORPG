using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUniqueWindow : MonoBehaviour
{
    public GameObject Inventory;
    public GameObject Merchant;
    public GameObject Upgrade;
    public GameObject Skills;
    public GameObject Crafting;
    public GameObject Guild;
    public GameObject ItemMall;
    public GameObject Loot;
    public GameObject Quests;
    public GameObject PlayerTrading;
    public GameObject Party;
    public GameObject GameMasterTool;
    public GameObject NpcDialogue;
    public GameObject NpcQuests;
    public GameObject NpcTrading;
    public GameObject NpcGuildManagement;
    public GameObject NpcRevive;
    public GameObject CharacterInfo;
    public void CloseWindows(
        bool inventory = true, bool merchant = true, bool upgrade = true, bool skills = true, bool crafting = true, bool guild = true, 
        bool itemMall = true, bool loot = true, bool quests = true, bool playerTrading = true, bool party = true, bool gameMasterTool = true,
        bool npcDialogue = true, bool npcQuests = true, bool npcTrading = true, bool npcGuildManagement = true, bool npcRevive = true, bool characterInfo = true
        )
    {
        if(inventory) Inventory.SetActive(false);
        if(merchant) Merchant.SetActive(false);
        if (upgrade) Upgrade.SetActive(false);
        if (skills) Skills.SetActive(false);
        if (crafting) Crafting.SetActive(false);
        if (guild) Guild.SetActive(false);
        if (itemMall) ItemMall.SetActive(false);
        if (loot) Loot.SetActive(false);
        if (quests) Quests.SetActive(false);
        if (playerTrading) PlayerTrading.SetActive(false);
        if (party) Party.SetActive(false);
        if (gameMasterTool) GameMasterTool.SetActive(false);
        if (npcDialogue) NpcDialogue.SetActive(false);
        if (npcQuests) NpcQuests.SetActive(false);
        if (npcTrading) NpcTrading.SetActive(false);
        if (npcGuildManagement) NpcGuildManagement.SetActive(false);
        if (npcRevive) NpcRevive.SetActive(false);
        if (characterInfo) CharacterInfo.SetActive(false);
    }
}
