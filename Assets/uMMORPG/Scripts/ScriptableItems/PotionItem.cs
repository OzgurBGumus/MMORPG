using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName="uMMORPG Item/Potion", order=999)]
public class PotionItem : UsableItem
{
    [Header("Potion")]
    public int usageHealth;
    public int usageMana;
    public int usageExperience;
    public int usagePetHealth; // to heal pet

    // usage
    public override void Use(Player player, int inventoryIndex)
    {
        // always call base function too
        base.Use(player, inventoryIndex);

        // increase health/mana/etc.
        player.health.current += usageHealth;
        player.mana.current += usageMana;
        player.experience.current += usageExperience;
        if (player.petControl.activePet != null)
            player.petControl.activePet.health.current += usagePetHealth;

        // decrease amount
        ItemSlot slot = player.inventory.slots[inventoryIndex];
        slot.DecreaseAmount(1, player);
        player.inventory.slots[inventoryIndex] = slot;
    }

    // tooltip
    public override string ToolTip()
    {
        string tooltip = 
            "<b>{NAME}</b>\n" +
                        "\n" +
            (usageHealth != 0 ? "Restores " + usageHealth.ToString() + " Health on use.\n" : "") +
            (usageMana != 0 ? "Restores " + usageMana.ToString() + " Mana on use.\n" : "") +
            (usagePetHealth != 0 ? "Restores " + usagePetHealth.ToString() + " Pet Health on use.\n" : "") +
            (usageExperience != 0 ? "Gain " + usageExperience.ToString() + " Experience on use.\n" : "") +
            "Destroyable: " + (destroyable ? "Yes" : "No") + "\n" +
            "Sellable: " + (sellable ? "Yes" : "No") + "\n" +
            "Tradable: " + (tradable ? "Yes" : "No") + "\n" +
            "Required Level: " + minLevel + "\n" +
                        "\n" +
            "Price: " + buyPrice.ToString() + " Gold\n" +
            "<i>Sells for: " + sellPrice.ToString() + " Gold</i>\n" +
            "\n"+
            base.toolTip;



        return tooltip;
    }
}
