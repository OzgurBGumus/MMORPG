// Inventories need a slot type to hold Item + Amount. This is better than
// storing .amount in 'Item' because then we can use Item.Equals properly
// any workarounds to ignore the .amount.
//
// Note: always check .amount > 0 before accessing .item.
//       set .amount=0 to clear it.
using System;
using System.Text;
using UnityEngine;

[Serializable]
public partial struct ItemSlot
{
    public Item item;
    public int amount;

    // constructors
    public ItemSlot(Item item, int amount=1)
    {
        this.item = item;
        this.amount = amount;
    }

    // helper functions to increase/decrease amount more easily
    // -> returns the amount that we were able to increase/decrease by
    public int DecreaseAmount(int reduceBy, Player player = null)
    {
        // as many as possible
        int limit = Mathf.Clamp(reduceBy, 0, amount);
        amount -= limit;

        //if we give player parameter, that means this is an inventory slot and need to check quests
        if(player != null)
        {
            player.quests.OnInventoryUpdateTrigger();
        }
        return limit;
    }

    public int IncreaseAmount(int increaseBy, Player player = null)
    {
        // as many as possible
        int limit = Mathf.Clamp(increaseBy, 0, item.maxStack - amount);
        amount += limit;
        //if we give player parameter, that means this is an inventory slot and need to check quests
        if (player != null)
        {
            player.quests.OnInventoryUpdateTrigger();
        }
        return limit;
    }

    // tooltip
    public string ToolTip(int merchantPrice = 0)
    {
        if (amount == 0) return "";

        // we use a StringBuilder so that addons can modify tooltips later too
        // ('string' itself can't be passed as a mutable object)
        string itemTooltip = item.ToolTip();
        if (merchantPrice != 0)
        {
            int lastAmountIndex = itemTooltip.LastIndexOf("Amount");
            string color = "red";
            itemTooltip  = itemTooltip.Substring(0, lastAmountIndex).Trim();
            itemTooltip += "\n\n";
            if(Player.localPlayer.gold >= merchantPrice)
            {
                color = "green";
            }
            itemTooltip += "<color="+ color + ">Price: " + merchantPrice+ "Coin</color>";
            StringBuilder tip = new StringBuilder(itemTooltip);
            return tip.ToString();
        }
        else
        {
            StringBuilder tip = new StringBuilder(itemTooltip);
            tip.Replace("{AMOUNT}", amount.ToString());
            return tip.ToString();
        }
        
    }

}
