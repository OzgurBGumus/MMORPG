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
                npcQuest.rewardItem.Count + 
                (npcQuest.rewardGold != 0 ? 1 : 0) +
                (npcQuest.rewardExperience != 0 ? 1 : 0)
                , contentReward);

            UIUtils.BalancePrefabs(slotMissionPrefab.gameObject, 1, contentMission);

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
            for(int i=0; i < npcQuest.rewardItem.Count; i++)
            {
                UIQuestRewardSlot slot = contentReward.GetChild(i).GetComponent<UIQuestRewardSlot>();
                slot.descriptionText.text = npcQuest.rewardItemCount[i].ToString();
                slot.nameText.text = npcQuest.rewardItem[i].name;
            }
            if (questIndex != -1)
            {
                // running quest: shows description with current progress
                // instead of static one
                Quest quest = player.quests.quests[questIndex];
                List<ScriptableItem> reward = npcQuest.rewardItem;
                bool hasSpace = reward == null || player.inventory.SlotsFree() >= reward.Count;

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
                    this.gameObject.SetActive(false);
                });
            }
        }
        else panel.SetActive(false);
    }
}
