// a simple gather quest example
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(menuName="uMMORPG Quest/Gather Quest", order=999)]
public class GatherQuest : ScriptableQuest
{
    [Serializable]
    public class GatherQuestItems
    {
        public ScriptableItem gatherItem;
        public int gatherAmount;
    }
    public List<GatherQuestItems> GatherItemList;
    

    // events //////////////////////////////////////////////////////////////////
    public override void OnInventoryUpdate(Player player, int questIndex)
    {
        Quest quest = player.quests.quests[questIndex];
        for (int i = 0; i < GatherItemList.Count; i++)
        {
            quest.progress[i] = player.inventory.Count(new Item(GatherItemList[i].gatherItem));
        }

    }


    // fulfillment /////////////////////////////////////////////////////////////
    public override bool IsFulfilled(Player player, Quest quest)
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
            for (int i = 0; i < GatherItemList.Count; i++)
            {

                tip.Append($"<color=green><link={GatherItemList[i].gatherItem.name}>{GatherItemList[i].gatherItem.name}</link></color>: {quest.progress[i].ToString()}/{GatherItemList[i].gatherAmount.ToString()}\n");
            }
        }
        
        return tip.ToString();
    }
    public override int GetMissionCount()
    {
        return GatherItemList.Count;
    }
}
