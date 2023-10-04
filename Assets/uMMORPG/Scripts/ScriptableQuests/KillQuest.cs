// a simple kill quest example.
// inherit from KillQuest and overwrite OnKilled for more advanced goals like
// 'kill a player 10 levels above you' or 'kill a pet in a guild war' etc.
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName="uMMORPG Quest/Kill Quest", order=999)]
public class KillQuest : ScriptableQuest
{
    [Serializable]
    public class KillQuestMonsters
    {
        public Monster killTarget;
        public int killAmount;
    }
    public List<KillQuestMonsters> KillMonsterList;

    // events //////////////////////////////////////////////////////////////////
    public override void OnKilled(Player player, int questIndex, Entity victim)
    {
        // not done yet, and same name as prefab? (hence same monster?)
        Quest quest = player.quests.quests[questIndex];
        for(int i = 0; i < KillMonsterList.Count; i++)
        {
            if (quest.progress[i] < KillMonsterList[i].killAmount && victim.name == KillMonsterList[i].killTarget.name)
            {
                // increase int field in quest (up to 'amount')
                ++quest.progress[i];
                player.quests.quests[questIndex] = quest;
            }
        }
        

    }

    // fulfillment /////////////////////////////////////////////////////////////
    public override bool IsFulfilled(Player player, Quest quest)
    {
        for(int i = 0; i < quest.progress.Count; i++)
        {
            if (quest.progress[i] < KillMonsterList[i].killAmount) return false;
        }
        return true;
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
            string color = "";
            for (int i = 0; i < KillMonsterList.Count; i++)
            {
               if( UIQuestInfos.singleton.targets.TargetPositions.ContainsKey(KillMonsterList[i].killTarget.name.ToLower()))
               {
                    color = "red";
               }
               else
               {
                    color = "blue";
               }
                tip.Append($"<color={color}><link={KillMonsterList[i].killTarget.name}>{KillMonsterList[i].killTarget.name}</link></color>: {quest.progress[i].ToString()}/{KillMonsterList[i].killAmount.ToString()}\n");

            }
        }
        StringBuilder targetsString = new StringBuilder();
        //for(int i = 0;i < killTargets.Count; i++)
        //{
        //    targetsString.AppendFormat("{0}: {1}/{2} \n", killTargets[i] != null ? killTargets[i].name : "", killAmounts[i].ToString(), quest.progress[i].ToString());
        //}
        return tip.ToString();
    }

    public override int GetMissionCount()
    {
        return KillMonsterList.Count;
    }
}
