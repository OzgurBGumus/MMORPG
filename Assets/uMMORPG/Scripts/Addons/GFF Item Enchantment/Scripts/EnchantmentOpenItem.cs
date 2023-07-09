using UnityEngine;

namespace GFFAddons
{
    [CreateAssetMenu(menuName = "GFF Addons/Item Enchantment/Open Item", order = 999)]
    public class EnchantmentOpenItem : UsableItem
    {
        // usage
        public override void Use(Player player, int inventoryIndex)
        {
            // decrease amount
            ItemSlot slot = player.inventory.slots[inventoryIndex];
            slot.DecreaseAmount(1);
            player.inventory.slots[inventoryIndex] = slot;
        }

        // [Client] OnUse Rpc callback for effects, sounds, etc.
        // -> can't pass slotIndex because .Use might clear it before getting here already
        public override void OnUsed(Player player)
        {
            if (player.isLocalPlayer && player.isClient)
            {
                UIUpgrade.singleton.Show();
            }
        }
    }
}


