using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UINpcQuests : MonoBehaviour
{
    public static UINpcQuests singleton;
    public Text title;
    public Image NpcAvatar;

    public GameObject panel;
    public UIQuestMissionSlot slotMissionPrefab;
    public UIQuestRewardSlot slotRewardPrefab;
    public Text descriptionText;
    public Transform contentMission;
    public Transform contentReward;

    public Button accept;

    public string currentQuestName;

    public UINpcQuests()
    {
        // assign singleton only once (to work with DontDestroyOnLoad when
        // using Zones / switching scenes)
        if (singleton == null) singleton = this;
    }

    void Update()
    {
        Player player = Player.localPlayer;

        // use collider point(s) to also work with big entities
        if (panel.activeSelf && player != null &&
            player.target != null &&
            player.target is Npc npc &&
            Utils.ClosestDistance(player, player.target) <= player.interactionRange)
        {
            // instantiate/destroy enough slots
            ScriptableQuest npcQuest = npc.quests.GetQuestsFor(player, currentQuestName);
            UIUtils.BalancePrefabs(slotRewardPrefab.gameObject,
                npcQuest.rewardItems.Count + 
                (npcQuest.rewardGold != 0 ? 1 : 0) +
                (npcQuest.rewardExperience != 0 ? 1 : 0)
                , contentReward);

            
            FillContentMission(npcQuest);
            // find quest index in original npc quest list (unfiltered)
            int npcIndex = npc.quests.GetIndexByName(npcQuest.name);

            // find quest index in player quest list
            int questIndex = player.quests.GetIndexByName(npcQuest.name);

            title.text = npcQuest.name;

            int currentLastIndexOfRewards = contentReward.childCount - 1;
            if (npcQuest.rewardExperience > 0)
            {
                UIQuestRewardSlot slot = contentReward.GetChild(currentLastIndexOfRewards).GetComponent<UIQuestRewardSlot>();
                slot.descriptionText.text = npcQuest.rewardExperience.ToString();
                slot.nameText.text = "Experience";
                currentLastIndexOfRewards--;
            }
            if(npcQuest.rewardGold > 0)
            {
                UIQuestRewardSlot slot = contentReward.GetChild(currentLastIndexOfRewards).GetComponent<UIQuestRewardSlot>();
                slot.descriptionText.text = "Gold";
                slot.nameText.text = npcQuest.rewardGold.ToString();
            }
            //fill Reward Prefabs
            for(int i=0; i < npcQuest.rewardItems.Count; i++)
            {
                UIQuestRewardSlot slot = contentReward.GetChild(i).GetComponent<UIQuestRewardSlot>();
                slot.descriptionText.text = npcQuest.rewardItems[i].count.ToString();
                slot.nameText.text = npcQuest.rewardItems[i].item.name;
            }
            if (questIndex != -1)
            {
                // running quest: shows description with current progress
                // instead of static one
                Quest quest = player.quests.quests[questIndex];
                bool hasSpace = npcQuest.rewardItems == null || player.inventory.SlotsFree() >= npcQuest.rewardItems.Count;

                // description + not enough space warning (if needed)
                descriptionText.text = quest.ToolTip(player);
                if (!hasSpace)
                    descriptionText.text += "\n<color=red>Not enough inventory space!</color>";

                accept.interactable = player.quests.CanComplete(quest.name);
                accept.GetComponentInChildren<Text>().text = "Complete";
                accept.onClick.SetListener(() => {
                    player.quests.CmdComplete(npcIndex);
                    panel.SetActive(false);
                });
            }
            else
            {
                // new quest
                descriptionText.text = new Quest(npcQuest).ToolTip(player);
                accept.interactable = true;
                accept.GetComponentInChildren<Text>().text = "Accept";
                accept.onClick.SetListener(() => {
                    player.quests.CmdAccept(npcIndex);
                    currentQuestName = "";
                    panel.SetActive(false);
                });
            }
        }
        else panel.SetActive(false);
    }

    private void FillContentMission(ScriptableQuest npcQuest)
    {
        UIUtils.BalancePrefabs(slotMissionPrefab.gameObject, npcQuest.GetMissionCount(), contentMission);
        if (npcQuest != null)
        {
            if(npcQuest is GeneralQuest generalQuest)
            {
                int currentTask = 0;
                for (int i=0; i< generalQuest.KillMonsterList.Count; i++)
                {
                    UIQuestMissionSlot slot = contentMission.GetChild(currentTask).GetComponent<UIQuestMissionSlot>();
                    slot.descriptionText.text = generalQuest.KillMonsterList[currentTask].killAmount.ToString();
                    slot.nameButton.text = generalQuest.KillMonsterList[currentTask].killTarget.name;
                    currentTask++;
                }
                for (int i = 0; i < generalQuest.GatherItemList.Count; i++)
                {
                    UIQuestMissionSlot slot = contentMission.GetChild(currentTask).GetComponent<UIQuestMissionSlot>();
                    slot.descriptionText.text = generalQuest.GatherItemList[i].gatherAmount.ToString();
                    slot.nameButton.text = generalQuest.GatherItemList[i].gatherItem.name;
                    currentTask++;
                }
                for (int i = 0; i < generalQuest.LocationList.Count; i++)
                {
                    UIQuestMissionSlot slot = contentMission.GetChild(currentTask).GetComponent<UIQuestMissionSlot>();
                    slot.descriptionText.text = "";
                    slot.nameButton.text = generalQuest.LocationList[i];
                    currentTask++;
                }
            }
        }
    }
}
