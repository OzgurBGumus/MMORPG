//// a simple kill quest example.
//// inherit from KillQuest and overwrite OnKilled for more advanced goals like
//// 'kill a player 10 levels above you' or 'kill a pet in a guild war' etc.
//using UnityEngine;
//using System.Text;
//using System.Collections.Generic;
//using System;

//[CreateAssetMenu(menuName="uMMORPG Quest/Kill Quest", order=999)]
//public class KillQuest : ScriptableQuest
//{
    
    

//    // events //////////////////////////////////////////////////////////////////
    

//    // fulfillment /////////////////////////////////////////////////////////////
    

//    // tooltip /////////////////////////////////////////////////////////////////
//    public override string ToolTip(Player player, Quest quest, bool isShort = false)
//    {
//        StringBuilder tip = new StringBuilder();
//        // we use a StringBuilder so that addons can modify tooltips later too
//        // ('string' itself can't be passed as a mutable object)
//        if (!isShort)
//        {
//            tip.Append(base.ToolTip(player, quest, isShort));

//        }
//        else
//        {
            
            
//        }
//        StringBuilder targetsString = new StringBuilder();
//        //for(int i = 0;i < killTargets.Count; i++)
//        //{
//        //    targetsString.AppendFormat("{0}: {1}/{2} \n", killTargets[i] != null ? killTargets[i].name : "", killAmounts[i].ToString(), quest.progress[i].ToString());
//        //}
//        return tip.ToString();
//    }

//    public override int GetMissionCount()
//    {
//        return KillMonsterList.Count;
//    }
//}
