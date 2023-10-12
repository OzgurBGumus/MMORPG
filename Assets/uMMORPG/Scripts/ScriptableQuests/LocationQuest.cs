//// a simple location quest example
////
//// how to use locations:
//// - create an empty GameObject
//// - add a SphereCollider with IsTrigger enabled
//// - set the layer to IgnoreRaycast so that we can still click through it
//// - set the tag to QuestLocation so that it's forwarded to the quest system
//// - name it the same as the quest
////
//// if you need multiple locations for one quest, then create another quest type
//// with a list of location names and set the separate bits in 'field0' to keep
//// track of what was visited.
//using UnityEngine;
//using System.Text;

//[CreateAssetMenu(menuName="uMMORPG Quest/Location Quest", order=999)]
//public class LocationQuest : ScriptableQuest
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
//            tip = new StringBuilder(base.ToolTip(player, quest, isShort));
//            tip.Replace("{LOCATIONSTATUS}", quest.progress[0] == 0 ? "Pending" : "Done");
//        }
//        else
//        {

//        }

//        return tip.ToString();
//    }
//}
