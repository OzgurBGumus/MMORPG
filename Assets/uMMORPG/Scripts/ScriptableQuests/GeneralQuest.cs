// a simple gather quest example
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(menuName="uMMORPG Quest/General Quest", order=999)]
public class GeneralQuest : ScriptableQuest
{
    

    [Serializable]
    public class GatherQuestItems
    {
        public ScriptableItem gatherItem;
        public int gatherAmount;
    }

    [Serializable]
    public class KillQuestMonsters
    {
        public Monster killTarget;
        public int killAmount;
    }

    
    public List<GatherQuestItems> GatherItemList;
    public List<KillQuestMonsters> KillMonsterList;
    public List<string> LocationList;

    // events //////////////////////////////////////////////////////////////////
    
    public override void OnKilled(Player player, int questIndex, Entity victim)
    {
        // not done yet, and same name as prefab? (hence same monster?)
        Quest quest = player.quests.quests[questIndex];
        for (int i = 0; i < KillMonsterList.Count; i++)
        {
            if (quest.progress[i] < KillMonsterList[i].killAmount && victim.name == KillMonsterList[i].killTarget.name)
            {
                // increase int field in quest (up to 'amount')
                ++quest.progress[i];
                player.quests.quests[questIndex] = quest;
            }
        }


    }

    public override void OnInventoryUpdate(Player player, int questIndex)
    {
        Quest quest = player.quests.quests[questIndex];
        for (int i = 0; i < GatherItemList.Count; i++)
        {
            quest.progress[KillMonsterList.Count + i] = player.inventory.Count(new Item(GatherItemList[i].gatherItem));
        }

    }

    public override void OnLocation(Player player, int questIndex, Collider location)
    {
        // the location counts if it has exactly the same name as the quest.
        // simple and stupid.
        for(int i = 0; i< LocationList.Count; i++)
        {
            if (location.name == LocationList[i])
            {
                Quest quest = player.quests.quests[questIndex];
                quest.progress[KillMonsterList.Count + GatherItemList.Count + i] = 1;
                player.quests.quests[questIndex] = quest;
            }
        }
        
    }

    // fulfillment /////////////////////////////////////////////////////////////
    bool IsFulfilledKill(Player player, Quest quest)
    {
        for (int i = 0; i < KillMonsterList.Count; i++)
        {
            if (quest.progress[i] < KillMonsterList[i].killAmount) return false;
        }
        return true;
    }
    bool IsFulfilledGathering(Player player, Quest quest)
    {
        if (readyToComplete) return true;

        for(int i = 0; i < GatherItemList.Count; i++)
        {
            if(player.inventory.Count(new Item(GatherItemList[i].gatherItem)) < GatherItemList[i].gatherAmount)
            {
                return false;
            }
        }
        readyToComplete = true;
        return readyToComplete;
    }
    
    bool IsFulfilledLocation(Player player, Quest quest)
    {
        for(int i = 0; i < LocationList.Count; i++)
        {
            if (quest.progress[KillMonsterList.Count + GatherItemList.Count + i] == 0) return false;
        }
        return true;
    }
    public override bool IsFulfilled(Player player, Quest quest)
    {
        return IsFulfilledGathering(player, quest) && IsFulfilledKill(player, quest) && IsFulfilledLocation(player, quest);
    }

    public override void OnCompleted(Player player, Quest quest)
    {
        for (int i = 0; i < GatherItemList.Count; i++)
        {
            // remove gathered items from player's inventory
            if (GatherItemList[i] != null)
                player.inventory.Remove(new Item(GatherItemList[i].gatherItem), GatherItemList[i].gatherAmount);
        }
            
            
    }

    // tooltip /////////////////////////////////////////////////////////////////
    public override string ToolTip(Player player, Quest quest, bool isShort = false)
    {
        StringBuilder tip = new StringBuilder();
        // we use a StringBuilder so that addons can modify tooltips later too
        // ('string' itself can't be passed as a mutable object)
        if (!isShort)
        {
            tip.Append(base.ToolTip(player, quest, isShort));
        }
        else
        {
            for (int i = 0; i < KillMonsterList.Count; i++)
            {
                tip.Append($"<color=red><link={KillMonsterList[i].killTarget.name}>{KillMonsterList[i].killTarget.name}</link></color>: {quest.progress[i].ToString()}/{KillMonsterList[i].killAmount.ToString()}\n");

            }

            for (int i = 0; i < GatherItemList.Count; i++)
            {

                tip.Append($"<color=green><link={GatherItemList[i].gatherItem.name}>{GatherItemList[i].gatherItem.name}</link></color>: {quest.progress[KillMonsterList.Count + i].ToString()}/{GatherItemList[i].gatherAmount.ToString()}\n");
            }

            for(int i = 0; i < LocationList.Count; i++)
            {
                tip.Append($"<color=yellow><link={LocationList[i]}>{LocationList[i]}</link></color>: {quest.progress[KillMonsterList.Count + GatherItemList.Count + i].ToString()}\n");
            }
        }
        
        return tip.ToString();
    }
    public override int GetMissionCount()
    {
        return GatherItemList.Count + KillMonsterList.Count + LocationList.Count;
    }
}
